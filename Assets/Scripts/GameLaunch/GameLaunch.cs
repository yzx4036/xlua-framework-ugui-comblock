using UnityEngine;
using System.Collections;
using AssetBundles;
using GameChannel;
using System;
using HedgehogTeam.EasyTouch;
using UnityEngine.UI;
using XLua;

[Hotfix]
[LuaCallCSharp]
public class GameLaunch : MonoBehaviour
{
    public RawImage bootBgRawImg;
    const string launchPrefabPath = "UI/Prefabs/View/UILaunch.prefab";
    const string noticeTipPrefabPath = "UI/Prefabs/Common/UINoticeTip.prefab";
    GameObject launchPrefab;
    GameObject noticeTipPrefab;
    AssetbundleUpdater updater;

    private void Awake()
    {
        LoggerHelper.Instance.Startup();
        // 启动ugui图集管理器
        var start = DateTime.Now;
        Sword.SpriteAtlasManager.Instance.Startup();
        Logger.Log(string.Format("SpriteAtlasManager Init use {0}ms", (DateTime.Now - start).Milliseconds));
    }

    IEnumerator Start ()
    {
#if UNITY_IPHONE
        UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert | UnityEngine.iOS.NotificationType.Badge | UnityEngine.iOS.NotificationType.Sound);
        UnityEngine.iOS.Device.SetNoBackupFlag(Application.persistentDataPath);
#endif
        InitLaunchBootBg();
        // 初始化App版本
        var start = DateTime.Now;
        yield return InitAppVersion();
        Logger.Log(string.Format("InitAppVersion use {0}ms", (DateTime.Now - start).Milliseconds));

        // 初始化渠道
        start = DateTime.Now;
        yield return InitChannel();
        Logger.Log(string.Format("InitChannel use {0}ms", (DateTime.Now - start).Milliseconds));
        
        // 启动资源管理模块
        start = DateTime.Now;
        yield return AssetBundleManager.Instance.Initialize();
        Logger.Log(string.Format("AssetBundleManager Initialize use {0}ms", (DateTime.Now - start).Milliseconds));

        
        // 启动xlua热修复模块
        start = DateTime.Now;
        XLuaManager.Instance.Startup();
        string luaAssetbundleName = XLuaManager.Instance.AssetbundleName;
        AssetBundleManager.Instance.SetAssetBundleResident(luaAssetbundleName, true);
        var abloader = AssetBundleManager.Instance.LoadAssetBundleAsync(luaAssetbundleName);
        yield return abloader;
        abloader.Dispose();
        XLuaManager.Instance.OnInit();
        XLuaManager.Instance.StartHotfix();
        Logger.Log(string.Format("XLuaManager StartHotfix use {0}ms", (DateTime.Now - start).Milliseconds));

        // 初始化UI界面
        yield return InitLaunchPrefab();
        yield return null;
        DeleteLaunchBootBg();
        yield return InitNoticeTipPrefab();

        // 开始更新
        if (updater != null)
        {
            updater.StartCheckUpdate();
        }
        
        // 启动easytouch扩展管理
        start = DateTime.Now;
        Sword.SceneRootManager.instance.Init();
        Sword.EventManager.instance.Init();
        Sword.TouchManager.instance.Init();
        Logger.Log(string.Format("TouchMgr Init use {0}ms", (DateTime.Now - start).Milliseconds));
    }

    IEnumerator InitAppVersion()
    {
        var appVersionRequest = AssetBundleManager.Instance.RequestAssetFileAsync(UtilityBuild.AppVersionFileName);
        yield return appVersionRequest;
        var streamingAppVersion = appVersionRequest.text;
        appVersionRequest.Dispose();

        var appVersionPath = AssetBundleUtility.GetPersistentDataPath(UtilityBuild.AppVersionFileName);
        var persistentAppVersion = UtilityGame.SafeReadAllText(appVersionPath);
        Logger.Log(string.Format("streamingAppVersion = {0}, persistentAppVersion = {1}", streamingAppVersion, persistentAppVersion));

        // 如果persistent目录版本比streamingAssets目录app版本低，说明是大版本覆盖安装，清理过时的缓存
        if (!string.IsNullOrEmpty(persistentAppVersion) && UtilityBuild.CheckIsNewVersion(persistentAppVersion, streamingAppVersion))
        {
            var path = AssetBundleUtility.GetPersistentDataPath();
            UtilityGame.SafeDeleteDir(path);
        }
        UtilityGame.SafeWriteAllText(appVersionPath, streamingAppVersion);
        ChannelManager.instance.appVersion = streamingAppVersion;
        yield break;
    }

    IEnumerator InitChannel()
    {
#if UNITY_EDITOR
        if (AssetBundleConfig.IsEditorMode)
        {
            yield break;
        }
#endif
        var channelNameRequest = AssetBundleManager.Instance.RequestAssetFileAsync(UtilityBuild.ChannelNameFileName);
        yield return channelNameRequest;
        var channelName = channelNameRequest.text;
        channelNameRequest.Dispose();
        ChannelManager.instance.Init(channelName);
        Logger.Log(string.Format("channelName = {0}", channelName));
        yield break;
    }

    GameObject InstantiateGameObject(GameObject prefab)
    {
        var start = DateTime.Now;
        GameObject go = GameObject.Instantiate(prefab);
        Logger.Log(string.Format("Instantiate use {0}ms", (DateTime.Now - start).Milliseconds));

        var luanchLayer = GameObject.Find("UIRoot/LuanchLayer");
        go.transform.SetParent(luanchLayer.transform);
        var rectTransform = go.GetComponent<RectTransform>();
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.localScale = Vector3.one;
        rectTransform.localPosition = Vector3.zero;

        return go;
    }

    IEnumerator InitNoticeTipPrefab()
    {
        var start = DateTime.Now;
        var loader = AssetBundleManager.Instance.LoadAssetAsync(noticeTipPrefabPath, typeof(GameObject));
        yield return loader;
        noticeTipPrefab = loader.asset as GameObject;
        Logger.Log(string.Format("Load noticeTipPrefab use {0}ms", (DateTime.Now - start).Milliseconds));
        loader.Dispose();
        if (noticeTipPrefab == null)
        {
            Logger.LogError("LoadAssetAsync noticeTipPrefab err : " + noticeTipPrefabPath);
            yield break;
        }
        var go = InstantiateGameObject(noticeTipPrefab);
        UINoticeTip.Instance.UIGameObject = go;
        yield break;
    }

    IEnumerator InitLaunchPrefab()
    {
        var start = DateTime.Now;
        var loader = AssetBundleManager.Instance.LoadAssetAsync(launchPrefabPath, typeof(GameObject));
        yield return loader;
        launchPrefab= loader.asset as GameObject;
        Logger.Log(string.Format("Load launchPrefab use {0}ms", (DateTime.Now - start).Milliseconds));
        loader.Dispose();
        if (launchPrefab == null)
        {
            Logger.LogError("LoadAssetAsync launchPrefab err : " + launchPrefabPath);
            yield break;
        }
        var go = InstantiateGameObject(launchPrefab);
        updater = go.AddComponent<AssetbundleUpdater>();
        go.SetActive(false);
        yield return new WaitForEndOfFrame();
        go.SetActive(true);
        yield break;
    }

    void InitLaunchBootBg()
    {
        if (bootBgRawImg != null)
        {
            var index = UnityEngine.Random.Range(0, 2);
            bootBgRawImg.gameObject.SetActive(true);
            bootBgRawImg.texture = Resources.Load<Texture>("Boot/boot" + index);
        }
    }
    
    void DeleteLaunchBootBg()
    {
        EyeSoft.Data.SqlDataHelper<Person>.InsertOneData(new Person()
        {
            Name = "Tom1233232",
            Surname = "Perez222222222222222",
            Age = 56
        }, false);
        if (bootBgRawImg != null)
        {
            Destroy(bootBgRawImg.gameObject);
        }
    }
    
}
