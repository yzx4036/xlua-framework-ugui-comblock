---@class CfgSpaces_datasTable
local CfgSpaces_datasTable ={
	[1] = {entityType = 'Space',name = '新手村',id = 1,type = 1,spawnPos = {771.5861,211.0021,776.5501},resPath = 'spaces/xinshoucun'},
	[2] = {entityType = 'Space',name = 'kbengine_ogre_demo',id = 2,type = 1,spawnPos = {-97.9299,0.0,-158.922},resPath = 'spaces/kbengine_ogre_demo'}
}

---@class CfgSpaces_datas
local CfgSpaces_datas = BaseClass('CfgSpaces_datas')
function CfgSpaces_datas:__init(data)
	self.entityType = data[1] 
	self.name = data[2] 
	self.id = data[3] 
	self.type = data[4] 
	self.spawnPos = data[5] 
	self.resPath = data[6] 
	data = nil
end


---@type table<number, CfgSpaces_datas>
local _instList={}

function CfgSpaces_datasTable.InitAll()
	if table.length(CfgSpaces_datasTable) > 0 then
		for k, v in pairs(CfgSpaces_datasTable) do
			_instList[k] = CfgSpaces_datas.New(v)
			CfgSpaces_datasTable[k] = nil
		end
	end
end

---@return table<number, CfgSpaces_datas>
function CfgSpaces_datasTable.GetTable()
	CfgSpaces_datasTable.InitAll()
	return _instList;
end

---@return CfgSpaces_datas
function CfgSpaces_datasTable.GetByKey(key)
	if CfgSpaces_datasTable[key] == nil  and _instList[key] == nil  then
		Logger.LogError('CfgSpaces_datasTable 配置没有key=%s对应的行!',key) return
	end
	if _instList[key] == nil  then
		_instList[key] = CfgSpaces_datas.New(CfgSpaces_datasTable[key])
		CfgSpaces_datasTable[key] = nil
	end
	return _instList[key]
end

----not overwrite----

--可在这里写一些自定义函数

--not overwrite
return CfgSpaces_datasTable