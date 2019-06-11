--[[
-- added by wsh @ 2017-12-18
-- Lua侧UISlider
-- 使用方式：
-- self.xxx_text = self:AddComponent(UISlider, var_arg)--添加孩子，各种重载方式查看UIBaseContainer
--]]

---@class UISlider:UIBaseComponent
local UISlider = BaseClass("UISlider", UIBaseComponent)
local base = UIBaseComponent

-- 创建
function UISlider:OnCreate()
	base.OnCreate(self)
	-- Unity侧原生组件
	self.unity_uislider = UIUtil.FindSlider(self.transform)
	self.__onValueChanged = nil
	if not IsNull(self.unity_uislider) and IsNull(self.gameObject) then
		self.gameObject = self.unity_uislider.gameObject
		self.transform = self.unity_uislider.transform
	end
end

-- 获取进度
function UISlider:GetValue()
	if not IsNull(self.unity_uislider) then
		return self.unity_uislider.normalizedValue
	end
end

-- 设置进度
function UISlider:SetValue(value)
	if not IsNull(self.unity_uislider) then
		self.unity_uislider.normalizedValue = value
	end
end

function UISlider:OnValueChanged(...)
	self.__onValueChanged = BindCallback(...)
	if not IsNull(self.unity_uislider) then
		self.unity_uislider.onValueChanged:AddListener(self.__onValueChanged)
	end
end

-- 销毁
function UISlider:OnDestroy()
	if self.__onValueChanged then
		self.unity_uislider.onValueChanged:AddListener(self.__onValueChanged)
		self.__onValueChanged = nil
	end
	self.unity_uislider = nil
	base.OnDestroy(self)
end

return UISlider