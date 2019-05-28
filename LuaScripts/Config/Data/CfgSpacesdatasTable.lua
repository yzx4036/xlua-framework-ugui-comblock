---@class CfgSpacesdatasTable
local CfgSpacesdatasTable ={
	[1] = {entityType = 'Space',name = '新手村',id = 1,type = 1,spawnPos = {771.5861,211.0021,776.5501},resPath = 'spaces/xinshoucun'},
	[2] = {entityType = 'Space',name = 'kbengine_ogre_demo',id = 2,type = 1,spawnPos = {-97.9299,0.0,-158.922},resPath = 'spaces/kbengine_ogre_demo'}
}

---@class CfgSpacesdatas
local CfgSpacesdatas = BaseClass('CfgSpacesdatas')
function CfgSpacesdatas:__init(data)
	self.entityType = data.entityType 
	self.name = data.name 
	self.id = data.id 
	self.type = data.type 
	self.spawnPos = data.spawnPos 
	self.resPath = data.resPath 
	data = nil
end

----not overwrite>> the class custom ----

--可在这里写一些自定义函数

----<<not overwrite----


--->>>>--->>>>--->>>>--------我是分割线--------<<<<---<<<<---<<<<---



---@type table<number, CfgSpacesdatas>
local _cfgInstDict = {}


---@class CfgSpacesdatasHelper
local CfgSpacesdatasHelper = BaseClass('CfgSpacesdatasHelper')

function CfgSpacesdatasHelper:InitAll()
	if table.count(CfgSpacesdatasTable) > 0 then
		for k, v in pairs(CfgSpacesdatasTable) do
			_cfgInstDict[k] = CfgSpacesdatas.New(v)
			CfgSpacesdatasTable[k] = nil
		end
	end
end

---@return table<number, CfgSpacesdatas>
function CfgSpacesdatasHelper:GetTable()
	self:InitAll()
	return _cfgInstDict
end

---@return CfgSpacesdatas
function CfgSpacesdatasHelper:GetByKey(key)
	if CfgSpacesdatasTable[key] == nil  and _cfgInstDict[key] == nil  then
		Logger.LogError('CfgSpacesdatasTable 配置没有key=%s对应的行!',key) return
	end
	if _cfgInstDict[key] == nil  then
		_cfgInstDict[key] = CfgSpacesdatas.New(CfgSpacesdatasTable[key])
		CfgSpacesdatasTable[key] = nil
	end
	return _cfgInstDict[key]
end

----not overwrite>> the helper custom ----

--可在这里写一些自定义函数

----<<not overwrite----
return CfgSpacesdatasHelper