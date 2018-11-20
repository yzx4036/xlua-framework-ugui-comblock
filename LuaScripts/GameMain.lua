-- 全局模块
require "Global.Global"
	
-- 定义为全局模块，整个lua程序的入口类
GameMain = {};

-- 全局初始化
local function Initilize()
	local loadingAssetbundlePath = "UI/Prefabs/View/UILoading.prefab"
	SingleGet.ResourcesManager():CoLoadAssetBundleAsync(loadingAssetbundlePath)
end

-- 进入游戏
local function EnterGame()
	-- TODO：服务器信息应该从服务器上拉取，这里读取测试数据
	local ServerData = require "DataCenter.ServerData.ServerData"
	local TestServerData = require "GameTest.DataTest.TestServerData"
	local ClientData = require "DataCenter.ClientData.ClientData"
	SingleGet.ServerData():ParseServerList(TestServerData)
	local selected = SingleGet.ClientData().login_server_id
	if selected == nil or SingleGet.ServerData().servers[selected] == nil then
		SingleGet.ClientData():SetLoginServerID(10001)
	end
	
	SingleGet.SceneManager():SwitchScene(SceneConfig.LoginScene)
	
	--Logger.Log("###################################################")
end

--主入口函数。从这里开始lua逻辑
local function Start()
	print("GameMain start...")
	
	-- 模块启动
	SingleGet.UpdateManager():Startup()
	SingleGet.TimerManager():Startup()
	SingleGet.LogicUpdater():Startup()
	SingleGet.UIManager():Startup()
	
	if Config.Debug then
		-- 单元测试
		local UnitTest = require "UnitTest.UnitTestMain"
		UnitTest.Run()
	end
	
	coroutine.start(function()
		Initilize()
		EnterGame()
	end)
	--local _wind = require("AlertWin").New()
	--_wind:Show()

end

-- 场景切换通知
local function OnLevelWasLoaded(level)
	collectgarbage("collect")
	Time.timeSinceLevelLoad = 0
end

local function OnApplicationQuit()
	-- 模块注销
	SingleGet.UpdateManager():Dispose()
	SingleGet.TimerManager():Dispose()
	SingleGet.LogicUpdater():Dispose()
end

-- GameMain公共接口，其它的一律为私有接口，只能在本模块访问
-- GameMain公共接口特殊， C#侧的LuaManager调用Lua， 开始或结束Lua端GameMain的生命周期
GameMain.Start = Start
GameMain.OnLevelWasLoaded = OnLevelWasLoaded
GameMain.OnApplicationQuit = OnApplicationQuit

return GameMain