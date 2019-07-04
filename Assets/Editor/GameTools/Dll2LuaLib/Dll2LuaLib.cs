using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ICSharpCode.SharpZipLib.Zip;

public class Dll2LuaLib : Editor {

	[MenuItem("Tools/Lua/Dll2LuaLib", false, -100)]
	public static void GenAll()
	{
		GenDll("Assembly-CSharp");
		GenDll("UnityEngine");
		GenDll("mscorlib");
		GenDll("DOTween");
	}

	private static void GenDll(string dllName)
	{
		Assembly assembly = Assembly.Load(dllName);
		if (assembly == null)
		{
			Debug.LogError(dllName + " assembly is not exist!");
			return;
		}
		Dictionary<string, byte[]> fileDict = new Dictionary<string, byte[]>();
		Type[] types = assembly.GetTypes();
		foreach (Type type in types)
		{
			string fileName;
			string content;
			GenType(type, out fileName, out content);
			fileDict[fileName] = Encoding.UTF8.GetBytes(content);

			Type baseType = type.BaseType;
			while (baseType != null && baseType.IsGenericType && !baseType.IsGenericTypeDefinition)
			{
				string baseFileName;
				string baseContent;
				GenType(baseType, out baseFileName, out baseContent);
				fileDict[baseFileName] = Encoding.UTF8.GetBytes(baseContent);
				baseType = baseType.BaseType;
			}
		}
		HashSet<string> nameSpaceSet = new HashSet<string>();
		foreach (Type type in types)
		{
			string nameSpace = type.Namespace;
			if (nameSpace != null && !nameSpaceSet.Contains(nameSpace))
			{
				nameSpaceSet.Add(nameSpace);
			}
		}
		foreach (string nameSpace in nameSpaceSet)
		{
			string fileName = nameSpace + ".ns.lua";
			string content = nameSpace + " = {}";
			fileDict[fileName] = Encoding.UTF8.GetBytes(content);
		}
		string zipFileName = Application.dataPath + "/../LuaProj/API/" + dllName + ".zip";
		ZipDerctory(zipFileName, fileDict);
		Debug.Log(dllName + ".zip generating is complete!");
	}

	private static void GenType(Type type, out string fileName, out string content)
	{
		string typeName = TypeToString(type, true);
		string typeFileName = typeName + ".lua";
		StringBuilder typeScriptSb = new StringBuilder();
		typeScriptSb.Append("---@class ");
		typeScriptSb.Append(typeName);
		typeScriptSb.Append(" : ");
		if (type.BaseType != null)
		{
			typeScriptSb.Append(TypeToString(type.BaseType, true));
		}
		else
		{
			typeScriptSb.Append("table");
		}
		typeScriptSb.AppendLine();

		FieldInfo[] staticFields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
		foreach (FieldInfo field in staticFields)
		{
			typeScriptSb.Append("---@field public ");
			typeScriptSb.Append(field.Name);
			typeScriptSb.Append(" ");
			typeScriptSb.Append(TypeToString(field.FieldType));
			typeScriptSb.Append(" @static");
			typeScriptSb.AppendLine();
		}
		PropertyInfo[] staticProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
		foreach (PropertyInfo property in staticProperties)
		{
			typeScriptSb.Append("---@field public ");
			typeScriptSb.Append(property.Name);
			typeScriptSb.Append(" ");
			typeScriptSb.Append(TypeToString(property.PropertyType));
			typeScriptSb.Append(" @static");
			typeScriptSb.AppendLine();
		}

		FieldInfo[] instanceFields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		foreach (FieldInfo field in instanceFields)
		{
			typeScriptSb.Append("---@field public ");
			typeScriptSb.Append(field.Name);
			typeScriptSb.Append(" ");
			typeScriptSb.Append(TypeToString(field.FieldType));
			typeScriptSb.AppendLine();
		}
		PropertyInfo[] instanceProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		foreach (PropertyInfo property in instanceProperties)
		{
			typeScriptSb.Append("---@field public ");
			typeScriptSb.Append(property.Name);
			typeScriptSb.Append(" ");
			typeScriptSb.Append(TypeToString(property.PropertyType));
			typeScriptSb.AppendLine();
		}

		typeScriptSb.Append("local m = {}");
		typeScriptSb.AppendLine();

		Dictionary<string, List<MethodInfo>> methodNameDict = new Dictionary<string, List<MethodInfo>>();
		MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		foreach (MethodInfo method in methods)
		{
			string methodName = method.Name;
			if (!methodName.StartsWith("get_") && !methodName.StartsWith("set_"))
			{
				if (!methodNameDict.ContainsKey(methodName))
				{
					methodNameDict.Add(methodName, new List<MethodInfo>());
				}
				methodNameDict[methodName].Add(method);
			}
		}

		foreach (string methodName in methodNameDict.Keys)
		{
			typeScriptSb.AppendLine();

			List<MethodInfo> methodList = methodNameDict[methodName];
			List<List<ParameterInfo>> paramListList = new List<List<ParameterInfo>>();
			List<List<ParameterInfo>> returnListList = new List<List<ParameterInfo>>();
			List<MethodInfo> fromMethodList = new List<MethodInfo>();
			for (int methodIndex = 0; methodIndex < methodList.Count; methodIndex++)
			{
				MethodInfo method = methodList[methodIndex];
				List<ParameterInfo> paramList = new List<ParameterInfo>();
				List<ParameterInfo> returnList = new List<ParameterInfo>();
                if (method.ReturnParameter.ParameterType != typeof(void))
				{
					returnList.Add(method.ReturnParameter);
				}
				ParameterInfo[] parameters = method.GetParameters();
				for (int paramIndex = 0; paramIndex < parameters.Length; paramIndex++)
				{
					ParameterInfo param = parameters[paramIndex];
					if (!param.IsOut)
					{
						paramList.Add(param);
					}
					if (param.ParameterType.IsByRef)
					{
						returnList.Add(param);
					}
				}
				paramListList.Add(paramList);
				returnListList.Add(returnList);
				fromMethodList.Add(method);
				for (int paramIndex = paramList.Count - 1; paramIndex >= 0; paramIndex--)
				{
					ParameterInfo param = paramList[paramIndex];
					if (param.IsOptional || param.IsDefined(typeof(ParamArrayAttribute), false))
					{
						List<ParameterInfo> overloadParamList = new List<ParameterInfo>();
						for (int index = 0; index < paramIndex; index++)
						{
							overloadParamList.Add(paramList[index]);
						}
						paramListList.Add(overloadParamList);
						returnListList.Add(returnList);
						fromMethodList.Add(method);
					}
				}
			}
			for (int overloadIndex = 1; overloadIndex < paramListList.Count; overloadIndex++)
			{
				typeScriptSb.Append("---@overload ");
				typeScriptSb.Append(MethodToString(paramListList[overloadIndex], returnListList[overloadIndex]));
				MethodInfo method = fromMethodList[overloadIndex];
				if (method.IsStatic)
				{
					typeScriptSb.Append(" @static");
				}
				if (method.IsAbstract)
				{
					typeScriptSb.Append(" @abstract");
				}
				else if (method.IsVirtual)
				{
					typeScriptSb.Append(" @virtual");
				}
				typeScriptSb.AppendLine();
			}
			{
				List<ParameterInfo> paramList = paramListList[0];
				List<ParameterInfo> returnList = returnListList[0];
				MethodInfo method = fromMethodList[0];
				if (method.IsStatic)
				{
					typeScriptSb.AppendLine("---@static");
				}
				if (method.IsAbstract)
				{
					typeScriptSb.AppendLine("---@abstract");
				}
				else if (method.IsVirtual)
				{
					typeScriptSb.AppendLine("---@virtual");
				}
				for (int paramIndex = 0; paramIndex < paramList.Count; paramIndex++)
				{
                    ParameterInfo param = paramList[paramIndex];
                    typeScriptSb.Append("---@param ");
					typeScriptSb.Append(param.Name);
					typeScriptSb.Append(" ");
					typeScriptSb.Append(TypeToString(param.ParameterType));
                    if (param.IsDefined(typeof(ParamArrayAttribute), false))
                    {
                        typeScriptSb.Append("|");
                        typeScriptSb.Append(TypeToString(param.ParameterType.GetElementType()));
                    }
					typeScriptSb.AppendLine();
				}
				if (returnList.Count > 0)
				{
					typeScriptSb.Append("---@return ");
					typeScriptSb.Append(TypeToString(returnList[0].ParameterType));
					for (int returnIndex = 1; returnIndex < returnList.Count; returnIndex++)
					{
						typeScriptSb.Append(", ");
						typeScriptSb.Append(TypeToString(returnList[returnIndex].ParameterType));
					}
					typeScriptSb.AppendLine();
				}

				typeScriptSb.Append("function m");
				if (method.IsStatic)
				{
					typeScriptSb.Append(".");
				}
				else
				{
					typeScriptSb.Append(":");
				}
				typeScriptSb.Append(methodName);
				typeScriptSb.Append("(");
				if (paramList.Count > 0)
				{
					typeScriptSb.Append(paramList[0].Name);
				}
				for (int paramIndex = 1; paramIndex < paramList.Count; paramIndex++)
				{
					typeScriptSb.Append(", ");
					typeScriptSb.Append(paramList[paramIndex].Name);
				}
				typeScriptSb.Append(") end");
				typeScriptSb.AppendLine();
			}
		}

		typeScriptSb.AppendLine();
		typeScriptSb.Append(typeName);
		typeScriptSb.AppendLine(" = m");
		typeScriptSb.AppendLine("return m");

		fileName = typeFileName;
		content = typeScriptSb.ToString();
	}

	private static string TypeToString(Type type, bool classDefine = false)
	{
		if (!classDefine)
        {
            if (type == typeof(object))
            {
                return "any";
            }

            if (type == typeof(sbyte) || type == typeof(byte) ||
				type == typeof(short) || type == typeof(ushort) ||
				type == typeof(int) || type == typeof(uint) ||
				type == typeof(long) || type == typeof(ulong) ||
				type == typeof(float) || type == typeof(double) ||
				type == typeof(char))
			{
				return "number";
			}

			if (type == typeof(string) || type == typeof(byte[]))
			{
				return "string";
			}

			if (type == typeof(bool))
			{
				return "boolean";
			}

			if (type.IsArray)
			{
				return TypeToString(type.GetElementType()) + "[]";
            }

            if (type.IsGenericType)
            {
                Type[] genericArgTypes = type.GetGenericArguments();
                if (genericArgTypes.Length == 1 && typeof(IList<>).MakeGenericType(genericArgTypes).IsAssignableFrom(type))
                {
                    return TypeToString(genericArgTypes[0]) + "[]";
                }

                if (genericArgTypes.Length == 2 && typeof(IDictionary<,>).MakeGenericType(genericArgTypes).IsAssignableFrom(type))
                {
                    if (genericArgTypes[0] != typeof(string))
                    {
                        return "table<" + TypeToString(genericArgTypes[0]) + ", " + TypeToString(genericArgTypes[1]) + ">";
                    }
                }
            }

			if (typeof(Delegate).IsAssignableFrom(type))
			{
				MethodInfo method = type == typeof(Delegate) || type == typeof(MulticastDelegate) ?
					type.GetMethod("DynamicInvoke") : type.GetMethod("Invoke");
				return MethodToString(method);
			}
		}

		if (type.FullName == null)
		{
			//GenericTypeDefinition like T
			return TypeToString(type.BaseType ?? typeof(object));
		}

        char[] typeNameChars = type.ToString().ToCharArray();
        StringBuilder sb = new StringBuilder();
        int brackets = 0;
        for (int index = 0; index < typeNameChars.Length; index++)
        {
            // Generic: “`[,]”，ByRef：“&”，Nested：“+”，Other：“<>$=”
            // We want no “.” in “[]” or "<>"
            char c = typeNameChars[index];
            if (c == '[' || c == '<')
            {
                brackets++;
                c = '_';
            }
            else if (c == ']' || c == '>')
            {
                brackets--;
                c = '_';
            }
            else if (c == '.' || c == '+')
            {
                if (brackets > 0)
                {
                    c = '_';
                }
                else
                {
                    c = '.';
                }
            }
            else if (c == '`' || c == ',' || c == '$' || c == '=')
            {
                c = '_';
            }
            if (c != '&')
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
	}

	private static string MethodToString(MethodInfo method)
	{
		if (method == null)
		{
			return "any";
		}

		List<ParameterInfo> paramList = new List<ParameterInfo>();
		List<ParameterInfo> returnList = new List<ParameterInfo>();
		if (method.ReturnParameter.ParameterType != typeof(void))
		{
			returnList.Add(method.ReturnParameter);
		}
		ParameterInfo[] parameters = method.GetParameters();
		for (int parameterIndex = 0; parameterIndex < parameters.Length; parameterIndex++)
		{
			ParameterInfo param = parameters[parameterIndex];
			if (!param.IsOut)
			{
				// !out
				paramList.Add(param);
			}
			if (param.ParameterType.IsByRef)
			{
				// out | ref
				returnList.Add(param);
			}
		}

		return MethodToString(paramList, returnList);
	}

	private static string MethodToString(List<ParameterInfo> paramList, List<ParameterInfo> returnList)
	{
		StringBuilder sb = new StringBuilder();
		sb.Append("fun(");
		if (paramList.Count > 0)
		{
			sb.Append(paramList[0].Name);
			sb.Append(":");
			sb.Append(ParamTypeToString(paramList[0].ParameterType));
		}
		for (int paramIndex = 1; paramIndex < paramList.Count; paramIndex++)
        {
            ParameterInfo param = paramList[paramIndex];
            sb.Append(", ");
			sb.Append(param.Name);
			sb.Append(":");
			sb.Append(ParamTypeToString(param.ParameterType));
            if (param.IsDefined(typeof(ParamArrayAttribute), false))
            {
                sb.Append("|");
                sb.Append(TypeToString(param.ParameterType.GetElementType()));
            }
        }
		sb.Append(")");
		if (returnList.Count > 0)
		{
			sb.Append(":");
			sb.Append(ParamTypeToString(returnList[0].ParameterType));
			for (int returnIndex = 1; returnIndex < returnList.Count; returnIndex++)
			{
				sb.Append(", ");
				sb.Append(ParamTypeToString(returnList[returnIndex].ParameterType));
			}
		}
		return sb.ToString();
	}

	private static string ParamTypeToString(Type paramType)
	{
		StringBuilder sb = new StringBuilder();
		bool isDelegate = typeof(Delegate).IsAssignableFrom(paramType);
		if (isDelegate)
		{
			sb.Append("(");
		}
		sb.Append(TypeToString(paramType));
		if (isDelegate)
		{
			sb.Append(")");
		}
		return sb.ToString();
	}

	private static void ZipDerctory(string zipedDirectory, Dictionary<string, byte[]> fileDict, int compressionLevel = 9)
	{
		FileStream fileStream = File.Create(zipedDirectory);
		ZipOutputStream zipStream = new ZipOutputStream(fileStream);
		zipStream.SetLevel(compressionLevel);
		foreach (string fileName in fileDict.Keys)
		{
			zipStream.PutNextEntry(new ZipEntry(fileName));
			byte[] buffer = fileDict[fileName];
			zipStream.Write(buffer, 0, buffer.Length);
		}
		zipStream.Flush();
		zipStream.Close();
		fileStream.Close();
	}
}
