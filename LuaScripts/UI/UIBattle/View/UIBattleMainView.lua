--[[
-- added by wsh @ 2018-02-26
-- UIBattleMain视图层
--]]
---@class UIBattleMainView:UIBaseView
local UIBattleMainView = BaseClass("UIBattleMainView", UIBaseView)
local base = UIBaseView

-- 各个组件路径
local back_btn_path = "BackBtn"

function UIBattleMainView:OnCreate()
	base.OnCreate(self)
	
	-- 控制角色
	self.chara = nil
	-- 退出按钮
	self.back_btn = self:AddComponent(UIButton, back_btn_path)
	self.back_btn:SetOnClick(function()
		self.ctrl:Back()
	end)
end

function UIBattleMainView:OnEnable()
	base.OnEnable(self)
end

function UIBattleMainView:LateUpdate()
	if IsNull(self.chara) then
		self.chara = CS.UnityEngine.GameObject.FindGameObjectWithTag("Player")
	end
	
	if IsNull(self.chara) then
		return
	end
	
	local axisXValue = CS.ETCInput.GetAxis("Horizontal")
	local axisYValue = CS.ETCInput.GetAxis("Vertical")
	if Time.frameCount % 30 == 0 then
		print("ETCInput : "..axisXValue..", "..axisYValue)
	end
	
	-- 说明：这里根据获取的摇杆输入向量控制角色移动
	-- 示例代码略
end

function UIBattleMainView:OnDestroy()
	base.OnDestroy(self)
end

return UIBattleMainView