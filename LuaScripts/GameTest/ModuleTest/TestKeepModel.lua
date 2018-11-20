--[[
-- added by wsh @ 2018-02-26
--]]

local function Run()
	local target = SingleGet.UIManager():GetWindow(UIWindowNames.UILogin, true, true)
	if target then
		SingleGet.SceneManager():SwitchScene(SceneConfig.HomeScene)
	end
end

return {
	Run = Run
}