--[[
-- added by wsh @ 2017-01-12
--]]

local function Run()
	SingleGet.UIManager():OpenTwoButtonTip("标题", "测试内容1", "按钮1", "按钮2", function()
		print("点击了按钮1")
	end,function()
		print("点击了按钮2")
	end)
	coroutine.start(function()
		local index = SingleGet.UIManager():WaitForTipResponse()
		print("等待响应结束：", index)
	end)
	coroutine.start(function()
		coroutine.waitforseconds(3)
		SingleGet.UIManager():OpenThreeButtonTip("标题", "测试内容2", "按钮1", "按钮2", "按钮3", function()
			print("点击了按钮1")
		end,function()
			print("点击了按钮2")
		end,function()
			print("点击了按钮3")
		end)
	end)
end

return {
	Run = Run
}