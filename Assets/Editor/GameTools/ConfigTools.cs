using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;

/// <summary>
/// modify by zfc @ 2018.11.16
/// 说明：此处xlsx生成lua 以及proto 生成lua配置工具
/// 如果生成失败 配置下protobuf环境 已经python环境 备注：python版本最好是2 以及安装读取excel库 xlrd
/// </summary>

public class ConfigTools : EditorWindow
{
    private static string _luaOutPutFolder = string.Empty;
    private static string protoFolder = string.Empty;
    private static string _toolRootPath = string.Empty;

    private bool xlsxGenLuaFinished = false;
    private bool protoGenLuaFinished = false;

    private bool foldOutClick;
    private string genBatName = "start.bat";
    
    void OnEnable()
    {
        ReadPath();
    }

    [MenuItem("Tools/LuaConfig")]
    static void Init()
    {
        GetWindow(typeof(ConfigTools));
        ReadPath();
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Tools path : ", EditorStyles.boldLabel, GUILayout.Width(150));
        _toolRootPath = GUILayout.TextField(_toolRootPath, GUILayout.Width(240));
        if (GUILayout.Button("...", GUILayout.Width(40)))
        {
            SelectToolFolder();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Lua output path : ", EditorStyles.boldLabel, GUILayout.Width(150));
        _luaOutPutFolder = GUILayout.TextField(_luaOutPutFolder, GUILayout.Width(240));
        if (GUILayout.Button("...", GUILayout.Width(40)))
        {
            SelectOutputFolder();
        }
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        
        //协议生成暂时不用
//        GUILayout.BeginHorizontal();
//        GUILayout.Label("proto path : ", EditorStyles.boldLabel, GUILayout.Width(80));
//        protoFolder = GUILayout.TextField(protoFolder, GUILayout.Width(240));
//        if (GUILayout.Button("...", GUILayout.Width(40)))
//        {
//            SelectProtoFolder();
//        }
//        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
//        GUILayout.Label("---------------------");
        if (GUILayout.Button("xlsx gen lua", GUILayout.Width(100)))
        {
            XlsxGenLua();
        }
//        GUILayout.Label("---------------------");
        GUILayout.EndHorizontal();
        
        foldOutClick = EditorGUILayout.Foldout(foldOutClick, "帮助"); // 定义折叠菜单
        if (foldOutClick)
        {
            EditorGUILayout.HelpBox("上方设置的路径为生成配置lua文件的存放,根据项目自身目录设置！", MessageType.Info); // 显示一个提示框
        }
        
//        协议生成留着以后
//        GUILayout.Space(20);
//        GUILayout.BeginHorizontal();
//        GUILayout.Label("---------------------");
//        if (GUILayout.Button("proto gen lua", GUILayout.Width(100)))
//        {
//            ProtoGenLua();
//        }
//        GUILayout.Label("---------------------");
//        GUILayout.EndHorizontal();
    }

    private void XlsxGenLua()
    {
        Process p = new Process();
        p.StartInfo.WorkingDirectory = _toolRootPath;
        p.StartInfo.FileName = _toolRootPath+"/"+genBatName;
        p.StartInfo.Arguments = _luaOutPutFolder;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.CreateNoWindow = false;
        p.Start();
    }

//    private void XlsxGenLua()
//    {
//        if (!CheckXlsxPath(luaOutPutFolder))
//        {
//            return;
//        }
//
//        Process p = new Process();
//        p.StartInfo.FileName = @"python";
//        p.StartInfo.Arguments = luaOutPutFolder + "/tools/toconfigs.py";
//        p.StartInfo.UseShellExecute = false;
//        p.StartInfo.RedirectStandardOutput = true;
//        p.StartInfo.RedirectStandardInput = true;
//        p.StartInfo.RedirectStandardError = true;
//        p.StartInfo.CreateNoWindow = true;
//        p.StartInfo.WorkingDirectory = luaOutPutFolder + "/tools";
//        p.Start();
//        p.BeginOutputReadLine();
//        p.OutputDataReceived += new DataReceivedEventHandler((object sender, DataReceivedEventArgs e) =>
//        {
//            if (!string.IsNullOrEmpty(e.Data))
//            {
//                UnityEngine.Debug.Log(e.Data);
//                if (e.Data.Contains("SUCCEEDED"))
//                {
//                    Process pr = sender as Process;
//                    if (pr != null)
//                    {
//                        pr.Close();
//                    }
//                    xlsxGenLuaFinished = true;
//                }
//            }
//        });
//    }
    
    private void ProtoGenLua()
    {
        if (!CheckProtoPath(protoFolder))
        {
            return;
        }

        Process p = new Process();
        p.StartInfo.FileName = protoFolder + "/make_proto.bat";
        p.StartInfo.Arguments = "";
        p.StartInfo.UseShellExecute = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.WorkingDirectory = protoFolder;
        p.Start();
        p.BeginOutputReadLine();
        p.OutputDataReceived += new DataReceivedEventHandler((object sender, DataReceivedEventArgs e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                UnityEngine.Debug.Log(e.Data);
                if (e.Data.Contains("DONE"))
                {
                    Process pr = sender as Process;
                    if (pr != null)
                    {
                        pr.Close();
                    }
                    protoGenLuaFinished = true;
                }
            }
        });
    }
    
    void Update()
    {
        if (protoGenLuaFinished)
        {
            protoGenLuaFinished = false;
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Succee", "Proto gen lua finished!", "Conform");
        }
    }

    private bool CheckProtoPath(string protoPath)
    {
        if (string.IsNullOrEmpty(protoPath))
        {
            return false;
        }

        if (!File.Exists(protoPath + "/make_proto.bat"))
        {
            EditorUtility.DisplayDialog("Error", "Err path :\nNo find ./make_proto.bat", "Conform");
            return false;
        }

        return true;
    }

    private void SelectOutputFolder()
    {
        var outoutPath = EditorUtility.OpenFolderPanel("Select out put folder", "", "");
        _luaOutPutFolder = outoutPath;
        SavePath();
    }
    
    /// <summary>
    /// 选择生成工具目录
    /// </summary>
    private void SelectToolFolder()
    {
        var toolPath = EditorUtility.OpenFolderPanel("Select tool folder", "", "");
        _toolRootPath = toolPath;
        SavePath();
    }

    private void SelectProtoFolder()
    {
        var selProtoPath = EditorUtility.OpenFolderPanel("Select proto folder", "", "");
        if (!CheckProtoPath(selProtoPath))
        {
            return;
        }

        protoFolder = selProtoPath;
        SavePath();
    }

    static private void SavePath()
    {
        EditorPrefs.SetString("luaOutPutFolder", _luaOutPutFolder);
        EditorPrefs.SetString("protoFolder", protoFolder);
        EditorPrefs.SetString("toolRootPath", _toolRootPath);
    }

    static private void ReadPath()
    {
        _luaOutPutFolder = EditorPrefs.GetString("luaOutPutFolder");
        _toolRootPath = EditorPrefs.GetString("toolRootPath");
        protoFolder = EditorPrefs.GetString("protoFolder");
    }
}
