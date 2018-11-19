--[[
-- added by wsh @ 2017-01-09
-- 游戏逻辑Updater，游戏逻辑模块可能需要严格的驱动顺序
--]]

---@class LogicUpdater:UpdatableSingleton
local LogicUpdater = BaseClass("LogicUpdater", UpdatableSingleton)
local traceback = debug.traceback

function LogicUpdater:Update()
	local delta_time = Time.deltaTime
	local hallConnector = SingleGet.HallConnector()
	local status,err = pcall(hallConnector.Update, hallConnector)
	if not status then
		Logger.LogError("hallConnector update err : "..err.."\n"..traceback())
	end
end

function LogicUpdater:LateUpdate()
end

function LogicUpdater:FixedUpdate()
end

function LogicUpdater:Dispose()
end

return LogicUpdater