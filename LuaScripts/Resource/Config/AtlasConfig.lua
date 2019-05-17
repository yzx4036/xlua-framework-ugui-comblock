--[[
-- added by wsh @ 2018-01-08
-- 图集配置
--]]

---@class AtlasConfig
local AtlasConfig = {
	Comm = {
		Name = "Comm",
		AtlasPath = "UI/Atlas/Comm.spriteatlas",
	},
	Group = {
		Name = "Group",
		PackagePath = "UI/Atlas/Group.spriteatlas",
	},
	Hyper = {
		Name = "Hyper",
		AtlasPath = "UI/Atlas/Hyper.spriteatlas",
	},
	Login = {
		Name = "Login",
		AtlasPath = "UI/Atlas/Login.spriteatlas",
	},
	Role = {
		Name = "Role",
		AtlasPath = "UI/Atlas/Role.spriteatlas",
	},
}

return ConstClass("AtlasConfig", AtlasConfig)