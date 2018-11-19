local BaseWindow = BaseClass("BaseWindow")

function BaseWindow:ctor(parentPage)
  self.window= nil
  self.contentPane = nil
  self.vars = {}
  self.EventDelegates = {}
  self._originPos = Vector2.zero
  self.animation = { "eject", "shrink"}
  self.asyncCreate = false
  self:Create()
end

function BaseWindow:SetContentSource(pkg, item)
  self.window:AddUISource(CS.FairyGame.UISource(pkg))
  self._packageName = pkg;
  self._itemId = item
end

function BaseWindow:SetDepends(...)
  local arg = {...}
  for i,v in ipairs(arg) do
      self.window:AddUISource(CS.FairyGame.UISource(v))
  end
end

function BaseWindow:Create()
--  self:mainCom = UIPackage.CreateObject(pkgPath, mainPath)
--  self:mainCom:SetSize(GRoot.inst:width, GRoot.inst:height)
--  self:mainCom:AddRelation(GRoot.inst, RelationType.Size)
--  GRoot.inst:AddChild(mainCom)
  self.window = LuaWindow()
  self.window:ConnectLua(self)
end

function BaseWindow:OnInit()
  --log(">>>>>BaseWindow:OnInit....")
  if not string.isNilOrEmpty(self._itemId) then
        if self.asyncCreate then
            UIPackage.CreateObjectAsync(self._packageName, self._itemId, 
                UIPackage.CreateObjectCallback(
                    function(obj)  
                        self.window.contentPane = obj
                        self.contentPane = self.window.contentPane
                        --log(">>>>>CreateObjectCallback:"..type(self.contentPane ))
                        self:OnInit2()
                        if self.window.isShowing then
                            self:DoShowAnimation()
                        end
                    end
                ))
        else
          self.window.contentPane = UIPackage.CreateObject(self._packageName, self._itemId)
          self.contentPane = self.window.contentPane
          self:OnInit2()
        end
  else
    self:OnInit2()
  end
end


function BaseWindow:OnInit2()
    self.window:SetPivot(0.5, 0.5)

    --[[local fullScreen = self.width == 1136 and self.height == 640
    if self.frame ~=nil and fullScreen then
        self.x = math.ceil((GRoot.inst.width - self.width)/2)
        if GRoot.inst.width < self.width then
            if self.closeButton ~= nil then
                self.closeButton.x = math.floor((GRoot.inst.width + self.width) /2- self.closeButton.width)
            end
        else
            self.frame.width = GRoot.inst.width
            self.frame.x = -self.x
        end
        self.frame.height = GRoot.inst.height
    elseif fullScreen then
        self:SetSize(GRoot.inst.width, GRoot.inst.height)
    end
]]
    self:OnInitWidget()
end

function BaseWindow:ReadyToUpdate()
end

function BaseWindow:DoShowAnimation()
    if self.contentPane==nil then return end

    self:ReadyToUpdate()
    
  self._originPos = self.window.xy

  local ani = self.animation[1]
  if ani=="eject" then
        self.window:SetScale(0.9, 0.9)
        local tween = self.window:TweenScale(Vector2(1, 1), 0.3)
        TweenUtils.SetEase(tween, Ease.OutBack)
        TweenUtils.OnComplete(tween, BaseWindow.CallOnShown, self)
    elseif ani=="fade_in" then
        self.alpha = 0
        local tween = self.window:TweenFade(1, 0.3)
        TweenUtils.SetEase(tween, Ease.OutQuad)
        TweenUtils.OnComplete(tween, BaseWindow.CallOnShown, self)
    elseif ani=="move_up" then
        self.y = GRoot.inst.height
        local tween = self.window:TweenMoveY(self._originPos.y, 0.3)
        TweenUtils.SetEase(tween, Ease.OutQuad)
        TweenUtils.OnComplete(tween, BaseWindow.CallOnShown, self)
    elseif ani=="move_left" then
        self.x = CS.FairyGUI.GRoot.inst.width
        local tween = self.window:TweenMoveX(self._originPos.x, 0.3)
        TweenUtils.SetEase(tween, Ease.OutQuad)
        TweenUtils.OnComplete(tween, BaseWindow.CallOnShown, self)
    elseif ani=="move_right" then
        self.window.x = -self.window.width-30
        local tween = self.window:TweenMoveX(self._originPos.x, 0.3)
        TweenUtils.SetEase(tween, Ease.OutQuad)
        TweenUtils.OnComplete(tween, BaseWindow.CallOnShown, self)
    else
        self:CallOnShown()
    end
end

function BaseWindow:DoHideAnimation()
  self._originPos = self.xy
  local ani = self.animation[2]
  if ani=="shrink" then
        self:SetScale(1, 1);
        local tween = self.window:TweenScale(Vector2(0.8, 0.8), 0.2)
        TweenUtils.SetEase(tween, Ease.InExpo)
        TweenUtils.OnComplete(tween, BaseWindow.DoHide, self)
    elseif ani=="move_down" then
        local tween = self.window:TweenMoveY(GRoot.inst.height + 30, 0.3)
        TweenUtils.SetEase(tween, Ease.OutQuad)
        TweenUtils.OnComplete(tween, BaseWindow.DoHide, self)
     elseif ani=="move_left" then
        local tween = self.window:TweenMoveX(-self.window.width-30, 0.3)
        TweenUtils.SetEase(tween, Ease.OutQuad)
        TweenUtils.OnComplete(tween, BaseWindow.DoHide, self)
    elseif ani=="move_right" then
        local tween = self.window:TweenMoveX(GRoot.inst.width + 30, 0.3)
        TweenUtils.SetEase(tween, Ease.OutQuad)
        TweenUtils.OnComplete(tween, BaseWindow.DoHide, self)
    else
        self:DoHide()
    end
end

function BaseWindow:CallOnShown()
  --log("BaseWindow:CallOnShown")
  self.window:SetScale(1, 1)
  self.window.alpha = 1
  self.window.xy = self._originPos
  
  self:OnShown()
end

function BaseWindow:DoHide()
    self.window:SetScale(1, 1)
    self.window.alpha = 1
    self.window.xy = self._originPos
    self.window:HideImmediately()
    self:OnHide()    
end


function BaseWindow:Show()
  if not self.contentPane then
    self:Create()
  end
  if not self.window.isShowing then
    self.window:Show()
    self:OnShown()
  end
end

function BaseWindow:Hide()
  if self.window.isShowing then
    self:DoHide()
  else
    self:Destroy()
  end
end

function BaseWindow:Destroy()
  self.mainCom:Dispose();
  self.mainCom = nil;
  self:OnRealseWidget();
end

--窗口控制初始化 这个相当于显示对象第一次生成


--窗口控制初始化 这个相当于显示对象第一次生成
-- function BaseWindow:OnInitWidget()
-- end

-- --将要显示
-- function BaseWindow:OnWillApprear()
-- end

-- --真正显示
-- function BaseWindow:OnDidAppear()
-- end

-- --将要隐藏
-- function BaseWindow:OnWillDisappear()
-- end

-- --真正关闭
-- function BaseWindow:OnDidDisappear()
-- end

-- function BaseWindow:OnRealseWidget()
-- end

-- function BaseWindow:OnWillApprear()
-- end

function BaseWindow:OnInitWidget()
end

--将要隐藏
function BaseWindow:DoHideAnimation()
end

--真正关闭
function BaseWindow:DoShowAnimation()
end

function BaseWindow:OnShown()
end

function BaseWindow:OnHide()
end

return BaseWindow



-- OnInit
-- DoHideAnimation
-- DoShowAnimation
-- OnShown
-- OnHide