---@class Cfgentities_datasTable
local Cfgentities_datasTable ={
	[10001001] = {entityType = 'NPC',id = 10001001,etype = 1,name = '新手接待员',modelID = 10001001,moveSpeed = 50,runSpeed = 65,dialogID = 10001001},
	[10002001] = {entityType = 'NPC',id = 10002001,etype = 1,name = '传送员',modelID = 10001001,moveSpeed = 50,runSpeed = 65,dialogID = 10001001},
	[10003001] = {entityType = 'Monster',id = 10003001,etype = 1,name = '怪物1',modelID = 10001001,moveSpeed = 50,runSpeed = 65,dialogID = 0},
	[10004001] = {entityType = 'Monster',id = 10004001,etype = 1,name = '怪物2',modelID = 10004001,moveSpeed = 50,runSpeed = 65,dialogID = 0},
	[20001001] = {entityType = 'Monster',id = 20001001,etype = 1,name = '艾克斯球',modelID = 20001001,moveSpeed = 50,runSpeed = 65,dialogID = 0},
	[20002001] = {entityType = 'Monster',id = 20002001,etype = 1,name = '压力山大巨龙',modelID = 20002001,moveSpeed = 50,runSpeed = 65,dialogID = 0},
	[20003001] = {entityType = 'Monster',id = 20003001,etype = 1,name = '怪物3',modelID = 20001001,moveSpeed = 50,runSpeed = 65,dialogID = 0},
	[40001001] = {entityType = 'Gate',id = 40001001,etype = 1,name = '传送门',modelID = 40001001,moveSpeed = 0,runSpeed = 0,dialogID = 0},
	[40001002] = {entityType = 'Gate',id = 40001002,etype = 1,name = '传送门(teleport-back)',modelID = 40001001,moveSpeed = 0,runSpeed = 0,dialogID = 0},
	[40001003] = {entityType = 'Gate',id = 40001003,etype = 1,name = '传送门(teleport-local)',modelID = 40001001,moveSpeed = 0,runSpeed = 0,dialogID = 0},
	[80001001] = {entityType = 'Monster',id = 80001001,etype = 1,name = '怪物1',modelID = 80001001,moveSpeed = 30,runSpeed = 60,dialogID = 0},
	[80002001] = {entityType = 'Monster',id = 80002001,etype = 1,name = '怪物2',modelID = 80002001,moveSpeed = 30,runSpeed = 60,dialogID = 0},
	[80003001] = {entityType = 'Monster',id = 80003001,etype = 1,name = '怪物3',modelID = 80003001,moveSpeed = 30,runSpeed = 60,dialogID = 0},
	[80004001] = {entityType = 'Monster',id = 80004001,etype = 1,name = '怪物4',modelID = 80004001,moveSpeed = 30,runSpeed = 60,dialogID = 0},
	[80005001] = {entityType = 'Monster',id = 80005001,etype = 1,name = '怪物5',modelID = 80005001,moveSpeed = 30,runSpeed = 60,dialogID = 0},
	[80006001] = {entityType = 'Monster',id = 80006001,etype = 1,name = '怪物6',modelID = 80006001,moveSpeed = 30,runSpeed = 60,dialogID = 0},
	[80007001] = {entityType = 'Monster',id = 80007001,etype = 1,name = '怪物7',modelID = 80007001,moveSpeed = 30,runSpeed = 60,dialogID = 0},
	[80008001] = {entityType = 'Monster',id = 80008001,etype = 1,name = '怪物8',modelID = 80008001,moveSpeed = 30,runSpeed = 60,dialogID = 0},
	[80009001] = {entityType = 'Monster',id = 80009001,etype = 1,name = '怪物9',modelID = 80009001,moveSpeed = 30,runSpeed = 60,dialogID = 0},
	[80010001] = {entityType = 'Monster',id = 80010001,etype = 1,name = '怪物10',modelID = 80010001,moveSpeed = 30,runSpeed = 60,dialogID = 0},
	[80011001] = {entityType = 'Monster',id = 80011001,etype = 1,name = '怪物11',modelID = 80011001,moveSpeed = 30,runSpeed = 60,dialogID = 0},
	[80012001] = {entityType = 'Monster',id = 80012001,etype = 1,name = '怪物12',modelID = 80012001,moveSpeed = 30,runSpeed = 60,dialogID = 0},
	[80013001] = {entityType = 'Monster',id = 80013001,etype = 1,name = '怪物13',modelID = 80013001,moveSpeed = 30,runSpeed = 60,dialogID = 0},
	[80014001] = {entityType = 'Monster',id = 80014001,etype = 1,name = '怪物14',modelID = 80014001,moveSpeed = 30,runSpeed = 60,dialogID = 0},
	[1001] = {entityType = 'NPC',id = 1001,etype = 1,name = '新手接待员',modelID = 1001,moveSpeed = 50,runSpeed = 65,dialogID = 10001001},
	[1002] = {entityType = 'NPC',id = 1002,etype = 1,name = '传送员',modelID = 1002,moveSpeed = 50,runSpeed = 65,dialogID = 10001001},
	[1003] = {entityType = 'Monster',id = 1003,etype = 1,name = '怪物1',modelID = 1003,moveSpeed = 50,runSpeed = 65,dialogID = 0},
	[1004] = {entityType = 'Monster',id = 1004,etype = 1,name = '怪物2',modelID = 1004,moveSpeed = 50,runSpeed = 65,dialogID = 0},
	[2001] = {entityType = 'Monster',id = 2001,etype = 1,name = '怪物1',modelID = 2001,moveSpeed = 50,runSpeed = 65,dialogID = 0},
	[2002] = {entityType = 'Monster',id = 2002,etype = 1,name = '怪物2',modelID = 2002,moveSpeed = 50,runSpeed = 65,dialogID = 0},
	[2003] = {entityType = 'Monster',id = 2003,etype = 1,name = '怪物3',modelID = 2003,moveSpeed = 50,runSpeed = 65,dialogID = 0}
}

---@class Cfgentities_datas
local Cfgentities_datas = BaseClass('Cfgentities_datas')
function Cfgentities_datas:__init(data)
	self.entityType = data[1] 
	self.id = data[2] 
	self.etype = data[3] 
	self.name = data[4] 
	self.modelID = data[5] 
	self.moveSpeed = data[6] 
	self.runSpeed = data[7] 
	self.dialogID = data[8] 
	data = nil
end


---@type table<number, Cfgentities_datas>
local _instList={}

function Cfgentities_datasTable.InitAll()
	if table.length(Cfgentities_datasTable) > 0 then
		for k, v in pairs(Cfgentities_datasTable) do
			_instList[k] = Cfgentities_datas.New(v)
			Cfgentities_datasTable[k] = nil
		end
	end
end

---@return table<number, Cfgentities_datas>
function Cfgentities_datasTable.GetTable()
	Cfgentities_datasTable.InitAll()
	return _instList;
end

---@return Cfgentities_datas
function Cfgentities_datasTable.GetByKey(key)
	if Cfgentities_datasTable[key] == nil  and _instList[key] == nil  then
		Logger.LogError('Cfgentities_datasTable 配置没有key=%s对应的行!',key) return
	end
	if _instList[key] == nil  then
		_instList[key] = Cfgentities_datas.New(Cfgentities_datasTable[key])
		Cfgentities_datasTable[key] = nil
	end
	return _instList[key]
end

----not overwrite----

--可在这里写一些自定义函数

--not overwrite
return Cfgentities_datasTable