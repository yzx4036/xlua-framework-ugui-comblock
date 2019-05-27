---@class CfgServerAreaLang_SevAreaTable
local CfgServerAreaLang_SevAreaTable ={
	[10001] = {id = 10001,name = '一区',desc = '测试用'},
	[10002] = {id = 10002,name = '二区',desc = '测试用'},
	[10003] = {id = 10003,name = '三区',desc = '测试用'},
	[10004] = {id = 10004,name = '四区',desc = '测试用'},
	[10005] = {id = 10005,name = '五区',desc = '测试用'}
}

---@class CfgServerAreaLang_SevArea
local CfgServerAreaLang_SevArea = BaseClass('CfgServerAreaLang_SevArea')
function CfgServerAreaLang_SevArea:__init(data)
	self.id = data[1] 
	self.name = data[2] 
	self.desc = data[3] 
	data = nil
end


---@type table<number, CfgServerAreaLang_SevArea>
local _instList={}

function CfgServerAreaLang_SevAreaTable.InitAll()
	if table.length(CfgServerAreaLang_SevAreaTable) > 0 then
		for k, v in pairs(CfgServerAreaLang_SevAreaTable) do
			_instList[k] = CfgServerAreaLang_SevArea.New(v)
			CfgServerAreaLang_SevAreaTable[k] = nil
		end
	end
end

---@return table<number, CfgServerAreaLang_SevArea>
function CfgServerAreaLang_SevAreaTable.GetTable()
	CfgServerAreaLang_SevAreaTable.InitAll()
	return _instList;
end

---@return CfgServerAreaLang_SevArea
function CfgServerAreaLang_SevAreaTable.GetByKey(key)
	if CfgServerAreaLang_SevAreaTable[key] == nil  and _instList[key] == nil  then
		Logger.LogError('CfgServerAreaLang_SevAreaTable 配置没有key=%s对应的行!',key) return
	end
	if _instList[key] == nil  then
		_instList[key] = CfgServerAreaLang_SevArea.New(CfgServerAreaLang_SevAreaTable[key])
		CfgServerAreaLang_SevAreaTable[key] = nil
	end
	return _instList[key]
end

----not overwrite----

--可在这里写一些自定义函数

--not overwrite
return CfgServerAreaLang_SevAreaTable