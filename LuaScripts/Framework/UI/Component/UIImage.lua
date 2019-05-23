--[[
-- added by wsh @ 2017-12-08
-- Lua侧UIImage
-- 使用方式：
-- self.xxx_img = self:AddComponent(UIImage, var_arg)--添加孩子，各种重载方式查看UIBaseContainer
--]]

---@class UIImage:UIBaseComponent
local UIImage = BaseClass("UIImage", UIBaseComponent)
local base = UIBaseComponent

-- 创建
function UIImage:OnCreate(atlas_config, original_sprite_name)
	base.OnCreate(self)
	-- Unity侧原生组件
	self.unity_uiimage = UIUtil.FindImage(self.transform)
	self.atlas_config = atlas_config

	if not IsNull(self.unity_uiimage) and
			not IsNull(self.unity_uiimage.sprite) then
		self.sprite_name = string.split(self.unity_uiimage.sprite.name, ' (')[1]
	end
	self.sprite_name = original_sprite_name or self.sprite_name
	if self.sprite_name ~= nil and type(self.sprite_name) == "string" then
		self:SetSpriteName(self.sprite_name)
	end
	if not IsNull(self.unity_uiimage) and IsNull(self.gameObject) then
		self.gameObject = self.unity_uiimage.gameObject
		self.transform = self.unity_uiimage.transform
	end
end

-- 获取Sprite名称
function UIImage:GetSpriteName()
	return self.sprite_name
end

-- 设置Sprite名称
function UIImage:SetSpriteName(sprite_name)
	self.sprite_name = sprite_name
	if IsNull(self.unity_uiimage) then
		return
	end
	
	SingleGet.AtlasManager():LoadImageAsync(self.atlas_config, sprite_name, function(sprite, sprite_name)
		-- 预设已经被销毁
		if IsNull(self.unity_uiimage) then
			return
		end
		-- 被加载的Sprite不是当前想要的Sprite：可能预设被复用，之前的加载操作就要作废
		if sprite_name ~= self.sprite_name then
			return
		end
		
		if not IsNull(sprite) then
			self.unity_uiimage.sprite = sprite
		end
	end, self.sprite_name)
end

-- 销毁
function UIImage:OnDestroy()
	self.unity_uiimage = nil
	base.OnDestroy(self)
end

return UIImage