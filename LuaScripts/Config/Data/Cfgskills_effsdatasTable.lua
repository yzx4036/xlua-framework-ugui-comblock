---@class Cfgskills_effsdatasTable
local Cfgskills_effsdatasTable ={
}

---@class Cfgskills_effsdatas
local Cfgskills_effsdatas = BaseClass('Cfgskills_effsdatas')
function Cfgskills_effsdatas:__init(data)
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


---@type table<number, Cfgskills_effsdatas>
local _instList={}

function Cfgskills_effsdatasTable.InitAll()
	if table.length(Cfgskills_effsdatasTable) > 0 then
		for k, v in pairs(Cfgskills_effsdatasTable) do
			_instList[k] = Cfgskills_effsdatas.New(v)
			Cfgskills_effsdatasTable[k] = nil
		end
	end
end

---@return table<number, Cfgskills_effsdatas>
function Cfgskills_effsdatasTable.GetTable()
	Cfgskills_effsdatasTable.InitAll()
	return _instList;
end

---@return Cfgskills_effsdatas
function Cfgskills_effsdatasTable.GetByKey(key)
	if Cfgskills_effsdatasTable[key] == nil  and _instList[key] == nil  then
		Logger.LogError('Cfgskills_effsdatasTable 配置没有key=%s对应的行!',key) return
	end
	if _instList[key] == nil  then
		_instList[key] = Cfgskills_effsdatas.New(Cfgskills_effsdatasTable[key])
		Cfgskills_effsdatasTable[key] = nil
	end
	return _instList[key]
end

----not overwrite----

--可在这里写一些自定义函数

--not overwrite
return Cfgskills_effsdatasTable