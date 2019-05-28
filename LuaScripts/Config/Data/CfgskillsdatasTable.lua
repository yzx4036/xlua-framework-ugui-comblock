---@class CfgskillsdatasTable
local CfgskillsdatasTable ={
	[1] = {script = 'SkillAttack',name = '普通攻击',id = 1,icon = 0,descr = '',HP = 0,MP = 0,speed = 0.0},
	[1000101] = {script = 'SkillAttack',name = '普通攻击',id = 1000101,icon = 0,descr = '',HP = 0,MP = 0,speed = 0.0},
	[2000101] = {script = 'SkillAttack',name = '普通攻击',id = 2000101,icon = 0,descr = '',HP = 0,MP = 0,speed = 0.0},
	[3000101] = {script = 'SkillAttack',name = '普通攻击',id = 3000101,icon = 0,descr = '',HP = 0,MP = 0,speed = 0.0},
	[4000101] = {script = 'SkillAttack',name = '普通攻击',id = 4000101,icon = 0,descr = '',HP = 0,MP = 0,speed = 0.0},
	[5000101] = {script = 'SkillAttack',name = '普通攻击',id = 5000101,icon = 0,descr = '',HP = 0,MP = 0,speed = 0.0},
	[6000101] = {script = 'SkillAttack',name = '普通攻击',id = 6000101,icon = 0,descr = '',HP = 0,MP = 0,speed = 0.0},
	[7000101] = {script = 'SkillAttack',name = '普通攻击',id = 7000101,icon = 0,descr = '',HP = 0,MP = 0,speed = 0.0}
}

---@class Cfgskillsdatas
local Cfgskillsdatas = BaseClass('Cfgskillsdatas')
function Cfgskillsdatas:__init(data)
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



---@type table<number, Cfgskillsdatas>
local _cfgInstDict = {}


---@class CfgskillsdatasHelper
local CfgskillsdatasHelper = BaseClass('CfgskillsdatasHelper')

function CfgskillsdatasHelper:InitAll()
	if table.count(CfgskillsdatasTable) > 0 then
		for k, v in pairs(CfgskillsdatasTable) do
			_cfgInstDict[k] = Cfgskillsdatas.New(v)
			CfgskillsdatasTable[k] = nil
		end
	end
end

---@return table<number, Cfgskillsdatas>
function CfgskillsdatasHelper:GetTable()
	self:InitAll()
	return _cfgInstDict
end

---@return Cfgskillsdatas
function CfgskillsdatasHelper:GetByKey(key)
	if CfgskillsdatasTable[key] == nil  and _cfgInstDict[key] == nil  then
		Logger.LogError('CfgskillsdatasTable 配置没有key=%s对应的行!',key) return
	end
	if _cfgInstDict[key] == nil  then
		_cfgInstDict[key] = Cfgskillsdatas.New(CfgskillsdatasTable[key])
		CfgskillsdatasTable[key] = nil
	end
	return _cfgInstDict[key]
end

----not overwrite>> the helper custom ----

--可在这里写一些自定义函数

----<<not overwrite----
return CfgskillsdatasHelper