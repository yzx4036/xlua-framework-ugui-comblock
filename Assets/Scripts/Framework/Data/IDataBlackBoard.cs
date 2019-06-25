using System;
using EyeSoft.Data;
using MatchKitEx;
using UnityEngine;

namespace EyeSoft.MatchKit
{
    /// <summary>
    /// 数据库黑板接口，没有用到sqlite的可忽略此接口
    /// </summary>
    public interface IDataBlackBoard:IDisposable
    {
        void Init();
        ICfgData LoadCfgData(int id);
        ICfgData GetCfgData(int id);
        DataModelBase GetModelDataById(int id);
        bool UpdateModelData(DataModelBase newValue);

    }
}