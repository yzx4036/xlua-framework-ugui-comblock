---@class Cfgskills_datasTable
local Cfgskills_datasTable ={
	[1] = {script = 'SkillAttack',name = '普通攻击',id = 1,icon = 0,descr = '',HP = 0,MP = 0,speed = 0.0},
	[1000101] = {script = 'SkillAttack',name = '普通攻击',id = 1000101,icon = 0,descr = '',HP = 0,MP = 0,speed = 0.0},
	[2000101] = {script = 'SkillAttack',name = '普通攻击',id = 2000101,icon = 0,descr = '',HP = 0,MP = 0,speed = 0.0},
	[3000101] = {script = 'SkillAttack',name = '普通攻击',id = 3000101,icon = 0,descr = '',HP = 0,MP = 0,speed = 0.0},
	[4000101] = {script = 'SkillAttack',name = '普通攻击',id = 4000101,icon = 0,descr = '',HP = 0,MP = 0,speed = 0.0},
	[5000101] = {script = 'SkillAttack',name = '普通攻击',id = 5000101,icon = 0,descr = '',HP = 0,MP = 0,speed = 0.0},
	[6000101] = {script = 'SkillAttack',name = '普通攻击',id = 6000101,icon = 0,descr = '',HP = 0,MP = 0,speed = 0.0},
	[7000101] = {script = 'SkillAttack',name = '普通攻击',id = 7000101,icon = 0,descr = '',HP = 0,MP = 0,speed = 0.0}
}

---@class Cfgskills_datas
local Cfgskills_datas = BaseClass('Cfgskills_datas')
function Cfgskills_datas:__init(data)
	self.script = data[1] 
	self.name = data[2] 
	self.id = data[3] 
	self.icon = data[4] 
	self.descr = data[5] 
	self.HP = data[6] 
	self.MP = data[7] 
	self.speed = data[8] 
	data = nil
end


---@type table<number, Cfgskills_datas>
local _instList={}

function Cfgskills_datasTable.InitAll()
	if table.length(Cfgskills_datasTable) > 0 then
		for k, v in pairs(Cfgskills_datasTable) do
			_instList[k] = Cfgskills_datas.New(v)
			Cfgskills_datasTable[k] = nil
		end
	end
end

---@return table<number, Cfgskills_datas>
function Cfgskills_datasTable.GetTable()
	Cfgskills_datasTable.InitAll()
	return _instList;
end

---@return Cfgskills_datas
function Cfgskills_datasTable.GetByKey(key)
	if Cfgskills_datasTable[key] == nil  and _instList[key] == nil  then
		Logger.LogError('Cfgskills_datasTable 配置没有key=%s对应的行!',key) return
	end
	if _instList[key] == nil  then
		_instList[key] = Cfgskills_datas.New(Cfgskills_datasTable[key])
		Cfgskills_datasTable[key] = nil
	end
	return _instList[key]
end

----not overwrite----

--可在这里写一些自定义函数

--not overwrite
return Cfgskills_datasTable