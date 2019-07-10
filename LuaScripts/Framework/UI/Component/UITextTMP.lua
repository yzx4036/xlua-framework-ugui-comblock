--[[
-- added by wsh @ 2019-7-10
-- Lua侧UITextTMP
-- 使用方式：
-- self.xxx_text = self:AddComponent(UIInput, var_arg)--添加孩子，各种重载方式查看UIBaseContainer
-- TODO：本地化支持
--]]
---@class UITextTMP:UIBaseComponent
local UITextTMP = BaseClass("UITextTMP", UIBaseComponent)
local base = UIBaseComponent

-- 创建
---@param self UITextTMP
local function OnCreate(self)
	base.OnCreate(self)
	-- Unity侧原生组件
	---@type 
	self.unity_UITextTMP = UIUtil.FindTextTMP(self.transform)
	
	if IsNull(self.unity_UITextTMP) and not IsNull(self.gameObject) then
		self.gameObject = self.unity_UITextTMP.gameObject
		self.transform = self.unity_UITextTMP.transform
	end
end

-- 获取文本
local function GetText(self)
	if not IsNull(self.unity_UITextTMP) then
		return self.unity_UITextTMP.text
	end
end

-- 设置文本
local function SetText(self, text)
	if not IsNull(self.unity_UITextTMP) then
		self.unity_UITextTMP.text = text
	end
end

-- 销毁
local function OnDestroy(self)
	self.unity_UITextTMP = nil
	base.OnDestroy(self)
end

UITextTMP.OnCreate = OnCreate
UITextTMP.GetText = GetText
UITextTMP.SetText = SetText
UITextTMP.OnDestroy = OnDestroy

return UITextTMP