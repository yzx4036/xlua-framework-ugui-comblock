using System;
using System.Collections;
using AssetBundles;
using UnityEngine;
using UnityEngine.U2D;

namespace Sword
{
    public class SpriteAtlasManager :MonoSingleton<SpriteAtlasManager>
    {
        private bool mIsInit = false;

        protected override void Init()
        {
            UnityEngine.U2D.SpriteAtlasManager.atlasRequested += OnAtlasRequested;
        }

        public void AddAtlasRequested()
        {
        }

        public void OnAtlasRequested(string tag, Action<SpriteAtlas> action)
        {
            StartCoroutine(DoLoadAsset(action, tag));
        }

        public IEnumerator DoLoadAsset(Action<SpriteAtlas> action, string atlasName)
        {
            var start = DateTime.Now;
            string path = $"{AssetBundleConfig.AtlasRoot}{atlasName}.spriteatlas";
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

        public override void Dispose()
        {
            UnityEngine.U2D.SpriteAtlasManager.atlasRequested -= OnAtlasRequested;
            mIsInit = false;
        }
    }
}