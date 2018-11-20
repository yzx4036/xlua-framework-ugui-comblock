---@class AlertWin:BaseWindow
local AlertWin = BaseClass("AlertWin", BaseWindow)
function AlertWin:__OnInit()
    Logger.Log(">>>AlertWin:__init %s",self.vars)

    self:SetContentSource("Common", "AlertWindow")
    self.asyncCreate = false
    self.window:Init()
end

function AlertWin:InternalOpen(msg, callback)
    if msg==nil then msg = "" end
    self.contentPane:GetChild("msg").text = msg
    self.vars.callback = callback
    self:Show()
end

function AlertWin:OnInitWidget()
	self.contentPane:GetChild("ok").onClick:Add(handler(self, self.OnConfirmBtnClick))
    self.modal = true
    self.sortingOrder = 2
end

function AlertWin:OnShown()
    Logger.Log(">>>AlertWin:ctor %s",self.vars)
    if self.vars.alertSound == '' then
        self.vars.alertSound = UIPackage.GetItemAsset("Sucai", "alert")
    end
    self.window:Center()
    
    GRoot.inst:PlayOneShotSound(self.vars.alertSound)
    GRoot.inst:CloseModalWait()
end

function AlertWin:OnConfirmBtnClick(context)
    -- log(">>>>>>>>>>AlertWin:OnConfirmBtnClick context:"..context.sender.name)
    self:Hide()
    -- if self.vars.callback ~=nil then
    -- 	local callback = self.vars.callback
    -- 	self.vars.callback = nil
    --     callback()
    -- end
end

return AlertWin