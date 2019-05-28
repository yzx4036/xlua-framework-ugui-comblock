---
--- 单例统一获取类
--- Created by Qi.
--- DateTime: 2018\7\26 0026 9:21
---

---@class SingleGet
SingleGet = {}

---@return AtlasManager
function SingleGet.AtlasManager()
    return 	AtlasManager:GetInstance()
end

---@return ClientData
function SingleGet.ClientData()
    return 	ClientData:GetInstance()
end

---@return DataManager
function SingleGet.DataManager()
    return 	DataManager:GetInstance()
end

---@return EffectManager
function SingleGet.EffectManager()
    return 	EffectManager:GetInstance()
end

---@return GameObjectPool
function SingleGet.GameObjectPool()
    return 	GameObjectPool:GetInstance()
end

---@return HallConnector
function SingleGet.HallConnector()
    return 	HallConnector:GetInstance()
end

---@return ResourcesManager
function SingleGet.ResourcesManager()
    return 	ResourcesManager:GetInstance()
end

---@return SceneManager
function SingleGet.SceneManager()
    return 	SceneManager:GetInstance()
end

---@return ServerData
function SingleGet.ServerData()
    return 	ServerData:GetInstance()
end

---@return TimerManager
function SingleGet.TimerManager()
    return 	TimerManager:GetInstance()
end

---@return UIManager
function SingleGet.UIManager()
    return 	UIManager:GetInstance()
end

---@return UpdateManager
function SingleGet.UpdateManager()
    return 	UpdateManager:GetInstance()
end

---@return LogicUpdater
function SingleGet.LogicUpdater()
    return LogicUpdater:GetInstance()
end

---@return ConfigCfgManager
function SingleGet.ConfigCfgManager()
    return ConfigCfgManager:GetInstance()
end

return SingleGet