--[[
-- added by wsh @ 2017-11-30
-- Lua全局配置
--]]

---@class Config
local Config = Config or {}

-- 调试模式：真机出包时关闭
Config.Debug = true
Config.DebugUnitTest = false
Config.UseAssetBundle = false

return Config