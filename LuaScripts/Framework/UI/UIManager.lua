--[[
-- added by wsh @ 2017-11-30
-- UI管理系统：提供UI操作、UI层级、UI消息、UI资源加载、UI调度、UI缓存等管理
-- 注意：
-- 1、Window包括：Model、Ctrl、View、和Active状态等构成的一个整体概念
-- 2、所有带Window接口的都是操作整个窗口，如CloseWindow以后：整个窗口将不再活动
-- 3、所有带View接口的都是操作视图层展示，如CloseView以后：View、Model依然活跃，只是看不见，可看做切入了后台
-- 4、如果只是要监听数据，可以创建不带View、Ctrl的后台窗口，配置为nil，比如多窗口需要共享某控制model（配置为后台窗口）
-- 5、可将UIManager看做一个挂载在UIRoot上的不完全UI组件，但是它是Singleton，不使用多重继承，UI组件特性隐式实现
--]]

local Messenger = require "Framework.Common.Messenger"
---@class UIManager:Singleton
local UIManager = BaseClass("UIManager", Singleton)

-- UIRoot路径
local UIRootPath = "UIRoot"
-- EventSystem路径
local EventSystemPath = "EventSystem"
-- UICamera路径
local UICameraPath = UIRootPath .. "/UICamera"
-- 分辨率
local Resolution = Vector2.New(1024, 768)
-- 窗口最大可使用的相对order_in_layer
local MaxOderPerWindow = 10
-- cs Tip
local UINoticeTip = CS.UINoticeTip.Instance

--region  --私有方法，当前内部使用

-- 激活窗口
---@param self UIManager
---@param target UIWindow
local function ActivateWindow(self, target, ...)
    assert(target)
    assert(target.IsLoading == false, "You can only activate window after prefab locaded!")
    target.Model:Activate(...)
    target.View:SetActive(true)
    self.activated_windows[target.Name] = target
    self:Broadcast(UIMessageNames.UIFRAME_ON_WINDOW_OPEN, target)
end

-- 反激活窗口
local function Deactivate(self, target)
    target.Model:Deactivate()
    target.View:SetActive(false)
    Logger.Log(">>>Deactivate")
    PrintTable(self.activated_windows)
    self.activated_windows[target.Name] = nil
    self:Broadcast(UIMessageNames.UIFRAME_ON_WINDOW_CLOSE, target)
end

-- 打开窗口：私有，必要时准备资源
local function InnerOpenWindow(self, target, ...)
    assert(target)
    assert(target.Model)
    assert(target.Ctrl)
    assert(target.View)
    assert(target.Active == false, "You should close window before open again!")

    target.Active = true
    local has_view = target.View ~= UIBaseView
    local has_prefab_res = target.PrefabPath and #target.PrefabPath > 0
    local has_loaded = not IsNull(target.View.gameObject)
    local need_load = has_view and has_prefab_res and not has_loaded
    if not need_load then
        ActivateWindow(self, target, ...)
    elseif not target.IsLoading then
        target.IsLoading = true
        local params = SafePack(...)
        SingleGet.GameObjectPool():GetGameObjectAsync(target.PrefabPath, function(go)
            if IsNull(go) then
                return
            end

            local trans = go.transform
            trans:SetParent(target.Layer.transform)
            trans.name = target.Name

            target.IsLoading = false
            target.View:OnCreate()
            if target.Active then
                ActivateWindow(self, target, SafeUnpack(params))
            end
        end)
    end
end

-- 关闭窗口：私有
local function InnerCloseWindow(self, target)
    assert(target)
    assert(target.Model)
    assert(target.Ctrl)
    assert(target.View)
    if target.Active then
        Deactivate(self, target)
        target.Active = false
    end
end

local function InnerDelete(plugin)
    if plugin.__ctype == ClassType.instance then
        plugin:Delete()
    end
end

local function InnerDestroyWindow(self, ui_name, target, include_keep_model)
    self:Broadcast(UIMessageNames.UIFRAME_ON_WINDOW_DESTROY, target)
    -- 说明：一律缓存，如果要真的清理，那是清理缓存时需要管理的功能
    SingleGet.GameObjectPool():RecycleGameObject(self.windows[ui_name].PrefabPath, target.View.gameObject)
    if include_keep_model then
        self.keep_model[ui_name] = nil
        InnerDelete(target.Model)
    elseif not self.keep_model[ui_name] then
        InnerDelete(target.Model)
    end
    InnerDelete(target.Ctrl)
    InnerDelete(target.View)
    self.windows[ui_name] = nil
end



--endregion

--region --公有方法，暴露给外部调用

-- 构造函数
function UIManager:__init()
    -- 成员变量
    -- 消息中心
    self.ui_message_center = Messenger.New()
    -- 所有存活的窗体
    ---@type table<int, UIWindow>
    self.windows = {}
    ---@type table<int, UIWindow>
    self.activated_windows = {}
    -- 所有可用的层级
    self.layers = {}
    -- 保持Model
    self.keep_model = {}
    -- 窗口记录队列
    self.__window_stack = {}
    -- 是否启用记录
    self.__enable_record = true

    -- 初始化组件
    self.gameObject = CS.UnityEngine.GameObject.Find(UIRootPath)
    self.transform = self.gameObject.transform
    self.camera_go = CS.UnityEngine.GameObject.Find(UICameraPath)
    self.UICamera = self.camera_go:GetComponent(typeof(CS.UnityEngine.Camera))
    self.Resolution = Resolution
    self.MaxOderPerWindow = MaxOderPerWindow
    CS.UnityEngine.Object.DontDestroyOnLoad(self.gameObject)
    local event_system = CS.UnityEngine.GameObject.Find(EventSystemPath)
    CS.UnityEngine.Object.DontDestroyOnLoad(event_system)
    assert(not IsNull(self.transform))
    assert(not IsNull(self.UICamera))

    -- 初始化层级
    local layers = table.choose(Config.Debug and getmetatable(UILayers) or UILayers, function(k, v)
        return type(v) == "table" and v.OrderInLayer ~= nil and v.Name ~= nil and type(v.Name) == "string" and #v.Name > 0
    end)
    table.walksort(layers, function(lkey, rkey)
        return layers[lkey].OrderInLayer < layers[rkey].OrderInLayer
    end, function(index, layer)
        assert(self.layers[layer.Name] == nil, "Aready exist layer : " .. layer.Name)
        local go = CS.UnityEngine.GameObject(layer.Name)
        local trans = go.transform
        trans:SetParent(self.transform)
        local new_layer = UILayer.New(self, layer.Name)
        new_layer:OnCreate(layer)
        self.layers[layer.Name] = new_layer
    end)
end

-- 注册消息
function UIManager:AddListener(e_type, e_listener)
    self.ui_message_center:AddListener(e_type, e_listener)
end

-- 发送消息
function UIManager:Broadcast(e_type, ...)
    self.ui_message_center:Broadcast(e_type, ...)
end

-- 注销消息
function UIManager:RemoveListener(e_type, e_listener)
    self.ui_message_center:RemoveListener(e_type, e_listener)
end

-- 获取窗口
function UIManager:GetWindow(ui_name, active, view_active)
    local target = self.windows[ui_name]
    if target == nil then
        return nil
    end
    if active ~= nil and target.Active ~= active then
        return nil
    end
    if view_active ~= nil and target.View:GetActive() ~= view_active then
        return nil
    end
    return target
end

-- 初始化窗口
function UIManager:InitWindow(ui_name, window)
    local config = UIConfig[ui_name]
    assert(config, "No window named : " .. ui_name .. ".You should add it to UIConfig first!")

    local layer = self.layers[config.Layer.Name]
    assert(layer, "No layer named : " .. config.Layer.Name .. ".You should create it first!")

    window.Name = ui_name
    if self.keep_model[ui_name] then
        window.Model = self.keep_model[ui_name]
    elseif config.Model then
        window.Model = config.Model.New(ui_name)
    end
    if config.Ctrl then
        window.Ctrl = config.Ctrl.New(window.Model)
    end
    if config.View then
        window.View = config.View.New(layer, window.Name, window.Model, window.Ctrl)
    end
    window.Active = false
    window.Layer = layer
    window.PrefabPath = config.PrefabPath

    self:Broadcast(UIMessageNames.UIFRAME_ON_WINDOW_CREATE, window)
    return window
end



-- 打开窗口：公有
function UIManager:OpenWindow(ui_name, ...)
    local target = self:GetWindow(ui_name)
    if not target then
        local window = UIWindow.New()
        self.windows[ui_name] = window
        target = self:InitWindow(ui_name, window)
    end

    -- 先关闭
    InnerCloseWindow(self, target)
    InnerOpenWindow(self, target, ...)

    -- 窗口记录
    local layer = UIConfig[ui_name].Layer
    if layer == UILayers.BackgroundLayer then
        local bg_index = self:GetLastBgWindowIndexInWindowStack()
        if bg_index == -1 or self.__window_stack[bg_index] ~= target.Name then
            self:AddToWindowStack(target.Name)
        else
            self:PopWindowStack()
        end
    elseif layer == UILayers.NormalLayer then
        self:AddToWindowStack(target.Name)
    end
end

-- 关闭窗口：公有
function UIManager:CloseWindow(ui_name)
    local target = self:GetWindow(ui_name, true)
    if not target then
        return
    end

    InnerCloseWindow(self, target)

    -- 窗口记录
    local layer = UIConfig[ui_name].Layer
    if layer == UILayers.BackgroundLayer then
        if target.Name == self.__window_stack[table.count(self.__window_stack)] then
            self:RemoveFormWindowStack(target.Name, true)
            --self:PopWindowStack()
        else
            self:RemoveFormWindowStack(target.Name, true)
        end
    elseif layer == UILayers.NormalLayer then
        self:RemoveFormWindowStack(target.Name, true)
    end
end

-- 关闭层级所有窗口
function UIManager:CloseWindowByLayer(layer)
    for _, v in pairs(self.windows) do
        if v.Layer:GetName() == layer.Name then
            InnerCloseWindow(self, v)
        end
    end
end

-- 关闭其它层级窗口
function UIManager:CloseWindowExceptLayer(layer)
    for _, v in pairs(self.windows) do
        if v.Layer:GetName() ~= layer.Name then
            InnerCloseWindow(self, v)
        end
    end
end

-- 关闭所有窗口
function UIManager:CloseAllWindows()
    for _, v in pairs(self.windows) do
        InnerCloseWindow(self, v)
    end
end

-- 展示窗口
function UIManager:OpenView(ui_name, ...)
    local target = self:GetWindow(ui_name)
    assert(target, "Try to show a window that does not exist: " .. ui_name)
    if not target.View:GetActive() then
        target.View:SetActive(true)
    end
end



-- 隐藏窗口
function UIManager:CloseView(ui_name)
    local target = self:GetWindow(ui_name)
    assert(target, "Try to hide a window that does not exist: " .. ui_name)
    if target.View:GetActive() then
        target.View:SetActive(false)
    end
end



-- 销毁窗口
function UIManager:DestroyWindow(ui_name, include_keep_model)
    local target = self:GetWindow(ui_name)
    if not target then
        return
    end

    InnerCloseWindow(self, target)
    InnerDestroyWindow(self, ui_name, target, include_keep_model)
end

-- 销毁层级所有窗口
function UIManager:DestroyWindowByLayer(layer, include_keep_model)
    for k, v in pairs(self.windows) do
        if v.Layer:GetName() == layer.Name then
            InnerCloseWindow(self, v)
            InnerDestroyWindow(self, k, v, include_keep_model)
        end
    end
end

-- 销毁其它层级窗口
function UIManager:DestroyWindowExceptLayer(layer, include_keep_model)
    for k, v in pairs(self.windows) do
        if v.Layer:GetName() ~= layer.Name then
            InnerCloseWindow(self, v)
            InnerDestroyWindow(self, k, v, include_keep_model)
        end
    end
end

-- 销毁所有窗口
function UIManager:DestroyAllWindow(include_keep_model)
    for k, v in pairs(self.windows) do
        InnerCloseWindow(self, v)
        InnerDestroyWindow(self, k, v, include_keep_model)
    end
end

-- 设置是否保持Model
function UIManager:SetKeepModel(ui_name, keep)
    local target = self:GetWindow(ui_name)
    assert(target, "Try to keep a model that window does not exist: " .. ui_name)
    if keep then
        self.keep_model[target.Name] = target.Model
    else
        self.keep_model[target.Name] = nil
    end
end

-- 获取保持的Model
function UIManager:GetKeepModel(ui_name)
    return self.keep_model[ui_name]
end

-- 加入窗口记录栈
function UIManager:AddToWindowStack(ui_name)
    if not self.__enable_record then
        return
    end

    table.insert(self.__window_stack, ui_name)
    -- 保持Model
    self:SetKeepModel(ui_name, true)
end

-- 从窗口记录栈中移除
function UIManager:RemoveFormWindowStack(ui_name, only_check_top)
    if not self.__enable_record then
        return
    end

    local index = table.indexof(self.__window_stack, ui_name)
    if not index then
        return
    end
    if only_check_top and index ~= table.count(self.__window_stack) then
        return
    end

    local ui_name = table.remove(self.__window_stack, index)
    -- 取消Model保持
    self:SetKeepModel(ui_name, false)
end

-- 获取记录栈
function UIManager:GetWindowStack()
    return self.__window_stack
end

-- 清空记录栈
function UIManager:ClearWindowStack()
    self.__window_stack = {}
end

-- 查看对应窗口是否显示
---@param pName string
function UIManager:IsShowing(pName)
    assert(pName, "窗口名为空，请检查")
    PrintTable(self.activated_windows)
    Logger.Log(">>>>window name %s", self.activated_windows[pName])
    return self.activated_windows[pName] ~= nil
end

-- 获取最后添加的一个背景窗口索引
function UIManager:GetLastBgWindowIndexInWindowStack()
    local bg_index = -1
    for i = 1, table.count(self.__window_stack) do
        local ui_name = self.__window_stack[i]
        if UIConfig[ui_name].Layer == UILayers.BackgroundLayer then
            bg_index = i
        end
    end
    return bg_index
end

-- 弹出栈
-- 注意：从上一个记录的背景UI开始弹出之后所有被记录的窗口
function UIManager:PopWindowStack()
    local bg_index = self:GetLastBgWindowIndexInWindowStack()
    if bg_index == -1 then
        -- 没找到背景UI
        if table.count(self.__window_stack) > 0 then
            error("There is something wrong!")
        end
        return
    end

    self.__enable_record = false
    local end_index = table.count(self.__window_stack)
    for i = bg_index + 1, end_index do
        local ui_name = self.__window_stack[i]
        SingleGet.UIManager():OpenWindow(ui_name)
    end
    self.__enable_record = true
end

-- 展示Tip：单按钮
function UIManager:OpenOneButtonTip(title, content, btnText, callback)
    local ui_name = UIWindowNames.UINoticeTip
    local cs_func = UINoticeTip.ShowOneButtonTip
    self:OpenWindow(ui_name, cs_func, title, content, btnText, callback)
end

-- 展示Tip：双按钮
function UIManager:OpenTwoButtonTip(title, content, btnText1, btnText2, callback1, callback2)
    local ui_name = UIWindowNames.UINoticeTip
    local cs_func = UINoticeTip.ShowTwoButtonTip
    self:OpenWindow(ui_name, cs_func, title, content, btnText1, btnText2, callback1, callback2)
end

-- 展示Tip：三按钮
function UIManager:OpenThreeButtonTip(title, content, btnText1, btnText2, btnText3, callback1, callback2, callback3)
    local ui_name = UIWindowNames.UINoticeTip
    local cs_func = UINoticeTip.ShowThreeButtonTip
    self:OpenWindow(ui_name, cs_func, title, content, btnText1, btnText2, btnText3, callback1, callback2, callback3)
end

-- 隐藏Tip
function UIManager:CloseTip(self)
    local ui_name = UIWindowNames.UINoticeTip
    self:CloseWindow(ui_name)
end

-- 等待View层窗口创建完毕（资源加载完毕）：用于协程
function UIManager:WaitForViewCreated(ui_name)
    local window = self:GetWindow(ui_name, true)
    assert(window ~= nil, "Try to wait for a not opened window : " .. ui_name)
    if IsNull(window.View.gameObject) then
        window.View:WaitForCreated()
    end
    return window
end

-- 等待Tip响应：用于协程，返回点击序号，-1表示无响应且窗口被异常关闭
function UIManager:WaitForTipResponse()
    local ui_name = UIWindowNames.UINoticeTip
    local window = self:WaitForViewCreated(ui_name)
    return window.Model:WaitForResponse()
end

-- 析构函数
function UIManager:__delete()
    self.ui_message_center = nil
    self.windows = nil
    self.activated_windows = nil
    self.layers = nil
    self.keep_model = nil
end

--endregion

return UIManager;