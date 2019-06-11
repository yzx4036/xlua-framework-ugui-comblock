using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;
using Sword;

public static class UtilEditorUI
{
    public const string UIPath = "Assets/AssetsPackage/Prefabs";


    [MenuItem("Tools/UI/修改所有的shader")]
    public static void SetAllShader()
    {
        string path = "Assets/";
        string[] files = Directory.GetFiles(path, "*.shader", SearchOption.AllDirectories);
        Dictionary<string, Shader> shaders = new Dictionary<string, Shader>();
        foreach (var t in files)
        {
            Shader s = AssetDatabase.LoadAssetAtPath<Shader>(t);
            shaders[s.name] = s;
        }

        files = Directory.GetFiles(path, "*.mat", SearchOption.AllDirectories);
        foreach (var t in files)
        {
            Material s = AssetDatabase.LoadAssetAtPath<Material>(t);
            if (s.shader != null)
            {
                if (shaders.ContainsKey(s.shader.name))
                {
                    s.shader = shaders[s.shader.name];
                    EditorUtility.SetDirty(s);
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("finish");
    }

    [MenuItem("Tools/AssetBundle/ab资源问题---修复重名资源")]
    public static void SetRepairSameName()
    {
        //FixSameName(".mat");
        FixSameName(".fbx");
        FixSameName(".png");

        //FixSpaceName(".mat", "_");
    }

    private static void FixSameName(string type)
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        string path = "Assets/";
        string[] files = Directory.GetFiles(path, "*" + type, SearchOption.AllDirectories);
        foreach (var t in files)
        {
            if (t.Contains("AtlasSource"))
            {
                continue;
            }

            if (t.Contains("Font"))
            {
                continue;
            }

            FileInfo info = new FileInfo(t);
            if (dic.ContainsKey(info.Name))
            {
                //Material s = AssetDatabase.LoadAssetAtPath<Material>(t);
                string old = t;
                if (old.Contains("Resources"))
                {
                    string newName = t.Replace(type, "_" + type.Replace(".", "") + "_fy" + type);
                    AssetDatabase.MoveAsset(old, newName);
                    //Debug.LogError(old + " " + newName);
                }
            }

            dic[info.Name] = info.Name;
        }
    }


    [MenuItem("Tools/AssetBundle/ab资源问题---粒子异常")]
    public static void CheckParticleError()
    {
        string path = "Assets/";
        string[] files = Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories);
        foreach (var t in files)
        {
            GameObject go = AssetDatabase.LoadAssetAtPath(t, typeof(GameObject)) as GameObject;
            if (go != null)
            {
                ParticleSystemRenderer[] temps = go.GetComponentsInChildren<ParticleSystemRenderer>();
                foreach (var ta in temps)
                {
                    if (ta.renderMode != ParticleSystemRenderMode.Mesh)
                    {
                        if (ta.mesh != null)
                        {
                            ta.mesh = null;
                            EditorUtility.SetDirty(go);
                            AssetDatabase.SaveAssets();
                        }
                    }
                    else
                    {
                        if (ta.mesh != null)
                        {
                            string meshPath = AssetDatabase.GetAssetOrScenePath(ta.mesh);
                            UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetsAtPath(meshPath);
                            if (objs != null)
                            {
                                int count = 0;
                                foreach (var o in objs)
                                {
                                    if (o is Mesh)
                                    {
                                        count++;
                                    }
                                }

                                if (count > 1)
                                {
                                    Debug.LogError(t + " " + count + " fbx 里面多个mesh " + meshPath);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    [MenuItem("Tools/UI/查找未使用prefab资源")]
    public static void FindNoUsedPrefabs()
    {
        Debug.ClearDeveloperConsole();
        Debug.LogError(">>>>正在查找未使用prefab资源<<<");
        Debug.Log(">>查找位置：Lua/Module中的所有lua文件，ModuleConfig.txt，和Resources/UI下的prefab名字比对 <<<");

        {
            List<string> paths = new List<string>();
            GetAllUsePrefabDictionary();
            string[] prefabNames = LookForPrefab();
            int totalCount = 0;
            for (int i = 0; i < prefabNames.Length; i++)
            {
                if (!usingDict.ContainsKey(prefabNames[i].GetHashCode()))
                {
                    totalCount++;
                    Debug.Log(prefabNames[i]);

                    paths.Add(prefabNames[i]);
                }
            }

            foreach (var t in paths)
            {
                string path = Application.dataPath + "/Resources/" + t;
                Debug.LogError("del " + path);
                UtilFile.DeleteFile(path);
            }

            Debug.LogError(">>>>查找结束，共找到" + totalCount + "个未使用资源<<<");
            Debug.LogError(@">>>>资源谨慎操作， 移到Assets\ArtTower\ui_old_prefab文件夹中，避免误删");
            usingDict.Clear();
        }
    }


    public static Dictionary<int, string> usingDict = new Dictionary<int, string>();

    public static void GetAllUsePrefabDictionary()
    {
        usingDict.Clear();
        string script = "";

        //lua
        string path = @"Assets\..\LuaScripts\";
        string[] files = UtilEditorUI.FindFiles(path, "*.lua");
        for (int i = 0; i < files.Length; i++)
        {
            script = File.ReadAllText(files[i]);
            CheckLuaContent(script);
        }

        //C#
        path = @"Assets\Scripts";
        files = UtilEditorUI.FindFiles(path, "*.cs");
        for (int i = 0; i < files.Length; i++)
        {
            script = File.ReadAllText(files[i]);
            CheckLuaContent(script);
        }
    }

    public static void CheckLuaContent(string content)
    {
        string rexMode1 = @"UI/.*?\.prefab";
        MatchCollection mc = Regex.Matches(content, rexMode1);
        foreach (Match match in mc)
        {
            string value = match.Value.Trim();
            int nameHash = value.GetHashCode();
            //Debug.Log(match.Value);
            if (!usingDict.ContainsKey(nameHash))
            {
                usingDict.Add(nameHash, value.Trim());
            }
        }
    }

    public static string[] LookForPrefab()
    {
        string path = UIPath;
        string[] files = UtilEditorUI.FindFiles(path, "*.prefab");
        string rexMode1 = @"UI/.*?\.prefab";
        string[] prefabNames = new string[files.Length];
        for (int i = 0; i < files.Length; i++)
        {
            files[i] = files[i].Replace(@"\", @"/"); //把路径的斜杠替换
            Match mc = Regex.Match(files[i], rexMode1);
            prefabNames[i] = mc.Value.Trim();
        }

        return prefabNames;
    }

    public static string[] FindFiles(string path, string extensionStr, bool isGetShortName = false)
    {
        string[] files = Directory.GetFiles(path, extensionStr, SearchOption.AllDirectories);
        
        if (isGetShortName)
        {
            for (int i = 0; i < files.Length ; i++)
            {
                string temp = files[i];
                files[i] = temp.Replace(path, "");
            }
        }

        return files;
    }

    [MenuItem("Tools/UI/PrintAllPng")]
    public static void PrintAllPng()
    {
        var temp = Selection.activeGameObject;
        if (temp != null)
        {
            List<string> names = new List<string>();
            var images = temp.GetComponentsInChildren<Image>();
            foreach (var t in images)
            {
                if (t.material != null)
                {
                    string name = t.material.name;
                    if (!names.Contains(name))
                    {
                        names.Add(name);
                    }
                }
            }

            foreach (var t in names)
            {
                Debug.LogError(t);
            }
        }
    }

    public static void GetShortFileNameFromPathString(string pathString)
    {
    }

    #region EditorWindows Menu

    [MenuItem("Tools/LuaUIGeneratorTool")]
    public static void OpenLuaUIGeneratorTool()
    {
        EditorWindow.GetWindow(typeof(LuaUiGeneratorTool));
    }

    #endregion
}