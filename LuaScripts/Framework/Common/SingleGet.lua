---
--- 单例统一获取类
--- Created by Qi.
--- DateTime: 2018\7\26 0026 9:21
---

SingleGet = {}

local _TimerManager
---@return TimerManager
function SingleGet.TimerManager()
    if _TimerManager == nil then
        _TimerManager = reload("gamecore.common.TimerManager").New()
    end
    return 	_TimerManager
end

local _TimerScaleManager
---@return TimerScaleManager
function SingleGet.TimerScaleManager()
    if _TimerScaleManager == nil then
        _TimerScaleManager = reload("gamecore.common.TimerScaleManager").New()
    end
    return 	_TimerScaleManager
end

local _MainActivityDataMgr
---@return MainActivityDataMgr
function SingleGet.MainActivityDataMgr()
    if _MainActivityDataMgr == nil then
        _MainActivityDataMgr = reload("gamecore.ui.main.Activity.MainActivityDataMgr").New()
    end
    return 	_MainActivityDataMgr
end

local _ArenaRankDataMgr
---@return ArenaRankDataMgr
function SingleGet.ArenaRankDataMgr()
    if _ArenaRankDataMgr == nil then
        _ArenaRankDataMgr = reload("gamecore.ui.Arena.Rank.ArenaRankDataMgr").New()
    end
    return 	_ArenaRankDataMgr
end

local _WorldDataMgr
---@return WorldDataMgr
function SingleGet.WorldDataMgr()
    if _WorldDataMgr == nil then
        _WorldDataMgr = reload("gamecore.world.WorldDataMgr").New()
    end
    return 	_WorldDataMgr
end

local _WorldMistyDataMgr
---@return WorldMistyDataMgr
function SingleGet.WorldMistyDataMgr()
    if _WorldMistyDataMgr == nil then
        _WorldMistyDataMgr = reload("gamecore.world.Misty.WorldMistyDataMgr").New()
    end
    return 	_WorldMistyDataMgr
end

local _WorldLostBoxDataMgr
---@return WorldLostBoxDataMgr
function SingleGet.WorldLostBoxDataMgr()
    if _WorldLostBoxDataMgr == nil then
        _WorldLostBoxDataMgr = reload("gamecore.world.LostBox.WorldLostBoxDataMgr").New()
    end
    return 	_WorldLostBoxDataMgr
end

local _CountryBattle_csgfz_BuffDataMgr
---@return CountryBattle_csgfz_BuffDataMgr
function SingleGet.CountryBattle_csgfz_BuffDataMgr()
    if _CountryBattle_csgfz_BuffDataMgr == nil then
        _CountryBattle_csgfz_BuffDataMgr = reload("gamecore.ui.countryBattle.CountryBattle_csgfz.buff.CountryBattle_csgfz_BuffDataMgr").New()
    end
    return 	_CountryBattle_csgfz_BuffDataMgr
end

local _CountryBattle_csgfz_DataMgr
---@return CountryBattle_csgfz_DataMgr
function SingleGet.CountryBattle_csgfz_DataMgr()
    if _CountryBattle_csgfz_DataMgr == nil then
        _CountryBattle_csgfz_DataMgr = reload("gamecore.ui.countryBattle.CountryBattle_csgfz.CountryBattle_csgfz_DataMgr").New()
    end
    return 	_CountryBattle_csgfz_DataMgr
end

local _CountryBattleDataMgr
---@return CountryBattleDataMgr
function SingleGet.CountryBattleDataMgr()
    if _CountryBattleDataMgr == nil then
        _CountryBattleDataMgr = reload("gamecore.ui.CountryBattle.CountryBattleDataMgr").New()
    end
    return 	_CountryBattleDataMgr
end

local _BarracksDataMgr
---@return BarracksDataMgr
function SingleGet.BarracksDataMgr()
    if _BarracksDataMgr == nil then
        _BarracksDataMgr = reload("gamecore.ui.Barracks.BarracksDataMgr").New()
    end
    return 	_BarracksDataMgr
end

local _RechargeDataMgr
---@return RechargeDataMgr
function SingleGet.RechargeDataMgr()
    if _RechargeDataMgr == nil then
        _RechargeDataMgr = reload("gamecore.ui.Recharge.RechargeDataMgr").New()
    end
    return 	_RechargeDataMgr
end

local _TeamMarshallingDataMgr
---@return TeamMarshallingDataMgr
function SingleGet.TeamMarshallingDataMgr()
    if _TeamMarshallingDataMgr == nil then
        _TeamMarshallingDataMgr = reload("gamecore.ui.Team.DataMgr.TeamMarshallingDataMgr").New()
    end
    return 	_TeamMarshallingDataMgr
end

local _TeamDataMgr
---@return TeamDataMgr
function SingleGet.TeamDataMgr()
    if _TeamDataMgr == nil then
        _TeamDataMgr = reload("gamecore.ui.Team.DataMgr.TeamDataMgr").New()
    end
    return 	_TeamDataMgr
end

local _WorldTeamDataMgr
---@return WorldTeamDataMgr
function SingleGet.WorldTeamDataMgr()
    if _WorldTeamDataMgr == nil then
        _WorldTeamDataMgr = reload("gamecore.ui.Team.DataMgr.WorldTeamDataMgr").New()
    end
    return 	_WorldTeamDataMgr
end

local _ArenaWarTeamDataMgr
---@return ArenaWarTeamDataMgr
function SingleGet.ArenaWarTeamDataMgr()
    if _ArenaWarTeamDataMgr == nil then
        _ArenaWarTeamDataMgr = reload("gamecore.ui.Team.DataMgr.ArenaWarTeamDataMgr").New()
    end
    return 	_ArenaWarTeamDataMgr
end

local _SandTableTeamDataMgr
---@return SandTableTeamDataMgr
function SingleGet.SandTableTeamDataMgr()
    if _SandTableTeamDataMgr == nil then
        _SandTableTeamDataMgr = reload("gamecore.ui.Team.DataMgr.SandTableTeamDataMgr").New()
    end
    return 	_SandTableTeamDataMgr
end

local _CopyTeamDataMgr
---@return CopyTeamDataMgr
function SingleGet.CopyTeamDataMgr()
    if _CopyTeamDataMgr == nil then
        _CopyTeamDataMgr = reload("gamecore.ui.Team.DataMgr.CopyTeamDataMgr").New()
    end
    return 	_CopyTeamDataMgr
end

local _WorldCityDataMgr
---@return WorldCityDataMgr
function SingleGet.WorldCityDataMgr()
    if _WorldCityDataMgr == nil then
        _WorldCityDataMgr = reload("gamecore.world.City.WorldCityDataMgr").New()
    end
    return 	_WorldCityDataMgr
end

local _ReddotDataMgr
---@return ReddotDataMgr
function SingleGet.ReddotDataMgr()
    if _ReddotDataMgr == nil then
        _ReddotDataMgr = reload("gamecore.ui.Reddot.ReddotDataMgr").New()
    end
    return 	_ReddotDataMgr
end

local _WorldMoveDataMgr
---@return WorldMoveDataMgr
function SingleGet.WorldMoveDataMgr()
    if _WorldMoveDataMgr == nil then
        _WorldMoveDataMgr = reload("gamecore.world.Move.WorldMoveDataMgr").New()
    end
    return 	_WorldMoveDataMgr
end


local _IronShopDataMgr
---@return IronShopDataMgr
function SingleGet.IronShopDataMgr()
    if _IronShopDataMgr == nil then
        _IronShopDataMgr = reload("gamecore.ui.IronShop.IronShopDataMgr").New()
    end
    return 	_IronShopDataMgr
end



local _WorldBuildingsDataMgr
---@return WorldBuildingsDataMgr
function SingleGet.WorldBuildingsDataMgr()
    if _WorldBuildingsDataMgr == nil then
        _WorldBuildingsDataMgr = reload("gamecore.ui.World.WorldBuildings.WorldBuildingsDataMgr").New()
    end
    return 	_WorldBuildingsDataMgr
end

local _WorldTanGuanDataMgr
---@return WorldTanGuanDataMgr
function SingleGet.WorldTanGuanDataMgr()
    if _WorldTanGuanDataMgr == nil then
        _WorldTanGuanDataMgr = reload("gamecore.world.SceneUI.Tag.WorldTanGuanDataMgr").New()
    end
    return _WorldTanGuanDataMgr
end

local _WorldCapitalDataMgr
---@return WorldCapitalDataMgr
function SingleGet.WorldCapitalDataMgr()
    if _WorldCapitalDataMgr == nil then
	    _WorldCapitalDataMgr = reload("gamecore.world.SceneUI.Tag.WorldCapitalDataMgr").New()
    end
    return _WorldCapitalDataMgr
end


local _SigninDataMgr
---@return SigninDataMgr
function SingleGet.SigninDataMgr()
    if _SigninDataMgr == nil then
        _SigninDataMgr = reload("gamecore.ui.Signin.SigninDataMgr").New()
    end
    return 	_SigninDataMgr
end



local _CountryCopyDataMgr
---@return CountryCopyDataMgr
function SingleGet.CountryCopyDataMgr()
    if _CountryCopyDataMgr == nil then
        _CountryCopyDataMgr = reload("gamecore.ui.CountryCopy.CountryCopyDataMgr").New()
    end
    return 	_CountryCopyDataMgr
end

local _StableDataMgr
---@return StableDataMgr
function SingleGet.StableDataMgr()
    if _StableDataMgr == nil then
        _StableDataMgr = reload("gamecore.ui.Stable.StableDataMgr").New()
    end
    return 	_StableDataMgr
end


local _EmailDataMgr
---@return EmailDataMgr
function SingleGet.EmailDataMgr()
    if _EmailDataMgr == nil then
        _EmailDataMgr = reload("gamecore.ui.Email.EmailDataMgr").New()
    end
    return 	_EmailDataMgr
end

local _GuideDataMgr
---@return GuideDataMgr
function SingleGet.GuideDataMgr()
    if _GuideDataMgr == nil then
        _GuideDataMgr = reload("gamecore.ui.Guide.Guide.GuideDataMgr").New()
    end
    return 	_GuideDataMgr
end

local _GuideTriggerController
---@return GuideTriggerController
function SingleGet.GuideTriggerController()
    if _GuideTriggerController == nil then
        _GuideTriggerController = reload("gamecore.ui.Guide.Guide.GuideTriggerController").New()
    end
    return 	_GuideTriggerController
end

local _FiefUpgradeDataMgr
---@return FiefUpgradeDataMgr
function SingleGet.FiefUpgradeDataMgr()
    if _FiefUpgradeDataMgr == nil then
        _FiefUpgradeDataMgr = reload("gamecore.ui.FiefUpgrade.FiefUpgradeDataMgr").New()
    end
    return 	_FiefUpgradeDataMgr
end

local _FiefBuildingRedPointMgr
---@return FiefBuildingRedPointMgr
function SingleGet.FiefBuildingRedPointMgr()
    if _FiefBuildingRedPointMgr == nil then
        _FiefBuildingRedPointMgr = reload("gamecore.fief.RedPoint.FiefBuildingRedPointMgr").New()
    end
    return 	_FiefBuildingRedPointMgr
end

local _TavernDataMgr
---@return TavernDataMgr
function SingleGet.TavernDataMgr()
    if _TavernDataMgr == nil then
        _TavernDataMgr = reload("gamecore.ui.Tavern.TavernDataMgr").New()
    end
    return 	_TavernDataMgr
end

local _RankDataMgr
---@return RankDataMgr
function SingleGet.RankDataMgr()
    if _RankDataMgr == nil then
        _RankDataMgr = reload("gamecore.ui.Rank.RankDataMgr").New()
    end
    return 	_RankDataMgr
end

local _PlayerDetailDataMgr
---@return PlayerDetailDataMgr
function SingleGet.PlayerDetailDataMgr()
    if _PlayerDetailDataMgr == nil then
        _PlayerDetailDataMgr = reload("gamecore.ui.otherPlayer.PlayerDetailDataMgr").New()
    end
    return 	_PlayerDetailDataMgr
end

local _PassGateRankTipsDataMgr
---@return PassGateRankTipsDataMgr
function SingleGet.PassGateRankTipsDataMgr()
    if _PassGateRankTipsDataMgr == nil then
        _PassGateRankTipsDataMgr = reload("gamecore.ui.PassGate.RankTips.PassGateRankTipsDataMgr").New()
    end
    return 	_PassGateRankTipsDataMgr
end

local _PeerageDataMgr
---@return PeerageDataMgr
function SingleGet.PeerageDataMgr()
    if _PeerageDataMgr == nil then
        _PeerageDataMgr = reload("gamecore.ui.peerage.PeerageDataMgr").New()
    end
    return 	_PeerageDataMgr
end


local _ConstructionDataMgr
---@return ConstructionDataMgr
function SingleGet.ConstructionDataMgr()
    if _ConstructionDataMgr == nil then
        _ConstructionDataMgr = reload("gamecore.ui.construction.ConstructionDataMgr").New()
    end
    return 	_ConstructionDataMgr
end

local _PassGateDataMgr
---@return PassGateDataMgr
function SingleGet.PassGateDataMgr()
    if _PassGateDataMgr == nil then
        _PassGateDataMgr = reload("gamecore.ui.PassGate.PassGateDataMgr").New()
    end
    return 	_PassGateDataMgr
end

local _HeroDataMgr
---@return HeroDataMgr
function SingleGet.HeroDataMgr()
    if _HeroDataMgr == nil then
        _HeroDataMgr = reload("gamecore.ui.Hero.HeroDataMgr").New()
    end
    return 	_HeroDataMgr
end

local _HeroReddotDataMgr
---@return HeroReddotDataMgr
function SingleGet.HeroReddotDataMgr()
    if _HeroReddotDataMgr == nil then
        _HeroReddotDataMgr = reload("gamecore.ui.Hero.HeroReddotDataMgr").New()
    end
    return 	_HeroReddotDataMgr
end



local _HeroWeaponDataMgr
---@return HeroWeaponDataMgr
function SingleGet.HeroWeaponDataMgr()
    if _HeroWeaponDataMgr == nil then
        _HeroWeaponDataMgr = reload("gamecore.ui.Hero.weapon.HeroWeaponDataMgr").New()
    end
    return 	_HeroWeaponDataMgr
end


local _HeroGuideDataMgr
---@return HeroGuideDataMgr
function SingleGet.HeroGuideDataMgr()
    if _HeroGuideDataMgr == nil then
        _HeroGuideDataMgr = reload("gamecore.ui.Hero.HeroGuideDataMgr").New()
    end
    return 	_HeroGuideDataMgr
end



local _PassGateFightSortDataMgr
---@return PassGateFightSortDataMgr
function SingleGet.PassGateFightSortDataMgr()
    if _PassGateFightSortDataMgr == nil then
        _PassGateFightSortDataMgr = reload("gamecore.ui.PassGate.FightSort.PassGateFightSortDataMgr").New()
    end
    return 	_PassGateFightSortDataMgr
end

local _ActivityUIDataMgr
---@return ActivityUIDataMgr
function SingleGet.ActivityUIDataMgr()
    if _ActivityUIDataMgr == nil then
        _ActivityUIDataMgr = reload("gamecore.ui.ActivityUI.ActivityUIDataMgr").New()
    end
    return 	_ActivityUIDataMgr
end

local _SevenRewardDataMgr
---@return SevenRewardDataMgr
function SingleGet.SevenRewardDataMgr()
    if _SevenRewardDataMgr == nil then
        _SevenRewardDataMgr = reload("gamecore.ui.ActivityUI.SevenReward.SevenRewardDataMgr").New()
    end
    return 	_SevenRewardDataMgr
end

local _DailyRechargeDataMgr
---@return DailyRechargeDataMgr
function SingleGet.DailyRechargeDataMgr()
    if _DailyRechargeDataMgr == nil then
        _DailyRechargeDataMgr = reload("gamecore.ui.ActivityUI.DailyRecharge.DailyRechargeDataMgr").New()
    end
    return 	_DailyRechargeDataMgr
end

local _GodTestDataMgr
---@return GodTestDataMgr
function SingleGet.GodTestDataMgr()
    if _GodTestDataMgr == nil then
        _GodTestDataMgr = reload("gamecore.ui.ActivityUI.GodTest.GodTestDataMgr").New()
    end
    return 	_GodTestDataMgr
end


local _GrowthFundDataMgr
---@return GrowthFundDataMgr
function SingleGet.GrowthFundDataMgr()
    if _GrowthFundDataMgr == nil then
        _GrowthFundDataMgr = reload("gamecore.ui.ActivityUI.GrowthFund.GrowthFundDataMgr").New()
    end
    return 	_GrowthFundDataMgr
end

local _CountryWarehouseDataMgr
---@return CountryWarehouseDataMgr
function SingleGet.CountryWarehouseDataMgr()
    if _CountryWarehouseDataMgr == nil then
        _CountryWarehouseDataMgr = reload("gamecore.ui.country.countryWarhouse.CountryWarehouseDataMgr").New()
    end
    return 	_CountryWarehouseDataMgr
end

local _CountryGovAffairsDataMgr
---@return CountryGovAffairsDataMgr
function SingleGet.CountryGovAffairsDataMgr()
    if _CountryGovAffairsDataMgr == nil then
        _CountryGovAffairsDataMgr = reload("gamecore.ui.country.countryGovAffairs.CountryGovAffairsDataMgr").New()
    end
    return 	_CountryGovAffairsDataMgr
end

local _OfficerSummonsDataMgr
---@return OfficerSummonsDataMgr
function SingleGet.OfficerSummonsDataMgr()
    if _OfficerSummonsDataMgr == nil then
        _OfficerSummonsDataMgr = reload("gamecore.ui.country.Officer.OfficerSummonsDataMgr").New()
    end
    return 	_OfficerSummonsDataMgr
end

local _OfficerDataMgr
---@return OfficerDataMgr
function SingleGet.OfficerDataMgr()
    if _OfficerDataMgr == nil then
        _OfficerDataMgr = reload("gamecore.ui.country.Officer.OfficerDataMgr").New()
    end
    return 	_OfficerDataMgr
end

local _MarketDataMgr
---@return MarketDataMgr
function SingleGet.MarketDataMgr()
    if _MarketDataMgr == nil then
        _MarketDataMgr = reload("gamecore.ui.Macket.MarketDataMgr").New()
    end
    return 	_MarketDataMgr
end

local _RolePeeragePrivilegeDataMgr
---@return RolePeeragePrivilegeDataMgr
function SingleGet.RolePeeragePrivilegeDataMgr()
    if _RolePeeragePrivilegeDataMgr == nil then
        _RolePeeragePrivilegeDataMgr = reload("gamecore.ui.role.RolePeeragePrivilegeDataMgr").New()
    end
    return 	_RolePeeragePrivilegeDataMgr
end

local _GMDataMgr
---@return GMDataMgr
function SingleGet.GMDataMgr()
    if _GMDataMgr == nil then
        _GMDataMgr = reload("gamecore.ui.gm.GMDataMgr").New()
    end
    return 	_GMDataMgr
end

local _BattlefieldDataMgr
---@return BattlefieldDataMgr
function SingleGet.BattlefieldDataMgr()
    if _BattlefieldDataMgr == nil then
        _BattlefieldDataMgr = reload("gamecore.ui.Battlefield.BattlefieldDataMgr").New()
    end
    return 	_BattlefieldDataMgr
end

local _PassGateBattleKillDataMgr
---@return PassGateBattleKillDataMgr
function SingleGet.PassGateBattleKillDataMgr()
    if _PassGateBattleKillDataMgr == nil then
        _PassGateBattleKillDataMgr = reload("gamecore.ui.PassGate.BattleKill.PassGateBattleKillDataMgr").New()
    end
    return 	_PassGateBattleKillDataMgr
end

local _SandTableDataMgr
---@return SandTableDataMgr
function SingleGet.SandTableDataMgr()
    if _SandTableDataMgr == nil then
        _SandTableDataMgr = reload("gamecore.ui.SandTable.SandTableDataMgr").New()
    end
    return 	_SandTableDataMgr
end

local _GroundInventoryDataMgr
---@return GroundInventoryDataMgr
function SingleGet.GroundInventoryDataMgr()
    if _GroundInventoryDataMgr == nil then
        _GroundInventoryDataMgr = reload("gamecore.ui.GroundInventory.GroundInventoryDataMgr").New()
    end
    return 	_GroundInventoryDataMgr
end

local _LoginDataMgr
---@return LoginDataMgr
function SingleGet.LoginDataMgr()
    if _LoginDataMgr == nil then
        _LoginDataMgr = reload("gamecore.ui.Login.LoginDataMgr").New()
    end
    return 	_LoginDataMgr
end

local _BattleQueueDataMgr
---@return BattleQueueDataMgr
function SingleGet.BattleQueueDataMgr()
    if _BattleQueueDataMgr == nil then
        _BattleQueueDataMgr = reload("gamecore.battle.Mgr.BattleQueueDataMgr").New()
    end
    return 	_BattleQueueDataMgr
end

local _BattleEventDataMgr
---@return BattleEventDataMgr
function SingleGet.BattleEventDataMgr()
    if _BattleEventDataMgr == nil then
        _BattleEventDataMgr = reload("gamecore.battle.Mgr.BattleEventDataMgr").New()
    end
    return 	_BattleEventDataMgr
end

local _BattleDataMgr
---@return BattleDataMgr
function SingleGet.BattleDataMgr()
    if _BattleDataMgr == nil then
        _BattleDataMgr = reload("gamecore.battle.Mgr.BattleDataMgr").New()
    end
    return 	_BattleDataMgr
end

local _BattleFixedDataMgr
---@return BattleFixedDataMgr
function SingleGet.BattleFixedDataMgr()
    if _BattleFixedDataMgr == nil then
        _BattleFixedDataMgr = reload("gamecore.battle.Mgr.BattleFixedDataMgr").New()
    end
    return 	_BattleFixedDataMgr
end

local _BattleAchievementDataMgr
---@return BattleAchievementDataMgr1
function SingleGet.BattleAchievementDataMgr()
    if _BattleAchievementDataMgr == nil then
        _BattleAchievementDataMgr = reload("gamecore.ui.BattleAchievement.BattleAchievementDataMgr1").New()
    end
    return 	_BattleAchievementDataMgr
end

local _AppSysDataMgr
---@return AppSysDataMgr
function SingleGet.AppSysDataMgr()
    if _AppSysDataMgr == nil then
        _AppSysDataMgr = reload("gamecore.ui.common.AppSysDataMgr").New()
    end
    return 	_AppSysDataMgr
end

local _FunctionTimesDataMgr
---@return FunctionTimesDataMgr
function SingleGet.FunctionTimesDataMgr()
    if _FunctionTimesDataMgr == nil then
        _FunctionTimesDataMgr = reload("gamecore.ui.common.FunctionTimesDataMgr").New()
    end
    return 	_FunctionTimesDataMgr
end

local _FunFirstUseDataMgr
---@return FunFirstUseDataMgr
function SingleGet.FunFirstUseDataMgr()
    if _FunFirstUseDataMgr == nil then
        _FunFirstUseDataMgr = reload("gamecore.ui.common.FunFirstUseDataMgr").New()
    end
    return 	_FunFirstUseDataMgr
end

local _BagDataMgr
---@return BagDataMgr
function SingleGet.BagDataMgr()
    if _BagDataMgr == nil then
        _BagDataMgr = reload("gamecore.ui.bag.BagDataMgr").New()
    end
    return 	_BagDataMgr
end

local _WorldCityBuffDataMgr
---@return WorldCityBuffDataMgr
function SingleGet.WorldCityBuffDataMgr()
    if _WorldCityBuffDataMgr == nil then
        _WorldCityBuffDataMgr = reload("gamecore.world.CityBuff.WorldCityBuffDataMgr").New()
    end
    return 	_WorldCityBuffDataMgr
end

local _HeroTrialDataMgr
---@return HeroTrialDataMgr
function SingleGet.HeroTrialDataMgr()
    if _HeroTrialDataMgr == nil then
        _HeroTrialDataMgr = reload("gamecore.ui.HeroTrial.HeroTrialDataMgr").New()
    end
    return 	_HeroTrialDataMgr
end

local _VisitDataMgr
---@return VisitDataMgr
function SingleGet.VisitDataMgr()
    if _VisitDataMgr == nil then
        _VisitDataMgr = reload("gamecore.ui.Visit.VisitDataMgr").New()
    end
    return 	_VisitDataMgr
end


local _VipDataMgr
---@return VipDataMgr
function SingleGet.VipDataMgr()
    if _VipDataMgr == nil then
        _VipDataMgr = reload("gamecore.ui.vip.VipDataMgr").New()
    end
    return 	_VipDataMgr
end

local _EmailWarReportDataMgr
---@return EmailWarReportDataMgr
function SingleGet.EmailWarReportDataMgr()
    if _EmailWarReportDataMgr == nil then
        _EmailWarReportDataMgr = reload("gamecore.ui.Email.WarReport.EmailWarReportDataMgr").New()
    end
    return 	_EmailWarReportDataMgr
end

local _FindDataMgr
---@return FindDataMgr
function SingleGet.FindDataMgr()
    if _FindDataMgr == nil then
        _FindDataMgr = reload("gamecore.ui.Find.FindDataMgr").New()
    end
    return 	_FindDataMgr
end

local _MainDataMgr
---@return MainDataMgr
function SingleGet.MainDataMgr()
    if _MainDataMgr == nil then
        _MainDataMgr = reload("gamecore.ui.main.MainDataMgr").New()
    end
    return 	_MainDataMgr
end

local _MainIconDataMgr
---@return MainIconDataMgr
function SingleGet.MainIconDataMgr()
    if _MainIconDataMgr == nil then
        _MainIconDataMgr = reload("gamecore.ui.main.Icon.MainIconDataMgr").New()
    end
    return 	_MainIconDataMgr
end

local _BackMainSceneMgr
---@return BackMainSceneMgr
function SingleGet.BackMainSceneMgr()
    if _BackMainSceneMgr == nil then
        _BackMainSceneMgr = reload("gamecore.ui.main.BackMainSceneMgr").New()
    end
    return 	_BackMainSceneMgr
end

local _ShopDataMgr
---@return ShopDataMgr
function SingleGet.ShopDataMgr()
    if _ShopDataMgr == nil then
        _ShopDataMgr = reload("gamecore.ui.Shop.ShopDataMgr").New()
    end
    return 	_ShopDataMgr
end

local _BuildQueueDataMgr
---@return BuildQueueDataMgr
function SingleGet.BuildQueueDataMgr()
    if _BuildQueueDataMgr == nil then
        _BuildQueueDataMgr = reload("gamecore.fief.Data.BuildQueueDataMgr").New()
    end
    return 	_BuildQueueDataMgr
end

local _BuildDataMgr
---@return BuildDataMgr
function SingleGet.BuildDataMgr()
    if _BuildDataMgr == nil then
        _BuildDataMgr = reload("gamecore.fief.Data.BuildDataMgr").New()
    end
    return 	_BuildDataMgr
end

local _BuildElementMgr
---@return BuildElementMgr
function SingleGet.BuildElementMgr()
    if _BuildElementMgr == nil then
        _BuildElementMgr = reload("gamecore.fief.Data.BuildElementMgr").New()
    end
    return 	_BuildElementMgr
end

local _FiefSceneMgr
---@return FiefSceneMgr
function SingleGet.FiefSceneMgr()
    if _FiefSceneMgr == nil then
        _FiefSceneMgr = reload("gamecore.fief.SceneUI.FiefSceneMgr").New()
    end
    return 	_FiefSceneMgr
end


local _FiefBuildOperateMgr
---@return FiefBuildOperateMgr
function SingleGet.FiefBuildOperateMgr()
    if _FiefBuildOperateMgr == nil then
        _FiefBuildOperateMgr = reload("gamecore.fief.SceneUI.BuildOperate.FiefBuildOperateMgr").New()
    end
    return 	_FiefBuildOperateMgr
end

local _FiefPatrolWalkMgr
---@return FiefPatrolWalkMgr
function SingleGet.FiefPatrolWalkMgr()
    if _FiefPatrolWalkMgr == nil then
        _FiefPatrolWalkMgr = reload("gamecore.fief.ScenePatrolWalk.FiefPatrolWalkMgr").New()
    end
    return 	_FiefPatrolWalkMgr
end

local _FiefCoolDownMgr
---@return FiefCoolDownMgr
function SingleGet.FiefCoolDownMgr()
    if _FiefCoolDownMgr == nil then
        _FiefCoolDownMgr = reload("gamecore.fief.SceneUI.FiefCoolDownMgr").New()
    end
    return 	_FiefCoolDownMgr
end


local _NoticeDataMgr
---@return NoticeDataMgr
function SingleGet.NoticeDataMgr()
    if _NoticeDataMgr == nil then
        _NoticeDataMgr = reload("gamecore.ui.notice.NoticeDataMgr").New()
    end
    return 	_NoticeDataMgr
end

local _ArenaDataMgr
---@return ArenaDataMgr
function SingleGet.ArenaDataMgr()
    if _ArenaDataMgr == nil then
        _ArenaDataMgr = reload("gamecore.ui.Arena.ArenaDataMgr").New()
    end
    return 	_ArenaDataMgr
end

local _ArenaFightDataMgr
---@return ArenaFightDataMgr
function SingleGet.ArenaFightDataMgr()
    if _ArenaFightDataMgr == nil then
        _ArenaFightDataMgr = reload("gamecore.ui.Arena.ArenaFightDataMgr").New()
    end
    return 	_ArenaFightDataMgr
end

local _ChatDataMgr
---@return ChatDataMgr
function SingleGet.ChatDataMgr()
    if _ChatDataMgr == nil then
        _ChatDataMgr = reload("gamecore.ui.chat.ChatDataMgr").New()
    end
    return 	_ChatDataMgr
end

local _SceneSelectDataMgr
---@return SceneSelectDataMgr
function SingleGet.SceneSelectDataMgr()
    if _SceneSelectDataMgr == nil then
        _SceneSelectDataMgr = reload("gamecore.ui.World.SelectTeam.SceneSelectDataMgr").New()
    end
    return 	_SceneSelectDataMgr
end

local _TaskDataMgr
---@return TaskDataMgr
function SingleGet.TaskDataMgr()
    if _TaskDataMgr == nil then
        _TaskDataMgr = reload("gamecore.ui.Task.TaskDataMgr").New()
    end
    return 	_TaskDataMgr
end

local _ReputationDataMgr
---@return ReputationDataMgr
function SingleGet.ReputationDataMgr()
    if _ReputationDataMgr == nil then
        _ReputationDataMgr = reload("gamecore.ui.Task.ReputationDataMgr").New()
    end
    return 	_ReputationDataMgr
end

local _LivenessDataMgr
---@return LivenessDataMgr
function SingleGet.LivenessDataMgr()
    if _LivenessDataMgr == nil then
        _LivenessDataMgr = reload("gamecore.ui.Task.liveness.LivenessDataMgr").New()
    end
    return 	_LivenessDataMgr
end

local _FriendDataMgr
---@return FriendDataMgr
function SingleGet.FriendDataMgr()
    if _FriendDataMgr == nil then
        _FriendDataMgr = reload("gamecore.ui.chat.friend.FriendDataMgr").New()
    end
    return 	_FriendDataMgr
end

local _ChatRedPacketDataMgr
---@return ChatRedPacketDataMgr
function SingleGet.ChatRedPacketDataMgr()
    if _ChatRedPacketDataMgr == nil then
        _ChatRedPacketDataMgr = reload("gamecore.ui.chat.redpacket.ChatRedPacketDataMgr").New()
    end
    return 	_ChatRedPacketDataMgr
end

local _PlotDataMgr
---@return PlotDataMgr
function SingleGet.PlotDataMgr()
    if _PlotDataMgr == nil then
        _PlotDataMgr = reload("gamecore.Plot.PlotDataMgr").New()
    end
    return 	_PlotDataMgr
end

local _PlotWorldCityDataMgr
---@return PlotWorldCityDataMgr
function SingleGet.PlotWorldCityDataMgr()
    if _PlotWorldCityDataMgr == nil then
        _PlotWorldCityDataMgr = reload("gamecore.Plot.PlotWorldCityDataMgr").New()
    end
    return 	_PlotWorldCityDataMgr
end

local _PlotCityAttackMgr
---@return PlotCityAttackMgr
function SingleGet.PlotCityAttackMgr()
    if _PlotCityAttackMgr == nil then
        _PlotCityAttackMgr = reload("gamecore.Plot.Attack.PlotCityAttackMgr").New()
    end
    return 	_PlotCityAttackMgr
end

local _PlotWorldElementMgr
---@return PlotWorldElementMgr
function SingleGet.PlotWorldElementMgr()
    if _PlotWorldElementMgr == nil then
	    _PlotWorldElementMgr = reload("gamecore.Plot.PlotWorldElementMgr").New()
    end
    return 	_PlotWorldElementMgr
end

local _PlotPlayMgr
---@return PlotPlayMgr
function SingleGet.PlotPlayMgr()
	if _PlotPlayMgr == nil then
		_PlotPlayMgr = reload("gamecore.Plot.PlotPlayMgr").New()
	end
	return 	_PlotPlayMgr
end

local _CompareNotesDataMgr
---@return CompareNotesDataMgr
function SingleGet.CompareNotesDataMgr()
    if _CompareNotesDataMgr == nil then
        _CompareNotesDataMgr = reload("gamecore.ui.Compare.CompareNotesDataMgr").New()
    end
    return 	_CompareNotesDataMgr
end


local _InvasionDataMgr
---@return InvasionDataMgr
function SingleGet.InvasionDataMgr()
    if _InvasionDataMgr == nil then
        _InvasionDataMgr = reload("gamecore.ui.Invasion.InvasionDataMgr").New()
    end
    return 	_InvasionDataMgr
end

local _WorldElementMgr
---@return WorldElementMgr
function SingleGet.WorldElementMgr()
    if _WorldElementMgr == nil then
        _WorldElementMgr = reload("gamecore.world.City.WorldElementMgr").New()
    end
    return 	_WorldElementMgr
end

local _WorldOperateBtnMgr
---@return WorldOperateBtnMgr
function SingleGet.WorldOperateBtnMgr()
    if _WorldOperateBtnMgr == nil then
        _WorldOperateBtnMgr = reload("gamecore.world.City.WorldOperateBtnMgr").New()
    end
    return 	_WorldOperateBtnMgr
end

local _WorldInformationMgr
---@return WorldInformationMgr
function SingleGet.WorldInformationMgr()
    if _WorldInformationMgr == nil then
        _WorldInformationMgr = reload("gamecore.world.SceneUI.Information.WorldInformationMgr").New()
    end
    return 	_WorldInformationMgr
end

local _GuidePlayMgr
---@return GuidePlayMgr
function SingleGet.GuidePlayMgr()
    if _GuidePlayMgr == nil then
        _GuidePlayMgr = reload("gamecore.ui.Guide.Guide.GuidePlayMgr").New()
    end
    return 	_GuidePlayMgr
end

local _GuideJumpPlayMgr
---@return GuideJumpPlayMgr
function SingleGet.GuideJumpPlayMgr()
    if _GuideJumpPlayMgr == nil then
        _GuideJumpPlayMgr = reload("gamecore.ui.Guide.Jump.GuideJumpPlayMgr").New()
    end
    return 	_GuideJumpPlayMgr
end

local _GuideController
---@return GuideController
function SingleGet.GuideController()
    if _GuideController == nil then
        _GuideController = reload("gamecore.ui.Guide.Guide.GuideController").New()
    end
    return 	_GuideController
end

local _TavernSceneMgr
---@return TavernSceneMgr
function SingleGet.TavernSceneMgr()
    if _TavernSceneMgr == nil then
        _TavernSceneMgr = reload("gamecore.ui.Tavern.TavernSceneMgr").New()
    end
    return 	_TavernSceneMgr
end

local _SourceGotoMgr
---@return SourceGotoMgr
function SingleGet.SourceGotoMgr()
    if _SourceGotoMgr == nil then
        _SourceGotoMgr = reload("gamecore.ui.SourceGoto.SourceGotoMgr").New()
    end
    return 	_SourceGotoMgr
end

local _HeartBeatMgr
---@return HeartBeatMgr
function SingleGet.HeartBeatMgr()
    if _HeartBeatMgr == nil then
        _HeartBeatMgr = reload("gamecore.ui.sys.HeartBeatMgr").New()
    end
    return 	_HeartBeatMgr
end

local _SceneLoadingMgr
---@return SceneLoadingMgr
function SingleGet.SceneLoadingMgr()
    if _SceneLoadingMgr == nil then
        _SceneLoadingMgr = reload("gamecore.ui.loader.SceneLoadingMgr").New()
    end
    return 	_SceneLoadingMgr
end

local _CityAttackMgr
---@return CityAttackMgr
function SingleGet.CityAttackMgr()
    if _CityAttackMgr == nil then
        _CityAttackMgr = reload("gamecore.ui.CityAttack.CityAttackMgr").New()
    end
    return 	_CityAttackMgr
end

local _ReconnectionMgr
---@return ReconnectionMgr
function SingleGet.ReconnectionMgr()
    if _ReconnectionMgr == nil then
        _ReconnectionMgr = reload("network/ReconnectionMgr").New()
    end
    return 	_ReconnectionMgr
end

local _InitializeRequestQueue
---@return InitializeRequestQueue
function SingleGet.InitializeRequestQueue()
    if _InitializeRequestQueue == nil then
        _InitializeRequestQueue = reload("network.InitializeRequestQueue").New()
    end
    return 	_InitializeRequestQueue
end

local _FollowTipsMgr
---@return FollowTipsMgr
function SingleGet.FollowTipsMgr()
    if _FollowTipsMgr == nil then
        _FollowTipsMgr = reload("gamecore.ui.tips.FollowTips.FollowTipsMgr").New()
    end
    return 	_FollowTipsMgr
end

local _MilitaryResearchDataMgr
---@return MilitaryResearchDataMgr
function SingleGet.MilitaryResearchDataMgr()
    if _MilitaryResearchDataMgr == nil then
        _MilitaryResearchDataMgr = reload("gamecore.ui.Research.MilitaryResearchDataMgr").New(EResearchType.military)
    end
    return 	_MilitaryResearchDataMgr
end


local _CollegeResearchDataMgr
---@return CollegeResearchDataMgr
function SingleGet.CollegeResearchDataMgr()
    if _CollegeResearchDataMgr == nil then
        _CollegeResearchDataMgr = reload("gamecore.ui.Research.CollegeResearchDataMgr").New(EResearchType.college)
    end
    return 	_CollegeResearchDataMgr
end



local _WorldTrendDataMgr
---@return WorldTrendDataMgr
function SingleGet.WorldTrendDataMgr()
    if _WorldTrendDataMgr == nil then
        _WorldTrendDataMgr = reload("gamecore.ui.WorldTrend.WorldTrendDataMgr").New()
    end
    return 	_WorldTrendDataMgr
end

local _SoloMeetingDataMgr
---@return SoloMeetingDataMgr
function SingleGet.SoloMeetingDataMgr()
    if _SoloMeetingDataMgr == nil then
        _SoloMeetingDataMgr = reload("gamecore.ui.SoloMeeting.SoloMeetingDataMgr").New()
    end
    return 	_SoloMeetingDataMgr
end


local _AutoAttDefDataMgr
---@return AutoAttDefDataMgr
function SingleGet.AutoAttDefDataMgr()
    if _AutoAttDefDataMgr == nil then
        _AutoAttDefDataMgr = reload("gamecore.ui.AutoAttDef.AutoAttDefDataMgr").New()
    end
    return 	_AutoAttDefDataMgr
end

local _WorldMiniInfluenceMgr
---@return WorldInfluenceDataMgr
function SingleGet.WorldInfluenceDataMgr()
    if _WorldMiniInfluenceMgr == nil then
        _WorldMiniInfluenceMgr = reload("gamecore.world.MiniMap.WorldInfluenceDataMgr").New()
    end
    return _WorldMiniInfluenceMgr
end

local _WorldTrendUpgDataMgr
---@return WorldTrendUpgDataMgr
function SingleGet.WorldTrendUpgDataMgr()
    if _WorldTrendUpgDataMgr == nil then
        _WorldTrendUpgDataMgr = reload("gamecore.ui.WorldTrendUpg.WorldTrendUpgDataMgr").New()
    end
    return _WorldTrendUpgDataMgr
end

local _CheerDataMgr
---@return CheerDataMgr
function SingleGet.CheerDataMgr()
    if _CheerDataMgr == nil then
        _CheerDataMgr = reload("gamecore.ui.country.Cheer.CheerDataMgr").New()
    end
    return _CheerDataMgr
end

local _WorldUnifyDataMgr
---@return WorldUnifyDataMgr
function SingleGet.WorldUnifyDataMgr()
    if _WorldUnifyDataMgr == nil then
        _WorldUnifyDataMgr = reload("gamecore.ui.WorldUnify.WorldUnifyDataMgr").New()
    end
    return _WorldUnifyDataMgr
end

local _CountryBuffDataMgr
---@return CountryBuffDataMgr
function SingleGet.CountryBuffDataMgr()
    if _CountryBuffDataMgr == nil then
        _CountryBuffDataMgr = reload("gamecore.ui.CountryBuff.CountryBuffDataMgr").New()
    end
    return _CountryBuffDataMgr
end

local _BattlePowerUpDataMgr
---@return BattlePowerUpDataMgr
function SingleGet.BattlePowerUpDataMgr()
    if _BattlePowerUpDataMgr == nil then
        _BattlePowerUpDataMgr = reload("gamecore.ui.BattlePowerUp.BattlePowerUpDataMgr").New()
    end
    return _BattlePowerUpDataMgr
end


