using SQLite4Unity3d;
using UnityEngine;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;
using EyeSoft.Data;

public class DataService  {

	private SQLiteConnection _connection;

	public DataService(string DatabaseName){

#if UNITY_EDITOR
            var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID 
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
		var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
		
#elif UNITY_STANDALONE_OSX
		var loadDb = Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
		_connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        Debug.Log("Final PATH: " + dbPath);     

	}

	public int CreateTable<T>() where  T:DataModelBase, new()
	{
		var a =_connection.CreateTable<T> ();
		return a;
	}
	
	public int DropTable<T>() where  T:DataModelBase, new()
	{
		var result = _connection.DropTable<T> ();
		return result;
	}
	
	/// <summary>
	/// 插入一条数据 最
	/// </summary>
	/// <param name="target"></param>
	/// <param name="isReplace"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public int InsertOneData<T>(T target, bool isReplace = false) where  T:DataModelBase, new()
	{
		var s = isReplace ? _connection.InsertOrReplace(target) : _connection.Insert(target);
		return s;
	}
	
	public int DeleteOneData<T>(object key) where  T:DataModelBase, new()
	{
		var row = _connection.Delete<T>(key);
		return row;
	}
	
	public int UpdateOneRowData<T>(T target) where  T:DataModelBase, new()
	{
		var row = _connection.Update(target);
		return row;

	}

	public IEnumerable<T> GetAllDataByT<T>() where  T:DataModelBase, new()
	{
		var list =_connection.Table<T>();
		return list;
	}
	
	public T GetOneDataByTWithPrimaryKey<T>(object key) where  T:DataModelBase, new()
	{
		var  obj = _connection.Get<T>(key);

		return obj;
	}

	public object GetSyncObject()
	{
		return _connection.SyncObject;
	}

	
//
//	public void CreateDB(){
//		_connection.DropTable<Person> ();
//		_connection.CreateTable<Person> ();
//		_connection.InsertAll (new[]{
//			new Person{
//				Id = 1,
//				Name = "Tom",
//				Surname = "Perez",
//				Age = 56
//			},
//			new Person{
//				Id = 2,
//				Name = "Fred",
//				Surname = "Arthurson",
//				Age = 16
//			},
//			new Person{
//				Id = 3,
//				Name = "John",
//				Surname = "Doe",
//				Age = 25
//			},
//			new Person{
//				Id = 4,
//				Name = "Roberto",
//				Surname = "Huertas",
//				Age = 37
//			}
//		});
//	}
//
//	public IEnumerable<Person> GetPersons(){
//		_connection.Close();
//		return _connection.Table<Person>();
//	}
//	
//
//	public IEnumerable<Person> GetPersonsNamedRoberto(){
//		return _connection.Table<Person>().Where(x => x.Name == "Roberto");
//	}
//
//	public Person GetJohnny(){
//		return _connection.Table<Person>().Where(x => x.Name == "Johnny").FirstOrDefault();
//	}
//
//	public Person CreatePerson(){
//		var p = new Person{
//				Name = "Johnny",
//				Surname = "Mnemonic",
//				Age = 21
//		};
//		_connection.Insert (p);
//		return p;
//	}

	private int refCount;

	public void OpenConnection()
	{
		refCount++;
		_connection.Open();
	}

	public void CloseConnection()
	{
		refCount--;
		if (refCount == 0)
		{
//			Logger.LogColor(Color.red, ">>>> refCount {0}", refCount);
			_connection.Close();
		}
	}


	public void Dispose()
	{
		Logger.Log(">>>>Dispose");
		_connection.Close();
		_connection.Dispose();
	}
}
