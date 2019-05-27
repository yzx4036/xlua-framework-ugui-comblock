---@class Cfgavatar_init_datasTable
local Cfgavatar_init_datasTable ={
	[1] = {role = 1,race = 1,sex = 1,modelID = 90000001,modelScale = 1,spaceUType = 1,spawnPos = {771.5861,211.0021,776.5501},spawnYaw = 0,money = 0,level = 0,moveSpeed = 60,hp_max = 158,hp = 158,mp_max = 97,mp = 97,anger = 0,anger_max = 150,energy = 10,energy_max = 50,constitution = 13,intellect = 5,strength = 15,stamina = 12,dexterity = 10,damage = 48,magic_damage = 15,magic_defense = 15,hitval = 55,defense = 19,speed = 11,dodge = 20,potential = 0,exp = 0},
	[2] = {role = 2,race = 2,sex = 1,modelID = 90000001,modelScale = 1,spaceUType = 1,spawnPos = {771.5861,211.0021,776.5501},spawnYaw = 0,money = 0,level = 0,moveSpeed = 60,hp_max = 158,hp = 158,mp_max = 97,mp = 97,anger = 0,anger_max = 150,energy = 10,energy_max = 50,constitution = 13,intellect = 5,strength = 15,stamina = 12,dexterity = 10,damage = 48,magic_damage = 15,magic_defense = 15,hitval = 55,defense = 19,speed = 11,dodge = 20,potential = 0,exp = 0},
	[3] = {role = 3,race = 3,sex = 1,modelID = 90000001,modelScale = 1,spaceUType = 1,spawnPos = {771.5861,211.0021,776.5501},spawnYaw = 0,money = 0,level = 0,moveSpeed = 60,hp_max = 158,hp = 158,mp_max = 97,mp = 97,anger = 0,anger_max = 150,energy = 10,energy_max = 50,constitution = 13,intellect = 5,strength = 15,stamina = 12,dexterity = 10,damage = 48,magic_damage = 15,magic_defense = 15,hitval = 55,defense = 19,speed = 11,dodge = 20,potential = 0,exp = 0},
	[4] = {role = 4,race = 3,sex = 1,modelID = 90000001,modelScale = 1,spaceUType = 1,spawnPos = {771.5861,211.0021,776.5501},spawnYaw = 0,money = 0,level = 0,moveSpeed = 60,hp_max = 158,hp = 158,mp_max = 97,mp = 97,anger = 0,anger_max = 150,energy = 10,energy_max = 50,constitution = 13,intellect = 5,strength = 15,stamina = 12,dexterity = 10,damage = 48,magic_damage = 15,magic_defense = 15,hitval = 55,defense = 19,speed = 11,dodge = 20,potential = 0,exp = 0},
	[5] = {role = 5,race = 3,sex = 1,modelID = 90000001,modelScale = 1,spaceUType = 1,spawnPos = {771.5861,211.0021,776.5501},spawnYaw = 0,money = 0,level = 0,moveSpeed = 60,hp_max = 158,hp = 158,mp_max = 97,mp = 97,anger = 0,anger_max = 150,energy = 10,energy_max = 50,constitution = 13,intellect = 5,strength = 15,stamina = 12,dexterity = 10,damage = 48,magic_damage = 15,magic_defense = 15,hitval = 55,defense = 19,speed = 11,dodge = 20,potential = 0,exp = 0},
	[6] = {role = 6,race = 3,sex = 1,modelID = 90000001,modelScale = 1,spaceUType = 1,spawnPos = {771.5861,211.0021,776.5501},spawnYaw = 0,money = 0,level = 0,moveSpeed = 60,hp_max = 158,hp = 158,mp_max = 97,mp = 97,anger = 0,anger_max = 150,energy = 10,energy_max = 50,constitution = 13,intellect = 5,strength = 15,stamina = 12,dexterity = 10,damage = 48,magic_damage = 15,magic_defense = 15,hitval = 55,defense = 19,speed = 11,dodge = 20,potential = 0,exp = 0}
}

---@class Cfgavatar_init_datas
local Cfgavatar_init_datas = BaseClass('Cfgavatar_init_datas')
function Cfgavatar_init_datas:__init(data)
	self.role = data[1] 
	self.race = data[2] 
	self.sex = data[3] 
	self.modelID = data[4] 
	self.modelScale = data[5] 
	self.spaceUType = data[6] 
	self.spawnPos = data[7] 
	self.spawnYaw = data[8] 
	self.money = data[9] 
	self.level = data[10] 
	self.moveSpeed = data[11] 
	self.hp_max = data[12] 
	self.hp = data[13] 
	self.mp_max = data[14] 
	self.mp = data[15] 
	self.anger = data[16] 
	self.anger_max = data[17] 
	self.energy = data[18] 
	self.energy_max = data[19] 
	self.constitution = data[20] 
	self.intellect = data[21] 
	self.strength = data[22] 
	self.stamina = data[23] 
	self.dexterity = data[24] 
	self.damage = data[25] 
	self.magic_damage = data[26] 
	self.magic_defense = data[27] 
	self.hitval = data[28] 
	self.defense = data[29] 
	self.speed = data[30] 
	self.dodge = data[31] 
	self.potential = data[32] 
	self.exp = data[33] 
	data = nil
end


---@type table<number, Cfgavatar_init_datas>
local _instList={}

function Cfgavatar_init_datasTable.InitAll()
	if table.length(Cfgavatar_init_datasTable) > 0 then
		for k, v in pairs(Cfgavatar_init_datasTable) do
			_instList[k] = Cfgavatar_init_datas.New(v)
			Cfgavatar_init_datasTable[k] = nil
		end
	end
end

---@return table<number, Cfgavatar_init_datas>
function Cfgavatar_init_datasTable.GetTable()
	Cfgavatar_init_datasTable.InitAll()
	return _instList;
end

---@return Cfgavatar_init_datas
function Cfgavatar_init_datasTable.GetByKey(key)
	if Cfgavatar_init_datasTable[key] == nil  and _instList[key] == nil  then
		Logger.LogError('Cfgavatar_init_datasTable 配置没有key=%s对应的行!',key) return
	end
	if _instList[key] == nil  then
		_instList[key] = Cfgavatar_init_datas.New(Cfgavatar_init_datasTable[key])
		Cfgavatar_init_datasTable[key] = nil
	end
	return _instList[key]
end

----not overwrite----

--可在这里写一些自定义函数

--not overwrite
return Cfgavatar_init_datasTable