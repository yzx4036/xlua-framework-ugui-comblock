// Decompiled with JetBrains decompiler
// Type: UnityFunction
// Assembly: Libs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0D761999-E7FD-401B-9784-673A833539CF
// Assembly location: E:\_Proj\UnityProj\client3-clone\TheKingOfTower\Assets\Plugins\Libs.dll

using System;
using System.Collections;
using UnityEngine;

namespace Sword
{
  
  public delegate string GameStringAction();
  public delegate UnityEngine.Object GameUnityObjectAction<in T>(T obj);
  public delegate void GameVoidAction<in T>(T obj);
  public delegate GamePlatform GamePlatformAction();

  public enum GamePlatform
  {
    Windows,
    OSX,
    Android,
    iOS,
  }
  
  public static class UnityFunction
  {
    public static GameStringAction WWWStreamingAssetPath;
    public static GameStringAction GetPlatform;
    public static GameUnityObjectAction<string> LoadEditor;
    public static GameVoidAction<GameObject> DestroySelf;
    public static GameVoidAction<UnityEngine.Object> DestoryObject;
    public static GamePlatformAction RunPlatform;
    public static GameStringAction Compress;

    public static Coroutine InvokeStart(MonoBehaviour initiator, IEnumerator routine)
    {
      try
      {
        return initiator.StartCoroutine(routine);
      }
      catch (Exception ex)
      {
        Debug.LogException(ex);
        return (Coroutine) null;
      }
    }
  }
}
