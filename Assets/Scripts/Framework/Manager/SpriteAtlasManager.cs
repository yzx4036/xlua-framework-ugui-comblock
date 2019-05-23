using System;
using System.Collections;
using System.Collections.Generic;
using AssetBundles;
using UnityEngine;
using UnityEngine.U2D;

namespace Sword
{
    public class SpriteAtlasManager :MonoSingleton<SpriteAtlasManager>
    {
        private Dictionary<string, Action<SpriteAtlas>> spriteAtlasFlagDict = null;
        private Dictionary<string, SpriteAtlas> spriteAtlasDict = null;
        private bool IsResetting { get; set; }

        private int resttingCount;

        void Start()
        {
            spriteAtlasDict = new  Dictionary<string, SpriteAtlas>();
            spriteAtlasFlagDict = new Dictionary<string, Action<SpriteAtlas>>();
        }

        protected override void Init()
        {
            UnityEngine.U2D.SpriteAtlasManager.atlasRegistered += OnAtlasRegistered;
            UnityEngine.U2D.SpriteAtlasManager.atlasRequested += OnAtlasRequested;
            Logger.LogColor(Color.magenta,  "<<>><SpriteAtlasManager Init");
        }
        
        private void OnAtlasRegistered(SpriteAtlas spriteAtlas)
        {
            Logger.LogColor(Color.magenta,  "<<>><OnAtlasRegistered spriteAtlas {0} ",spriteAtlas.name);

            if(!spriteAtlasDict.ContainsKey(spriteAtlas.name))
                spriteAtlasDict[spriteAtlas.name] = spriteAtlas;
            
            if (IsResetting)
            {
                resttingCount -= 1;
                Logger.LogColor(Color.magenta,  "<<>><OnAtlasRegistered spriteAtlas {0} _resttingCount {1}",spriteAtlas.name, resttingCount);
            }
        }

        private void OnAtlasRequested(string tag, Action<SpriteAtlas> action)
        {
            if (!spriteAtlasDict.ContainsKey(tag))
            {
                Logger.LogColor(Color.magenta,  "<<>><SpriteAtlasManager OnAtlasRequested {0}", tag);
                StartCoroutine(DoLoadAsset(action, tag));
                spriteAtlasFlagDict[tag] = action;
            }
        }

        private IEnumerator DoLoadAsset(Action<SpriteAtlas> action, string atlasName)
        {
            var start = DateTime.Now;
            string path = $"{AssetBundleConfig.AtlasRoot}{atlasName}.spriteatlas";
            
#if UNITY_EDITOR
            if (AssetBundleConfig.IsSimulateMode)
                yield return new WaitUntil(() => AssetBundleManager.Instance.IsInitialized);
#else
            yield return new WaitUntil(() => AssetBundleManager.Instance.IsInitialized);
#endif
            
            var loader = AssetBundleManager.Instance.LoadAssetAsync(path, typeof(SpriteAtlas));
            yield return loader;

            if (loader != null)
            {
                var  spriteAtlas = loader.asset as SpriteAtlas;
                loader.Dispose();
                if (spriteAtlas == null)
                {
                    Logger.LogError("SpriteAtlasManager LoadAssetAsync spriteAtlas err : {0}", atlasName);
                    yield break;
                }
                action(spriteAtlas);
                Logger.LogColor(Color.yellow,"SpriteAtlasManager Load SpriteAtlas : {0} use {1}ms", path, (DateTime.Now - start).Milliseconds);
            }
        }

        public IEnumerator Reset()
        {
            IsResetting = true;
            Logger.LogColor(Color.red,">>>>SpriteAtlasManager Reset<<<<<  cache count {0} ", AssetBundleManager.Instance.assetbundlesCaching.Count);

            foreach (var spriteAtlasItem in spriteAtlasFlagDict)
            {
                if (!spriteAtlasDict.ContainsKey(spriteAtlasItem.Key))
                {
                    resttingCount += 1;
                    Logger.LogColor(Color.magenta, "<<>><SpriteAtlasManager Reset {0}", spriteAtlasItem.Key);
                    StartCoroutine(DoLoadAsset(spriteAtlasItem.Value, spriteAtlasItem.Key));
                }

            }

            //在更新前如果请求了图集但因找不到 assetbundle 没填充完成，这里等待填充计数全部请求填充完成， 
            //未填充前
            yield return new WaitUntil(() => resttingCount <= 0);
            IsResetting = false;
            Logger.LogColor(Color.red,">>>>SpriteAtlasManager Reset<<<<< finish");
            Dispose();
            Init();
            yield break;
        }

        public override void Dispose()
        {
            resttingCount = 0;
            UnityEngine.U2D.SpriteAtlasManager.atlasRequested -= OnAtlasRequested;
            UnityEngine.U2D.SpriteAtlasManager.atlasRegistered -= OnAtlasRegistered;
            spriteAtlasFlagDict.Clear();
        }
    }
}