--[[
-- added by wsh @ 2018-03-02
--]]

local function Run()
	local stack = SingleGet.UIManager().__window_stack
	print(table.dump(stack))
end

return {
	Run = Run
}