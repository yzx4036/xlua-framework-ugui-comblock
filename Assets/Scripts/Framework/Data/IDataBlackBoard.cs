using System;
using EyeSoft.Data;
using MatchKitEx;
using UnityEngine;

namespace EyeSoft.MatchKit
{
    /// <summary>
    /// 数据黑板接口
    /// </summary>
    public interface IDataBlackBoard:IDisposable
    {
        void Init();
        ICfgData LoadCfgData(int id);
        ICfgData GetCfgData(int id);
        SqlDataModelBase GetModelDataById(int id);
        bool UpdateModelData(SqlDataModelBase newValue);

    }
}