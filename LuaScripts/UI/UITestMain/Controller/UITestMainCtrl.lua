--[[
-- added by wsh @ 2018-02-26
-- UITestMain控制层
--]]

local UITestMainCtrl = BaseClass("UITestMainCtrl", UIBaseCtrl)

local function StartFighting(self)
	SingleGet.SceneManager():SwitchScene(SceneConfig.BattleScene, 45)
end

local function Logout(self)
	SingleGet.SceneManager():SwitchScene(SceneConfig.LoginScene)
end

UITestMainCtrl.StartFighting = StartFighting
UITestMainCtrl.Logout = Logout

return UITestMainCtrl