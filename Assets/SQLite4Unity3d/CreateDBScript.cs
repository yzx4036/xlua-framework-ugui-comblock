using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using EyeSoft.Data;
using UnityEngine.UI;

public class CreateDBScript : MonoBehaviour {

	public Text DebugText;

	// Use this for initialization
	void Start () {
//		StartSync();
		TestMyDataHelper();
	}

    private void StartSync()
    {
//        var ds = new DataService("game.db");
//        ds.CreateDB();
//        
//        var people = ds.GetPersons ();
//        ToConsole (people);
//        people = ds.GetPersonsNamedRoberto ();
//        ToConsole("Searching for Roberto ...");
//        ToConsole (people); 
    }

    private void TestMyDataHelper()
    {
	    EyeSoft.Data.SqlDataHelper<Person>.DropTable();
//
	    EyeSoft.Data.SqlDataHelper<Person>.CreateTable();
	    EyeSoft.Data.SqlDataHelper<Person>.InsertOneData(new Person()
	    {
		    Name = "Tom1233232",
		    Surname = "Perez222222222222222",
		    Age = 56,
		    numberList = new List<string>(){"1222","2ssssscxcx"},
	    }, false);
//	    EyeSoft.Data.SqlDataHelper<Person>.InsertOneData(new Person()
//	    {
//		    Name = "2232",
//		    Surname = "sxds",
//		    Age = 2
//	    }, false);
	    
//	    var item1 = SqlDataHelper<Person>.GetAllData().FirstOrDefault(x => x.Age ==2);
//	    if (item1 != null)
//	    {
//	    item1.Age = 22;
//	    EyeSoft.Data.SqlDataHelper<Person>.UpdateOneRowData(item1);
//	    }

//	    var  p = EyeSoft.Data.SqlDataHelper<Person>.GetOneDataByKey(45);
//	    Logger.LogColor(Color.green,  p.ToString());
	    var list = EyeSoft.Data.SqlDataHelper<Person>.GetAllData();
	    ToConsole(list);
    }

    private void ToConsole<T>(List<T> people){
		foreach (var person in people) {
			ToConsole(person.ToString());
		}
	}
	
	private void ToConsole(string msg){
		DebugText.text += System.Environment.NewLine + msg;
		Debug.Log (msg);
	}
}
