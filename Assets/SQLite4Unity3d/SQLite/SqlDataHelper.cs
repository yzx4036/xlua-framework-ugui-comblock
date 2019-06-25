using System.Collections;
using System.Collections.Generic;

namespace EyeSoft.Data
{
    /// <summary>
    /// sqlite数据库操作Helper
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SqlDataHelper<T> where T:DataModelBase, new()
    {
        public static int CreateTable()
        {
            return SQLiteManager.Instance.CreateTable<T>();
        }
        public static int DropTable()
        {
            return SQLiteManager.Instance.DropTable<T>();
        }
        public static int InsertOneData(T target, bool isReplace)
        {
            return SQLiteManager.Instance.InsertOneData(target, isReplace);
        }
	
        public static int DeleteOneData(object key)
        {
            return SQLiteManager.Instance.DeleteOneData<T>(key);
        }
	
        public static int UpdateOneRowData(T target)
        {
            return SQLiteManager.Instance.UpdateOneRowData(target);
        }

        public static List<T> GetAllData()
        {
            return SQLiteManager.Instance.GetAllDataByT<T>();
        }
	
        public static T GetOneDataByKey(object key)
        {
            return SQLiteManager.Instance.GetOneDataByTWithPrimaryKey<T>(key);
        }
    }
}