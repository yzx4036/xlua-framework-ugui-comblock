---@class CfgskillseffsdatasTable
local CfgskillseffsdatasTable ={
}

---@class Cfgskillseffsdatas
local Cfgskillseffsdatas = BaseClass('Cfgskillseffsdatas')
function Cfgskillseffsdatas:__init(data)
	self.script = data.script 
	self.name = data.name 
	self.id = data.id 
	self.icon = data.icon 
	self.descr = data.descr 
	self.HP = data.HP 
	self.MP = data.MP 
	self.speed = data.speed 
	data = nil
end

----not overwrite>> the class custom ----

--可在这里写一些自定义函数

----<<not overwrite----


--->>>>--->>>>--->>>>--------我是分割线--------<<<<---<<<<---<<<<---



---@type table<number, Cfgskillseffsdatas>
local _cfgInstDict = {}


---@class CfgskillseffsdatasHelper
local CfgskillseffsdatasHelper = BaseClass('CfgskillseffsdatasHelper')

function CfgskillseffsdatasHelper:InitAll()
	if table.count(CfgskillseffsdatasTable) > 0 then
		for k, v in pairs(CfgskillseffsdatasTable) do
			_cfgInstDict[k] = Cfgskillseffsdatas.New(v)
			CfgskillseffsdatasTable[k] = nil
		end
	end
end

---@return table<number, Cfgskillseffsdatas>
function CfgskillseffsdatasHelper:GetTable()
	self:InitAll()
	return _cfgInstDict
end

---@return Cfgskillseffsdatas
function CfgskillseffsdatasHelper:GetByKey(key)
	if CfgskillseffsdatasTable[key] == nil  and _cfgInstDict[key] == nil  then
		Logger.LogError('CfgskillseffsdatasTable 配置没有key=%s对应的行!',key) return
	end
	if _cfgInstDict[key] == nil  then
		_cfgInstDict[key] = Cfgskillseffsdatas.New(CfgskillseffsdatasTable[key])
		CfgskillseffsdatasTable[key] = nil
	end
	return _cfgInstDict[key]
end

----not overwrite>> the helper custom ----

--可在这里写一些自定义函数

----<<not overwrite----
return CfgskillseffsdatasHelper