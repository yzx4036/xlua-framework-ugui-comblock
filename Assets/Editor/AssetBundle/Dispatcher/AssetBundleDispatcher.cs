using UnityEngine;
using UnityEditor;

/// <summary>
/// added by wsh @ 2018.01.03
/// 说明：Assetbundle分发器，用于分发、管理某个目录下的各种Checker
/// 注意：
/// 1、一个分发器可以管理多个Checker，但是所有的这些Checker共享一套配置
/// TODO：
/// 1、提供一套可视化编辑界面，将Dispatcher配置化并展示到Inspector
/// </summary>

namespace AssetBundles
{
    public enum AssetBundleDispatcherFilterType
    {
        Root,
        Children,
        ChildrenFoldersOnly,
        ChildrenFilesOnly,
        FairyGUI, //fairygui包需特殊处理，分别打desc_bundle res_bundle
    }

    public class AssetBundleDispatcher
    {
        string assetsPath;
        AssetBundleImporter importer;
        AssetBundleDispatcherConfig config;

        public AssetBundleDispatcher(AssetBundleDispatcherConfig config)
        {
            this.config = config;
            assetsPath = AssetBundleUtility.PackagePathToAssetsPath(config.PackagePath);
            importer = AssetBundleImporter.GetAtPath(assetsPath);
            if (importer == null)
            {
                Debug.LogError("Asset path err : " + assetsPath);
            }
        }

        public void RunCheckers(bool checkChannel)
        {
            switch (config.Type)
            {
                case AssetBundleDispatcherFilterType.Root:
                    CheckRoot(checkChannel);
                    break;
                case AssetBundleDispatcherFilterType.Children:
                case AssetBundleDispatcherFilterType.ChildrenFoldersOnly:
                case AssetBundleDispatcherFilterType.ChildrenFilesOnly:
                    CheckChildren(checkChannel);
                    break;
                case AssetBundleDispatcherFilterType.FairyGUI:
                    CheckFairyGUISource(checkChannel);
                    break;
            }
        }

        void CheckRoot(bool checkChannel)
        {
            var checkerConfig = new AssetBundleCheckerConfig(config.PackagePath, config.CheckerFilters);
            AssetBundleChecker.Run(checkerConfig, checkChannel);
        }

        void CheckChildren(bool checkChannel)
        {
            var childrenImporters = importer.GetChildren();
            var checkerConfig = new AssetBundleCheckerConfig();
            foreach (var childrenImport in childrenImporters)
            {
                if (config.Type == AssetBundleDispatcherFilterType.ChildrenFilesOnly && !childrenImport.IsFile)
                {
                    continue;
                }
                else if (config.Type == AssetBundleDispatcherFilterType.ChildrenFoldersOnly && childrenImport.IsFile)
                {
                    continue;
                }

                checkerConfig.CheckerFilters = config.CheckerFilters;
                checkerConfig.PackagePath = childrenImport.packagePath;
                AssetBundleChecker.Run(checkerConfig, checkChannel);
            }
        }

        void CheckFairyGUISource(bool checkChannel)
        {
            var childrenImporters = importer.GetChildren();
            var checkerConfig = new AssetBundleCheckerConfig();
            foreach (var childrenImport in childrenImporters)
            {
                if (config.Type == AssetBundleDispatcherFilterType.FairyGUI && !childrenImport.IsFile)
                {
                    continue;
                }

                checkerConfig.CheckerFilters = config.CheckerFilters;
                checkerConfig.PackagePath = childrenImport.packagePath;
                AssetBundleChecker.Run(checkerConfig, checkChannel, true);
            }
        }

        public static void Run(AssetBundleDispatcherConfig config, bool checkChannel)
        {
            var dispatcher = new AssetBundleDispatcher(config);
            dispatcher.RunCheckers(checkChannel);
            AssetDatabase.Refresh();
        }
    }
}
