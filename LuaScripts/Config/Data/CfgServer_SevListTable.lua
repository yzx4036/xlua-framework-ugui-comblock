---@class CfgServer_SevListTable
local CfgServer_SevListTable ={
	[10001] = {id = 10001,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10002] = {id = 10002,ip = '120.24.167.144',hall_port = 6601,pvp_port = 6701},
	[10003] = {id = 10003,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10004] = {id = 10004,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10005] = {id = 10005,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10006] = {id = 10006,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10007] = {id = 10007,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10008] = {id = 10008,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10009] = {id = 10009,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10010] = {id = 10010,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10011] = {id = 10011,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10012] = {id = 10012,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10013] = {id = 10013,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10014] = {id = 10014,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10015] = {id = 10015,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10016] = {id = 10016,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10017] = {id = 10017,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10018] = {id = 10018,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10019] = {id = 10019,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10020] = {id = 10020,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10021] = {id = 10021,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10022] = {id = 10022,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10023] = {id = 10023,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10024] = {id = 10024,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10025] = {id = 10025,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10026] = {id = 10026,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10027] = {id = 10027,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10028] = {id = 10028,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10029] = {id = 10029,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10030] = {id = 10030,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10031] = {id = 10031,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10032] = {id = 10032,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10033] = {id = 10033,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10034] = {id = 10034,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701},
	[10035] = {id = 10035,ip = '192.168.1.182',hall_port = 6601,pvp_port = 6701}
}

---@class CfgServer_SevList
local CfgServer_SevList = BaseClass('CfgServer_SevList')
function CfgServer_SevList:__init(data)
	self.id = data[1] 
	self.ip = data[2] 
	self.hall_port = data[3] 
	self.pvp_port = data[4] 
	data = nil
end


---@type table<number, CfgServer_SevList>
local _instList={}

function CfgServer_SevListTable.InitAll()
	if table.length(CfgServer_SevListTable) > 0 then
		for k, v in pairs(CfgServer_SevListTable) do
			_instList[k] = CfgServer_SevList.New(v)
			CfgServer_SevListTable[k] = nil
		end
	end
end

---@return table<number, CfgServer_SevList>
function CfgServer_SevListTable.GetTable()
	CfgServer_SevListTable.InitAll()
	return _instList;
end

---@return CfgServer_SevList
function CfgServer_SevListTable.GetByKey(key)
	if CfgServer_SevListTable[key] == nil  and _instList[key] == nil  then
		Logger.LogError('CfgServer_SevListTable 配置没有key=%s对应的行!',key) return
	end
	if _instList[key] == nil  then
		_instList[key] = CfgServer_SevList.New(CfgServer_SevListTable[key])
		CfgServer_SevListTable[key] = nil
	end
	return _instList[key]
end

----not overwrite----

--可在这里写一些自定义函数

--not overwrite
return CfgServer_SevListTable