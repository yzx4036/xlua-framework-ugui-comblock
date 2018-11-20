--[[
-- added by wsh @ 2017-11-30
-- Logger系统：Lua中所有错误日志输出均使用本脚本接口，以便上报服务器
--]]

local Logger = BaseClass("Logger")

local function Log(msg, ...)
	local _string = string.format(msg, ...)
	if Config.Debug then
		print(debug.traceback(_string, 2))
	else
		CS.Logger.Log(debug.traceback(_string, 2))
	end
end

local function LogError(msg, ...)
	local _string = string.format(msg, ...)
	if Config.Debug then
		error(_string, 2)
	else
		CS.Logger.LogError(debug.traceback(_string, 2))
	end
end

-- 重定向event错误处理函数
event_err_handle = function(msg, ...)
	LogError(msg, ...)
end

Logger.Log = Log
Logger.LogError = LogError

return Logger