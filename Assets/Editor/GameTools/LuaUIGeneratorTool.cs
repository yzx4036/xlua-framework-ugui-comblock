using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Text;

/// <summary>
/// modify by E_Ye @ 2019.6.12
/// 说明：生成 UI lua MVC代码工具
/// </summary>
public class LuaUiGeneratorTool : EditorWindow
{
    private string _luaOutPutFolder = string.Empty;
    private string uiModuleName = string.Empty; 
    private bool foldOutClick;

    private bool isHasModle;
    private int currentLayerIndex = 1;

    private const string SecondViewFolder = "View";
    private const string SecondCtrlFolder = "Controller";
    private const string SecondModelFolder = "Model";
    
    private const string LastViewFileString = "View";
    private const string LastCtrlFileString = "Ctrl";
    private const string LastModelFileString = "Model";
    private const string LastConfigFileString = "Config";
    
    private const string BaseViewString = "UIBaseView";
    private const string BaseCtrlString = "UIBaseCtrl";
    private const string BaseModelString = "UIBaseModel";

    private const string Extend = ".lua";

    private enum UiLayer
    {
        SceneLayer,
        BackgroundLayer,
        NormalLayer,
        InfoLayer,
        TipLayer,
        TopLayer,
    }

    private Dictionary<UiLayer, string> layerNameDict = null; 
    private List<string> functionNameList = null; 

    void OnEnable()
    {
        maxSize = new Vector2(600, 320);
        ReadPath();
        layerNameDict = new Dictionary<UiLayer, string>()
        {
            [UiLayer.SceneLayer] = "SceneLayer",
            [UiLayer.BackgroundLayer] = "BackgroundLayer",
            [UiLayer.NormalLayer] = "NormalLayer",
            [UiLayer.InfoLayer] = "InfoLayer",
            [UiLayer.TipLayer] = "TipLayer",
            [UiLayer.TopLayer] = "TopLayer",
        };
        
        functionNameList = new List<string>()
        {
            "OnCreate",
            "OnEnable",
            "OnAddListener",
            "OnRemoveListener",
            "OnDisable",
            "OnDestroy",
        };
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Lua output path : ", EditorStyles.boldLabel, GUILayout.Width(120));
        _luaOutPutFolder = GUILayout.TextField(_luaOutPutFolder, GUILayout.Width(400));
        if (GUILayout.Button("...", GUILayout.Width(30)))
        {
            SelectOutputFolder();
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("UI Module name : ", EditorStyles.boldLabel, GUILayout.Width(120));
        uiModuleName = GUILayout.TextField(uiModuleName, GUILayout.Width(200));
        if (GUILayout.Button("Auto select prefab name", GUILayout.Width(150)))
        {
            AutoName();
        }
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        
        currentLayerIndex = EditorGUILayout.Popup("对应的UILayer：", currentLayerIndex, layerNameDict.Values.ToArray(), GUILayout.Width(300));

        isHasModle = EditorGUILayout.Toggle("是否生成UIModel：", isHasModle);
        
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate To Lua", GUILayout.Width(150)))
        {
            GenUi2Lua();
        }

        GUILayout.EndHorizontal();
        
        foldOutClick = EditorGUILayout.Foldout(foldOutClick, "帮助"); // 定义折叠菜单
        if (foldOutClick)
        {
            EditorGUILayout.HelpBox("上方设置的路径为生成uilua文件的存放,根据项目自身目录设置！", MessageType.Info); // 显示一个提示框
        }
        
        GUILayout.EndVertical();
        
    }

    private void GenUi2Lua()
    {
        if (string.IsNullOrEmpty(uiModuleName))
        {
            Logger.LogError(">>>>uiModuleName 为空，请指定对应UI prefab 生成名字");
            return;
        }
        Logger.LogColor(Color.green,  ">>>Generated ui to lua ! start ");
        StringBuilder sb = new StringBuilder();
        string genRootPath = $@"{_luaOutPutFolder}\Temp";
        EyeSoft.UtilFile.CheckDirExists(genRootPath);
        Logger.LogColor(Color.green,  ">>>genRootPath {0} ", genRootPath);
        
        GenUiView(sb, genRootPath, uiModuleName);
        GenUiCtrl(sb, genRootPath, uiModuleName);
        GenUiModel(sb, genRootPath, uiModuleName);
        GenUiConfig(sb, genRootPath, uiModuleName, layerNameDict[(UiLayer) currentLayerIndex]);
        Logger.LogColor(Color.green,  ">>>Generated ui to lua ! end ");

    }

    private void GenUiView(StringBuilder sb, string rootPath, string fileName)
    {
        sb.Clear();
        string realName = $"{fileName}{LastViewFileString}";
        string uiViewFilePath = $@"{rootPath}/{fileName}/{SecondViewFolder}/{realName}{Extend}";
        //todo Gen ui View
        GenFile(ref sb, fileName, realName, SecondViewFolder, BaseViewString);
//        Logger.LogColor(Color.red, sb.ToString());
        EyeSoft.UtilFile.WriteFile(sb.ToString(), uiViewFilePath);
    }
    
    private void GenUiCtrl(StringBuilder sb, string rootPath, string fileName)
    {
        sb.Clear();
        string realName = $"{fileName}{LastCtrlFileString}";
        string uiCtrlFilePath = $@"{rootPath}/{fileName}/{SecondCtrlFolder}/{realName}{Extend}";
        //todo Gen ui View
        GenFileHeader(ref sb, fileName, realName, SecondCtrlFolder, BaseCtrlString);
        GenFileOver(ref sb, realName);
//        Logger.LogColor(Color.red, sb.ToString());
        EyeSoft.UtilFile.WriteFile(sb.ToString(), uiCtrlFilePath);
    }
    
    private void GenUiModel(StringBuilder sb, string rootPath, string fileName)
    {
        if(!isHasModle ) return; 
        
        sb.Clear();
        string realName = $"{fileName}{LastModelFileString}";
        string uiModelFilePath =  $@"{rootPath}/{fileName}/{SecondModelFolder}/{realName}{Extend}";
        //todo
        GenFile(ref sb, fileName, realName, SecondModelFolder, BaseModelString);
//        Logger.LogColor(Color.red, sb.ToString());
        EyeSoft.UtilFile.WriteFile(sb.ToString(), uiModelFilePath);
    }
    
    private void GenUiConfig(StringBuilder sb,  string rootPath, string fileName, string layer)
    {
        sb.Clear();
        string realName = $"{fileName}{LastConfigFileString}";
        string uiConfigFilePath =  $@"{rootPath}/{fileName}/{realName}{Extend}";
        
        string header = @"--
-- Auto Generate by LuaUIGeneratorTool 
-- @yzx {0}
-- {1} 模块窗口配置
--";
        sb.Append(string.Format(header, System.DateTime.Now, fileName));
        sb.AppendFormat("\n\n---@class {0}\n", fileName);
        sb.AppendFormat("local {0} = {1}\n", fileName, "{");
        sb.AppendFormat("\tName = UIWindowNames.{0},\n", fileName);
        sb.AppendFormat("\tLayer = UILayers.{0},\n", layer);
        if (isHasModle)
        {
            sb.AppendFormat("\tModel =  require 'UI.{0}.Model.{1}Model',\n", fileName, fileName);
        }
        else
        {
            sb.AppendFormat("\tModel = nil,\n", layer);
        }
        sb.AppendFormat("\tCtrl =  require 'UI.{0}.Controller.{1}Ctrl',\n", fileName, fileName);
        sb.AppendFormat("\tView =  require 'UI.{0}.View.{1}View',\n", fileName, fileName);
        sb.AppendFormat("\tPrefabPath =  'UI/Prefabs/View/{0}.prefab',\n", fileName);

        
        sb.AppendFormat("{0}\n\n return {1} {2} = {3} {4}", "}", "{", fileName, fileName, "}");
        
        EyeSoft.UtilFile.WriteFile(sb.ToString(), uiConfigFilePath);

    }
    
//    private void GenUiGlobalConfig(StringBuilder sb, string rootPath, string fileName)
//    {
//        sb.Clear();
//        string uiGlobalConfigFilePath = $@"{rootPath}\{fileName}";
//     
//        
//        Sword.UtilFile.WriteFile(sb.ToString(), uiGlobalConfigFilePath);
//    }

    private void GenFile(ref StringBuilder sb, string fileName, string realName, string secondFolder, string baseName)
    {
        GenFileHeader(ref sb, fileName, realName, secondFolder, baseName);
        foreach (string functionName in functionNameList)
        {
            if(functionName == "OnCreate" && baseName == BaseViewString)
                GenFileOnCreateFunction(ref sb, realName, fileName);
            else
                GenFileFunction(ref sb, realName, functionName);
        }
        GenFileOver(ref sb, realName);
    }

    private void GenFileHeader(ref StringBuilder sb, string fileName, string realName, string secondFolder, string baseName)
    {
        string header = @"--
-- Auto Generate by LuaUIGeneratorTool 
-- @yzx {0}
-- {1} {2}层
--";
        sb.Append(string.Format(header, System.DateTime.Now, fileName, secondFolder));
        sb.AppendFormat("\n\n---@class {0}:{1}\n", realName, baseName);
        sb.AppendFormat("local {0} = BaseClass('{1}', {2})\n", realName, realName, baseName);
        sb.AppendFormat("\nlocal base = {0}\n", baseName);
    }

    private void GenFileOnCreateFunction(ref StringBuilder sb, string realName, string fileName)
    {
        //OnCreate 方法
        sb.AppendFormat("\nfunction {0}:OnCreate()\n",realName);
        sb.AppendFormat("\tbase.OnCreate(self)\n");
        sb.AppendFormat("\t---@type {0}Ctrl\n\tself.ctrl = self.ctrl\n", fileName);
        sb.AppendFormat("\nend\n");
    }
    
    private void GenFileFunction(ref StringBuilder sb, string realName, string functionName)
    {
        sb.AppendFormat("\nfunction {0}:{1}()\n",realName, functionName);
        sb.AppendFormat("\tbase.{0}(self)\n", functionName);
        sb.AppendFormat("\nend\n");
    }
    
    private void GenFileOver(ref StringBuilder sb, string realName)
    {
        sb.AppendFormat("\nreturn {0}\n", realName);
    }

    private void SelectOutputFolder()
    {
        var outoutPath = EditorUtility.OpenFolderPanel("Select out put folder", "", "");
        _luaOutPutFolder = outoutPath;
        SavePath();
    }
    
    private void AutoName()
    {
        if (Selection.activeObject != null)
        {
            uiModuleName = Selection.activeObject.name;
        }
        else
        {
            Logger.LogError(">>> 没有选中对象");
        }
    }

    private void SavePath()
    {
        EditorPrefs.SetString("uiLuaOutPutFolder", _luaOutPutFolder);
    }

    private void ReadPath()
    {
        _luaOutPutFolder = EditorPrefs.GetString("uiLuaOutPutFolder");
    }
}