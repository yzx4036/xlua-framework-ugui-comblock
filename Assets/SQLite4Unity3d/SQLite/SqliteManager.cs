using System;
using System.Collections;
//using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SQLite4Unity3d;

namespace EyeSoft.Data
{
    /// <summary>
    /// 数据库管理器 对DataService操作
    /// </summary>
    public class SQLiteManager : MonoSingleton<SQLiteManager>
    {
        private string _dbName = "gameDb";
        private DataService dataService = null;
        private static object syncObject = new object();

        protected override void Init()
        { 
            dataService = new DataService(_dbName);
        }

        public int CreateTable<T>() where T:DataModelBase, new()
        {
            lock (syncObject)
            {
                dataService.OpenConnection();
                var  result = dataService.CreateTable<T>();
                dataService.CloseConnection();

                return result;
            }
        }
        
        public int DropTable<T>() where T:DataModelBase, new()
        {
            lock (syncObject)
            {
                dataService.OpenConnection();
                var result = dataService.DropTable<T>();
                dataService.CloseConnection();
                return result;

            }
        }
        
        public int InsertOneData<T>(T target, bool isReplace) where  T:DataModelBase, new()
        {
            lock (syncObject)
            {
                dataService.OpenConnection();
                var result = dataService.InsertOneData(target, isReplace);
                dataService.CloseConnection();
                return result;

            }
        }
	
        public int DeleteOneData<T>(object key) where T:DataModelBase, new()
        {
            lock (syncObject)
            {
                dataService.OpenConnection();
                var result = dataService.DeleteOneData<T>(key);
                dataService.CloseConnection();
                return result;

            }
        }
	
        public int UpdateOneRowData<T>(T target) where T:DataModelBase, new()
        {
            lock (syncObject)
            {
                dataService.OpenConnection();
                var result = dataService.UpdateOneRowData(target);
                dataService.CloseConnection();
                return result;

            }
        }

        public List<T> GetAllDataByT<T>() where T:DataModelBase, new()
        {
            lock (syncObject)
            {
                dataService.OpenConnection();
                var result = dataService.GetAllDataByT<T>().ToList();
                dataService.CloseConnection();
                return result;

            }
        }
	
        public T GetOneDataByTWithPrimaryKey<T>(object key) where T:DataModelBase, new()
        {
            lock (syncObject)
            {
                dataService.OpenConnection();
                var result = dataService.GetOneDataByTWithPrimaryKey<T>(key);
                dataService.CloseConnection();
                return result;

            }
        }

        private void OnDisable()
        {
            this.Dispose();
        }

        public override void Dispose()
        {
            if (dataService != null)
            {
                dataService.Dispose();
                dataService = null;
            }
        }
    }

}
