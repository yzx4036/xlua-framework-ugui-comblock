---@class CfgServerAreaLangSevAreaTable
local CfgServerAreaLangSevAreaTable ={
	[10001] = {id = 10001,name = '一区',desc = '测试用'},
	[10002] = {id = 10002,name = '二区',desc = '测试用'},
	[10003] = {id = 10003,name = '三区',desc = '测试用'},
	[10004] = {id = 10004,name = '四区',desc = '测试用'},
	[10005] = {id = 10005,name = '五区',desc = '测试用'}
}

---@class CfgServerAreaLangSevArea
local CfgServerAreaLangSevArea = BaseClass('CfgServerAreaLangSevArea')
function CfgServerAreaLangSevArea:__init(data)
	self.id = data.id 
	self.name = data.name 
	self.desc = data.desc 
	data = nil
end

----not overwrite>> the class custom ----

--可在这里写一些自定义函数

----<<not overwrite----


--->>>>--->>>>--->>>>--------我是分割线--------<<<<---<<<<---<<<<---



---@type table<number, CfgServerAreaLangSevArea>
local _cfgInstDict = {}


---@class CfgServerAreaLangSevAreaHelper
local CfgServerAreaLangSevAreaHelper = BaseClass('CfgServerAreaLangSevAreaHelper')

function CfgServerAreaLangSevAreaHelper:InitAll()
	if table.count(CfgServerAreaLangSevAreaTable) > 0 then
		for k, v in pairs(CfgServerAreaLangSevAreaTable) do
			_cfgInstDict[k] = CfgServerAreaLangSevArea.New(v)
			CfgServerAreaLangSevAreaTable[k] = nil
		end
	end
end

---@return table<number, CfgServerAreaLangSevArea>
function CfgServerAreaLangSevAreaHelper:GetTable()
	self:InitAll()
	return _cfgInstDict
end

---@return CfgServerAreaLangSevArea
function CfgServerAreaLangSevAreaHelper:GetByKey(key)
	if CfgServerAreaLangSevAreaTable[key] == nil  and _cfgInstDict[key] == nil  then
		Logger.LogError('CfgServerAreaLangSevAreaTable 配置没有key=%s对应的行!',key) return
	end
	if _cfgInstDict[key] == nil  then
		_cfgInstDict[key] = CfgServerAreaLangSevArea.New(CfgServerAreaLangSevAreaTable[key])
		CfgServerAreaLangSevAreaTable[key] = nil
	end
	return _cfgInstDict[key]
end

----not overwrite>> the helper custom ----

--可在这里写一些自定义函数

----<<not overwrite----
return CfgServerAreaLangSevAreaHelper