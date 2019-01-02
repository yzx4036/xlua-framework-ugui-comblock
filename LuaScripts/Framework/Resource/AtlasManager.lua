--[[
-- added by wsh @ 2017-01-08
-- 图集管理：为逻辑层透明化图集路径和图集资源加载等底层操作
-- 注意：
-- 1、只提供异步操作，为的是不需要逻辑层取操心图集AB是否已经加载的问题
-- 2、图集管理器不做资源缓存
-- 3、图片名称带后缀
--]]

---@class AtlasManager:Singleton
local AtlasManager = BaseClass("AtlasManager", Singleton)
local sprite_type = typeof(CS.UnityEngine.Sprite)
local sprite_type1 = typeof(CS.UnityEngine.U2D.SpriteAtlas)

-- 从图集异步加载图片：回调方式
function AtlasManager:LoadImageAsync(atlas_config, image_name, callback, ...)
	local atlas_path = atlas_config.AtlasPath
	local image_path = atlas_path.."/Login.spriteatlas"--..image_name
	SingleGet.ResourcesManager():LoadAsync(image_path, sprite_type1, function(spriteAtlas, ...)
		if callback then
			local _sp = spriteAtlas:GetSprite(image_name)
			callback(not IsNull(_sp) and _sp or nil, ...)
		end
	end, ...)
end

-- 从图集异步加载图片：协程方式
function AtlasManager:CoLoadImageAsync(self, atlas_config, image_name, progress_callback)
	local sprite = SingleGet.ResourcesManager():CoLoadAsync(path, sprite_type, progress_callback)
	return not IsNull(sprite) and sprite or nil
end

return AtlasManager
