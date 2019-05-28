---@class CfgServerSevListTable
local CfgServerSevListTable ={
	[10001] = {id = 10001,ip = '192.168.1.182',name = '1服',hall_port = 6601,pvp_port = 6701},
	[10002] = {id = 10002,ip = '120.24.167.144',name = '2服',hall_port = 6601,pvp_port = 6701},
	[10003] = {id = 10003,ip = '192.168.1.182',name = '3服',hall_port = 6601,pvp_port = 6701},
	[10004] = {id = 10004,ip = '192.168.1.182',name = '4服',hall_port = 6601,pvp_port = 6701},
	[10005] = {id = 10005,ip = '192.168.1.182',name = '5服',hall_port = 6601,pvp_port = 6701},
	[10006] = {id = 10006,ip = '192.168.1.182',name = '6服',hall_port = 6601,pvp_port = 6701},
	[10007] = {id = 10007,ip = '192.168.1.182',name = '7服',hall_port = 6601,pvp_port = 6701},
	[10008] = {id = 10008,ip = '192.168.1.182',name = '8服',hall_port = 6601,pvp_port = 6701},
	[10009] = {id = 10009,ip = '192.168.1.182',name = '9服',hall_port = 6601,pvp_port = 6701},
	[10010] = {id = 10010,ip = '192.168.1.182',name = '10服',hall_port = 6601,pvp_port = 6701},
	[10011] = {id = 10011,ip = '192.168.1.182',name = '11服',hall_port = 6601,pvp_port = 6701},
	[10012] = {id = 10012,ip = '192.168.1.182',name = '12服',hall_port = 6601,pvp_port = 6701},
	[10013] = {id = 10013,ip = '192.168.1.182',name = '13服',hall_port = 6601,pvp_port = 6701},
	[10014] = {id = 10014,ip = '192.168.1.182',name = '14服',hall_port = 6601,pvp_port = 6701},
	[10015] = {id = 10015,ip = '192.168.1.182',name = '15服',hall_port = 6601,pvp_port = 6701},
	[10016] = {id = 10016,ip = '192.168.1.182',name = '16服',hall_port = 6601,pvp_port = 6701},
	[10017] = {id = 10017,ip = '192.168.1.182',name = '17服',hall_port = 6601,pvp_port = 6701},
	[10018] = {id = 10018,ip = '192.168.1.182',name = '18服',hall_port = 6601,pvp_port = 6701},
	[10019] = {id = 10019,ip = '192.168.1.182',name = '19服',hall_port = 6601,pvp_port = 6701},
	[10020] = {id = 10020,ip = '192.168.1.182',name = '20服',hall_port = 6601,pvp_port = 6701},
	[10021] = {id = 10021,ip = '192.168.1.182',name = '21服',hall_port = 6601,pvp_port = 6701},
	[10022] = {id = 10022,ip = '192.168.1.182',name = '22服',hall_port = 6601,pvp_port = 6701},
	[10023] = {id = 10023,ip = '192.168.1.182',name = '23服',hall_port = 6601,pvp_port = 6701},
	[10024] = {id = 10024,ip = '192.168.1.182',name = '24服',hall_port = 6601,pvp_port = 6701},
	[10025] = {id = 10025,ip = '192.168.1.182',name = '25服',hall_port = 6601,pvp_port = 6701},
	[10026] = {id = 10026,ip = '192.168.1.182',name = '26服',hall_port = 6601,pvp_port = 6701},
	[10027] = {id = 10027,ip = '192.168.1.182',name = '27服',hall_port = 6601,pvp_port = 6701},
	[10028] = {id = 10028,ip = '192.168.1.182',name = '28服',hall_port = 6601,pvp_port = 6701},
	[10029] = {id = 10029,ip = '192.168.1.182',name = '29服',hall_port = 6601,pvp_port = 6701},
	[10030] = {id = 10030,ip = '192.168.1.182',name = '30服',hall_port = 6601,pvp_port = 6701},
	[10031] = {id = 10031,ip = '192.168.1.182',name = '31服',hall_port = 6601,pvp_port = 6701},
	[10032] = {id = 10032,ip = '192.168.1.182',name = '32服',hall_port = 6601,pvp_port = 6701},
	[10033] = {id = 10033,ip = '192.168.1.182',name = '33服',hall_port = 6601,pvp_port = 6701},
	[10034] = {id = 10034,ip = '192.168.1.182',name = '34服',hall_port = 6601,pvp_port = 6701},
	[10035] = {id = 10035,ip = '192.168.1.182',name = '35服',hall_port = 6601,pvp_port = 6701}
}

---@class CfgServerSevList
local CfgServerSevList = BaseClass('CfgServerSevList')
function CfgServerSevList:__init(data)
	self.id = data.id 
	self.ip = data.ip 
	self.name = data.name 
	self.hall_port = data.hall_port 
	self.pvp_port = data.pvp_port 
	data = nil
end

----not overwrite>> the class custom ----

--可在这里写一些自定义函数

function CfgServerSevList:GetId()
	return self.id
end

----<<not overwrite----


--->>>>--->>>>--->>>>--------我是分割线--------<<<<---<<<<---<<<<---



---@type table<number, CfgServerSevList>
local _cfgInstDict = {}


---@class CfgServerSevListHelper
local CfgServerSevListHelper = BaseClass('CfgServerSevListHelper')

function CfgServerSevListHelper:InitAll()
	if table.count(CfgServerSevListTable) > 0 then
		for k, v in pairs(CfgServerSevListTable) do
			_cfgInstDict[k] = CfgServerSevList.New(v)
			CfgServerSevListTable[k] = nil
		end
	end
end

---@return table<number, CfgServerSevList>
function CfgServerSevListHelper:GetTable()
	self:InitAll()
	return _cfgInstDict
end

---@return CfgServerSevList
function CfgServerSevListHelper:GetByKey(key)
	if CfgServerSevListTable[key] == nil  and _cfgInstDict[key] == nil  then
		Logger.LogError('CfgServerSevListTable 配置没有key=%s对应的行!',key) return
	end
	if _cfgInstDict[key] == nil  then
		_cfgInstDict[key] = CfgServerSevList.New(CfgServerSevListTable[key])
		CfgServerSevListTable[key] = nil
	end
	return _cfgInstDict[key]
end

----not overwrite>> the helper custom ----

--可在这里写一些自定义函数

----<<not overwrite----
return CfgServerSevListHelper