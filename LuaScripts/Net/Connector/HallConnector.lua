--[[
-- added by wsh @ 2017-01-09
-- 大厅网络连接器
--]]

---@class HallConnector:Singleton
local HallConnector = BaseClass("HallConnector", Singleton)
local SendMsgDefine = require "Net.Config.SendMsgDefine"
local NetUtil = require "Net.Util.NetUtil"

local ConnStatus = {
	Init = 0,
	Connecting = 1,
	WaitLogin = 2,
	Done = 3,
}

function HallConnector:__init()
	self.hallSocket = nil
	self.globalSeq = 0
end

local function OnReceivePackage(self, receive_bytes)
	local receive_msg = NetUtil.DeserializeMessage(receive_bytes)
	Logger.Log(tostring(receive_msg))
end

function HallConnector:Connect(host_ip, host_port, on_connect, on_close)
	if not self.hallSocket then
		self.hallSocket = CS.Networks.HjTcpNetwork()
		self.hallSocket.ReceivePkgHandle = Bind(self, OnReceivePackage)
	end
	self.hallSocket.OnConnect = on_connect
	self.hallSocket.OnClosed = on_close
	self.hallSocket:SetHostPort(host_ip, host_port)
	self.hallSocket:Connect()
	Logger.Log("Connect to "..host_ip..", port : "..host_port)
	return self.hallSocket
end

function HallConnector:SendMessage(msg_id, msg_obj, show_mask, need_resend)
	show_mask = show_mask == nil and true or show_mask
	need_resend = need_resend == nil and true or need_resend
	
	local request_seq = 0
	local send_msg = SendMsgDefine.New(msg_id, msg_obj, request_seq)
	local msg_bytes = NetUtil.SerializeMessage(send_msg, self.globalSeq)
	Logger.Log(tostring(send_msg))
	self.hallSocket:SendMessage(msg_bytes)
	self.globalSeq = self.globalSeq + 1
end

function HallConnector:Update()
	if self.hallSocket then
		self.hallSocket:UpdateNetwork()
	end
end

function HallConnector:Disconnect()
	if self.hallSocket then
		self.hallSocket:Disconnect()
	end
end

function HallConnector:Dispose()
	if self.hallSocket then
		self.hallSocket:Dispose()
	end
	self.hallSocket = nil
end

return HallConnector
