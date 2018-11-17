--[[
-- added by wsh @ 2018-02-26
-- UIBattleMain控制层
--]]

local UIBattleMainCtrl = BaseClass("UIBattleMainCtrl", UIBaseCtrl)

local function Back(self)
	SingleGet.SceneManager():SwitchScene(SceneConfig.HomeScene)
end

UIBattleMainCtrl.Back = Back

return UIBattleMainCtrl