using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Debug = UnityEngine.Debug;

/// <summary>
/// modify by E_Ye @ 2019.5.28
/// 说明：此处xlsx生成lua 以及proto 生成lua配置工具
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
        maxSize = new Vector2(600, 320);
//        minSize = maxSize;
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
        GUILayout.Label("Generate tools path : ", EditorStyles.boldLabel, GUILayout.Width(150));
        _toolRootPath = GUILayout.TextField(_toolRootPath, GUILayout.Width(400));
        if (GUILayout.Button("...", GUILayout.Width(30)))
        {
            SelectToolFolder();
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Lua output path : ", EditorStyles.boldLabel, GUILayout.Width(150));
        _luaOutPutFolder = GUILayout.TextField(_luaOutPutFolder, GUILayout.Width(400));
        if (GUILayout.Button("...", GUILayout.Width(30)))
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

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Space(100);
        if (GUILayout.Button("Xlsx Gen Lua", GUILayout.Width(150)))
        {
            XlsxGenLua();
        }
        GUILayout.Space(100);
        if (GUILayout.Button("Gen ConfigCfgManager", GUILayout.Width(150)))
        {
            Gen2ConfigCfgManager();
        }

        
        GUILayout.EndHorizontal();
        
        foldOutClick = EditorGUILayout.Foldout(foldOutClick, "帮助"); // 定义折叠菜单
        if (foldOutClick)
        {
            EditorGUILayout.HelpBox("上方设置的路径为生成配置lua文件的存放,根据项目自身目录设置！", MessageType.Info); // 显示一个提示框
            EditorGUILayout.HelpBox("Gen ConfigCfgManager功能自动遍历lua配置表路径下的lua文件生成", MessageType.Info); // 显示一个提示框
        }
        
        GUILayout.EndVertical();

        

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
        p.StartInfo.FileName = _toolRootPath + "/" + genBatName;
        p.StartInfo.Arguments = _luaOutPutFolder;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.CreateNoWindow = false;
        p.Start();
    }

    /// <summary>
    /// 生成ConfigCfgManager获取配置接口
    /// </summary>
    private void Gen2ConfigCfgManager()
    {
        Logger.LogColor(Color.green,  ">>>Generated ConfigCfgManager start ");
        StringBuilder sb = new StringBuilder();
        string mgrContent = null;
        string machContent = null;
        string configMgrPath = string.Format("{0}/../ConfigCfgManager.lua", _luaOutPutFolder);
        if (string.IsNullOrEmpty(_luaOutPutFolder))
        {
            Debug.LogError("路径为空，请选择lua路径");
            return;
        }

        var luaFiles = UtilEditorUI.FindFiles(_luaOutPutFolder, "*.lua", true);
        if (luaFiles.Length > 0)
        {
            string genPatt = @"---Auto Generated Start---([\s\S]*).*?---Auto Generated End---";
            mgrContent = File.ReadAllText(configMgrPath, Encoding.Default);
            Match mc = Regex.Match(mgrContent, genPatt);
            machContent = mc.Value;
            Debug.LogError(mc.Value);
        }

        if (string.IsNullOrEmpty(machContent))
        {
            Debug.LogError(">>>>>找不到匹配的---Generated start--- 生成区块， 本次生成ConfigCfgManager失败，请检查");
            return;
        }

        sb.Clear();
        sb.AppendLine("---Auto Generated Start---");
        foreach (var luaFile in luaFiles)
        {
            var tempFileName = luaFile.Replace(@"\","");
            tempFileName = tempFileName.Replace(".lua", "");

            GenEveryFileFromName(ref sb, tempFileName);
        }
        sb.AppendLine("---Auto Generated End---");
        mgrContent = mgrContent.Replace(machContent, sb.ToString());
        
        File.WriteAllText(configMgrPath, mgrContent, Encoding.Default);
        Logger.LogColor(Color.green,  ">>>Generated ConfigCfgManager finish !!");
    }

    private void GenEveryFileFromName(ref StringBuilder sb, string fileName)
    {
        string helperName = fileName.Replace("Table", "Helper");
        string functionNmae = helperName.Replace("Helper", "");
        functionNmae = functionNmae.Replace("Cfg", "Get");

        sb.AppendFormat("\n---@return {0}\n", helperName);
        sb.AppendFormat("function ConfigCfgManager:{0}()\n", functionNmae);
        sb.AppendFormat("\treturn reload('Config.Data.{0}')\n", fileName);
        sb.AppendFormat("end\n");
    }

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