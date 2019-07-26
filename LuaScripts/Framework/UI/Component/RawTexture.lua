
---@class RawTexture
local RawTexture = BaseClass("RawTexture")

--自定义Texture这里管理加载与释放
---@param pTextureComp UnityEngine.UI.RawImage 显示的texture
---@param pIsAsync boolean 加载是否异步
function RawTexture:__init(pTextureComp)
    self.texture = pTextureComp
end

---@param pHeroId number
---@param isLeft boolean 是否在左边
function RawTexture:CreateHeroHorizonIcon(pHeroId, isLeft)
    return self:SetTexturePath(string.format(HeroHorizonIconResPath, pHeroId), isLeft, true)
end


---设置UITextrue的sprite 基于prefabs/icon目录下的文件加载
---@param pDirName string icon所在的目录名称
---@param pName string icon名称
---@param isMakePixel boolean 是否设置图片为原始尺寸
function RawTexture:SetTextureLevelMap(pName, isMakePixel, pRootDirName)
    pRootDirName = pRootDirName or "Main/Texture"
    local path = string.format("%s/%s/%s.png", pRootDirName, "LevelMap", pName or "")
    return self:SetTexturePath(path, isMakePixel)
end

---@param path string 全路径
---@param isLeft boolean 是否在左边
---@param isMakePixel boolean 是否设置图片为原始尺寸
function RawTexture:SetTexturePath(path, isMakePixel)
    if IsNull(self.texture) then
        LogError(">>>>Not found the UITextrue compotent<<<")
        return
    end
    if self.path == path then
        return;
    end
    self.path = path
    self:LoadAsync(isMakePixel)

    return self
end

function RawTexture:LoadAsync(isMakePixel)
    SingleGet.ResourcesManager():LoadAsync(self.path, typeof(CS.UnityEngine.Texture), function(texture)
        if not IsNull(texture) then
            if IsNull(self.texture) then
                LogError(">>>>Not found the UITextrue compotent<<<")
                return
            end
            self.texture.texture = texture
            if isMakePixel then self.texture:SetNativeSize() end
        end
    end)
end

function RawTexture:__delete()
    self.texture = nil
end

return RawTexture