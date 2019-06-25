using System.Collections;
using System.Linq;
using EyeSoft.Data;
using SQLite4Unity3d;

public class Person:DataModelBase
{

	[PrimaryKey, AutoIncrement]
	public int Id { get; set; }
	public string Name { get; set; }
	public string Surname { get; set; }
	public int Age { get; set; }
	public IList numberList { get; set; }

	public override string ToString ()
	{
		string str = string.Format ("[Person: Id={0}, Name={1},  Surname={2}, Age={3}]", Id, Name, Surname, Age);
		for (int i = 0; i < numberList.Count; i++)
		{
			str +=string.Format("item[{0}] ={1}", i, numberList[i]);

		}
		return str;
	}
}
