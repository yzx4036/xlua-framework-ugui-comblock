---@class Cfgavatar_initdatasTable
local Cfgavatar_initdatasTable ={
	[1] = {role = 1,race = 1,sex = 1,modelID = 90000001,modelScale = 1,spaceUType = 1,spawnPos = {771.5861,211.0021,776.5501},spawnYaw = 0,money = 0,level = 0,moveSpeed = 60,hp_max = 158,hp = 158,mp_max = 97,mp = 97,anger = 0,anger_max = 150,energy = 10,energy_max = 50,constitution = 13,intellect = 5,strength = 15,stamina = 12,dexterity = 10,damage = 48,magic_damage = 15,magic_defense = 15,hitval = 55,defense = 19,speed = 11,dodge = 20,potential = 0,exp = 0},
	[2] = {role = 2,race = 2,sex = 1,modelID = 90000001,modelScale = 1,spaceUType = 1,spawnPos = {771.5861,211.0021,776.5501},spawnYaw = 0,money = 0,level = 0,moveSpeed = 60,hp_max = 158,hp = 158,mp_max = 97,mp = 97,anger = 0,anger_max = 150,energy = 10,energy_max = 50,constitution = 13,intellect = 5,strength = 15,stamina = 12,dexterity = 10,damage = 48,magic_damage = 15,magic_defense = 15,hitval = 55,defense = 19,speed = 11,dodge = 20,potential = 0,exp = 0},
	[3] = {role = 3,race = 3,sex = 1,modelID = 90000001,modelScale = 1,spaceUType = 1,spawnPos = {771.5861,211.0021,776.5501},spawnYaw = 0,money = 0,level = 0,moveSpeed = 60,hp_max = 158,hp = 158,mp_max = 97,mp = 97,anger = 0,anger_max = 150,energy = 10,energy_max = 50,constitution = 13,intellect = 5,strength = 15,stamina = 12,dexterity = 10,damage = 48,magic_damage = 15,magic_defense = 15,hitval = 55,defense = 19,speed = 11,dodge = 20,potential = 0,exp = 0},
	[4] = {role = 4,race = 3,sex = 1,modelID = 90000001,modelScale = 1,spaceUType = 1,spawnPos = {771.5861,211.0021,776.5501},spawnYaw = 0,money = 0,level = 0,moveSpeed = 60,hp_max = 158,hp = 158,mp_max = 97,mp = 97,anger = 0,anger_max = 150,energy = 10,energy_max = 50,constitution = 13,intellect = 5,strength = 15,stamina = 12,dexterity = 10,damage = 48,magic_damage = 15,magic_defense = 15,hitval = 55,defense = 19,speed = 11,dodge = 20,potential = 0,exp = 0},
	[5] = {role = 5,race = 3,sex = 1,modelID = 90000001,modelScale = 1,spaceUType = 1,spawnPos = {771.5861,211.0021,776.5501},spawnYaw = 0,money = 0,level = 0,moveSpeed = 60,hp_max = 158,hp = 158,mp_max = 97,mp = 97,anger = 0,anger_max = 150,energy = 10,energy_max = 50,constitution = 13,intellect = 5,strength = 15,stamina = 12,dexterity = 10,damage = 48,magic_damage = 15,magic_defense = 15,hitval = 55,defense = 19,speed = 11,dodge = 20,potential = 0,exp = 0},
	[6] = {role = 6,race = 3,sex = 1,modelID = 90000001,modelScale = 1,spaceUType = 1,spawnPos = {771.5861,211.0021,776.5501},spawnYaw = 0,money = 0,level = 0,moveSpeed = 60,hp_max = 158,hp = 158,mp_max = 97,mp = 97,anger = 0,anger_max = 150,energy = 10,energy_max = 50,constitution = 13,intellect = 5,strength = 15,stamina = 12,dexterity = 10,damage = 48,magic_damage = 15,magic_defense = 15,hitval = 55,defense = 19,speed = 11,dodge = 20,potential = 0,exp = 0}
}

---@class Cfgavatar_initdatas
local Cfgavatar_initdatas = BaseClass('Cfgavatar_initdatas')
function Cfgavatar_initdatas:__init(data)
	self.role = data.role 
	self.race = data.race 
	self.sex = data.sex 
	self.modelID = data.modelID 
	self.modelScale = data.modelScale 
	self.spaceUType = data.spaceUType 
	self.spawnPos = data.spawnPos 
	self.spawnYaw = data.spawnYaw 
	self.money = data.money 
	self.level = data.level 
	self.moveSpeed = data.moveSpeed 
	self.hp_max = data.hp_max 
	self.hp = data.hp 
	self.mp_max = data.mp_max 
	self.mp = data.mp 
	self.anger = data.anger 
	self.anger_max = data.anger_max 
	self.energy = data.energy 
	self.energy_max = data.energy_max 
	self.constitution = data.constitution 
	self.intellect = data.intellect 
	self.strength = data.strength 
	self.stamina = data.stamina 
	self.dexterity = data.dexterity 
	self.damage = data.damage 
	self.magic_damage = data.magic_damage 
	self.magic_defense = data.magic_defense 
	self.hitval = data.hitval 
	self.defense = data.defense 
	self.speed = data.speed 
	self.dodge = data.dodge 
	self.potential = data.potential 
	self.exp = data.exp 
	data = nil
end

----not overwrite>> the class custom ----

--可在这里写一些自定义函数

----<<not overwrite----


--->>>>--->>>>--->>>>--------我是分割线--------<<<<---<<<<---<<<<---



---@type table<number, Cfgavatar_initdatas>
local _cfgInstDict = {}


---@class Cfgavatar_initdatasHelper
local Cfgavatar_initdatasHelper = BaseClass('Cfgavatar_initdatasHelper')

function Cfgavatar_initdatasHelper:InitAll()
	if table.count(Cfgavatar_initdatasTable) > 0 then
		for k, v in pairs(Cfgavatar_initdatasTable) do
			_cfgInstDict[k] = Cfgavatar_initdatas.New(v)
			Cfgavatar_initdatasTable[k] = nil
		end
	end
end

---@return table<number, Cfgavatar_initdatas>
function Cfgavatar_initdatasHelper:GetTable()
	self:InitAll()
	return _cfgInstDict
end

---@return Cfgavatar_initdatas
function Cfgavatar_initdatasHelper:GetByKey(key)
	if Cfgavatar_initdatasTable[key] == nil  and _cfgInstDict[key] == nil  then
		Logger.LogError('Cfgavatar_initdatasTable 配置没有key=%s对应的行!',key) return
	end
	if _cfgInstDict[key] == nil  then
		_cfgInstDict[key] = Cfgavatar_initdatas.New(Cfgavatar_initdatasTable[key])
		Cfgavatar_initdatasTable[key] = nil
	end
	return _cfgInstDict[key]
end

----not overwrite>> the helper custom ----

--可在这里写一些自定义函数

----<<not overwrite----
return Cfgavatar_initdatasHelper