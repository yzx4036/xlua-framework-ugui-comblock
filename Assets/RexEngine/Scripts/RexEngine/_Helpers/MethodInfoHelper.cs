using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;

public static class MethodInfoHelper 
{
	public static List<MethodInfo> GetPublicVoids(this MonoBehaviour monoBehavior)
	{
		BindingFlags publicFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Default; 
		var publicVoids = monoBehavior.GetMethods(typeof(void), null, publicFlags); 

		return publicVoids;
	}

	public static List<MethodInfo> GetPrivateVoids(this MonoBehaviour monoBehavior)
	{
		BindingFlags privateFlags = BindingFlags.Instance | BindingFlags.NonPublic; 
		var privateVoids = monoBehavior.GetMethods(typeof(void), null, privateFlags);

		return privateVoids;
	}

	public static List<MethodInfo> GetMethods(this MonoBehaviour monoBehavior, Type returnType, Type[] paramTypes, BindingFlags flags)
	{
		return monoBehavior.GetType().GetMethods(flags).Where(m => m.ReturnType == returnType).Select(m => new { m, Params = m.GetParameters() }).Where(x =>
		{
			return (paramTypes == null) ? x.Params.Length == 0 : (x.Params.Length == paramTypes.Length && x.Params.Select(p => p.ParameterType).ToArray().IsEqualTo(paramTypes));
		})
		.Select(x => x.m).ToList();
	}

	public static List<MethodInfo> GetMethods(this GameObject go, Type returnType, Type[] paramTypes, BindingFlags flags)
	{
		var monoBehaviors = go.GetComponents<MonoBehaviour>();
		List<MethodInfo> methodInfo = new List<MethodInfo>();

		foreach(var monoBehavior in monoBehaviors) 
		{
			methodInfo.AddRange(monoBehavior.GetMethods(returnType, paramTypes, flags));
		}

		return methodInfo;
	}

	public static bool IsEqualTo<T>(this IList<T> list, IList<T> other)
	{
		if(list.Count != other.Count) 
		{
			return false;
		}
		for(int i = 0, count = list.Count; i < count; i++) 
		{
			if(!list[i].Equals(other[i])) 
			{
				return false;
			}
		}

		return true;
	}
}
