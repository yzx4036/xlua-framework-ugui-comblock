--[[
-- added by wsh @ 2017-11-19
-- 战斗场景
-- TODO：这里只是做一个战斗场景展示Demo，大部分代码以后需要挪除
--]]

---@class BattleScene:BaseScene
local BattleScene = BaseClass("BattleScene", BaseScene)
local base = BaseScene

local CharacterAnimation = require "GameLogic.Battle.CharacterAnimation"

-- 临时：角色资源路径
local chara_res_path = "Models/1001/Character.prefab"

-- 创建：准备预加载资源
local function OnCreate(self)
	base.OnCreate(self)
	-- TODO
	-- 预加载资源
	self:AddPreloadResource(chara_res_path, typeof(CS.UnityEngine.GameObject), 1)
	self:AddPreloadResource(UIConfig[UIWindowNames.UIBattleMain].PrefabPath, typeof(CS.UnityEngine.GameObject), 1)
	
	-- 临时：角色动画控制脚本
	self.charaAnim = nil

	--self._layer = UtilityLuaCallCS.CreateTouchLayer(ConstTouchLayer.Explorer, self)

end

-- 准备工作
---@param self BattleScene
local function OnComplete(self)
	base.OnComplete(self)
	--UtilityLuaCallCS.AddTouchLayer(self._layer)

    self.cameraGo = GameObject.Find("Main Camera")
	UtilityLuaCallCS.AddCameraToEasyTouch(self.cameraGo)

	-- 创建角色
	local chara = SingleGet.GameObjectPool():GetGameObjectAsync(chara_res_path, function(inst)
		if IsNull(inst) then
			error("Load chara res err!")
			do return end
		end
		
		local chara_root = CS.UnityEngine.GameObject.Find("CharacterRoot")
		if IsNull(chara_root) then
			error("chara_root null!")
			do return end
		end
		
		inst.transform:SetParent(chara_root.transform)
		inst.transform.localPosition = Vector3.New(-7.86, 50, 5.85)
		
		SingleGet.UIManager():OpenWindow(UIWindowNames.UIBattleMain)
		
		-- 启动角色控制
		self.charaAnim = CharacterAnimation.New()
		self.charaAnim:Start(inst)
	end)
end

-- 离开场景
local function OnLeave(self)
	self.charaAnim = nil
	SingleGet.UIManager():CloseWindow(UIWindowNames.UIBattleMain)
	UtilityLuaCallCS.RemoveCameraFromEasyTouch(self.cameraGo)
	--UtilityLuaCallCS.RemoveTouchLayer(self._layer)
	base.OnLeave(self)
end

--function BattleScene:OnTap(go)
--	Logger.Log("OnTap %s %s", go.pickedObject, go.position)
--	return true
--end
----
--function BattleScene:OnTouchStart(go)
--	Logger.Log("OnTouchStart %s %s", go.pickedObject, go.position)
--	return true
--end
--
--function BattleScene:OnTouchEnd(go)
--	Logger.Log("OnTouchEnd %s %s", go.pickedObject, go.position)
--	return true
--end

BattleScene.OnCreate = OnCreate
BattleScene.OnComplete = OnComplete
BattleScene.OnLeave = OnLeave

return BattleScene;