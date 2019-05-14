using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Sword
{
  public static class UtilityGameObject
  {
    public static GameObject Clone(this GameObject prefab, string name)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab);
      gameObject.name = prefab.name;
      if (name != null)
        gameObject.name = name;
      return gameObject;
    }

    public static GameObject CloneWithInitPos(
      this GameObject prefab,
      Vector3 pos,
      string name)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, pos, Quaternion.identity);
      gameObject.name = prefab.name;
      if (name != null)
        gameObject.name = name;
      return gameObject;
    }

    public static GameObject Clone(this GameObject prefab, string name, Transform parent)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, parent);
      gameObject.name = prefab.name;
      if (name != null)
        gameObject.name = name;
      return gameObject;
    }

    public static GameObject AddNewChildTo(this GameObject parent, string childName = null)
    {
      return parent.AddChildToParent(new GameObject(), childName, false, true);
    }

    public static T AddPrefabChildTo<T>(
      this GameObject parent,
      GameObject prefab,
      string childName)
      where T : Component
    {
      return parent.AddPrefabChildTo(prefab, childName).AddComponent<T>();
    }

    public static void ClearChildren(GameObject go)
    {
      for (int index = 0; index < go.transform.childCount; ++index)
      {
        Transform child = go.transform.GetChild(index);
        if ((bool) ((UnityEngine.Object) child))
          child.gameObject.DestroySelf();
      }
      go.transform.DetachChildren();
    }

    public static void CopyLocalTransform(GameObject src, GameObject dst)
    {
      Transform transform1 = src.transform;
      Transform transform2 = dst.transform;
      transform2.localPosition = transform1.localPosition;
      transform2.localRotation = transform1.localRotation;
      transform1.localScale = transform1.localScale;
    }

    public static void Reset(GameObject gameObject)
    {
      Transform transform = gameObject.transform;
      transform.localPosition = Vector3.zero;
      transform.localRotation = Quaternion.identity;
      transform.localScale = Vector3.one;
    }

    public static GameObject GetChild(this GameObject root, string name)
    {
      GameObject gameObject = (GameObject) null;
      if ((UnityEngine.Object) root != (UnityEngine.Object) null)
      {
        Transform transform = root.transform.Find(name);
        if ((UnityEngine.Object) transform != (UnityEngine.Object) null)
          gameObject = transform.gameObject;
      }
      return gameObject;
    }

    public static T GetChild<T>(this GameObject root, string name) where T : Component
    {
      GameObject child = root.GetChild(name);
      if (!(bool) ((UnityEngine.Object) child))
        return default (T);
      return child.GetComponent<T>();
    }

    public static GameObject FindChild(this GameObject root, string name)
    {
      if ((UnityEngine.Object) root == (UnityEngine.Object) null)
        return (GameObject) null;
      GameObject child1 = root.GetChild(name);
      if ((UnityEngine.Object) child1 != (UnityEngine.Object) null)
        return child1;
      int childCount = root.transform.childCount;
      for (int index = 0; index < childCount; ++index)
      {
        GameObject child2 = root.transform.GetChild(index).gameObject.FindChild(name);
        if ((UnityEngine.Object) child2 != (UnityEngine.Object) null)
          return child2;
      }
      return (GameObject) null;
    }

    public static T FindChild<T>(this GameObject root, string name) where T : Component
    {
      GameObject child = root.FindChild(name);
      if ((UnityEngine.Object) child == (UnityEngine.Object) null)
        return default (T);
      return child.GetComponent<T>();
    }

    public static GameObject AddMissingChild(this GameObject parent, string childName)
    {
      GameObject gameObject = parent.GetChild(childName);
      if (!(bool) ((UnityEngine.Object) gameObject))
        gameObject = parent.AddChildToParent(new GameObject(), childName, false, true);
      return gameObject;
    }

    public static T AddChildWithComp<T>(this GameObject parent, GameObject child, string childName = null) where T : Component
    {
      return parent.AddChildToParent(child, childName, false, true).AddComponent<T>();
    }

    public static T AddComp2Child<T>(this GameObject parent, string childName) where T : Component
    {
      return parent.GetChild(childName).AddComponent<T>();
    }

    public static void ChangeLayerRecursively(this Transform root, int layer)
    {
      root.gameObject.layer = layer;
      if (root.childCount <= 0)
        return;
      for (int index = 0; index < root.transform.childCount; ++index)
        root.transform.GetChild(index).ChangeLayerRecursively(layer);
    }

    public static GameObject GetChild(this GameObject root, int index)
    {
      if ((UnityEngine.Object) root != (UnityEngine.Object) null && root.transform.childCount > index)
      {
        Transform child = root.transform.GetChild(index);
        if ((UnityEngine.Object) child != (UnityEngine.Object) null)
          return child.gameObject;
      }
      return (GameObject) null;
    }

    public static GameObject GetChildByIndex(this GameObject go, int index)
    {
      return go.GetChild(index);
    }

    public static T GetChild<T>(this GameObject root, int index) where T : Component
    {
      GameObject child = root.GetChild(index);
      if ((UnityEngine.Object) child != (UnityEngine.Object) null)
        return child.GetComponent<T>();
      return default (T);
    }

    public static void RemoveComponent<T>(this GameObject go) where T : Component
    {
      if ((UnityEngine.Object) go == (UnityEngine.Object) null)
        return;
      T component = go.GetComponent<T>();
      if (!(bool) ((UnityEngine.Object) component))
        return;
      UnityFunction.DestoryObject((UnityEngine.Object) component);
    }

    public static void RemoveComponent(this GameObject go, System.Type type)
    {
      UnityFunction.DestoryObject((UnityEngine.Object) go.GetComponent(type));
    }

    public static void RemoveComponent(this Component comp)
    {
      UnityFunction.DestoryObject((UnityEngine.Object) comp);
    }

    public static void DestroySelf(this GameObject obj)
    {
      UnityFunction.DestroySelf(obj);
    }

    public static void CreateOrDestroy<T>(this GameObject go, ref T comp, bool isCreate) where T : MonoBehaviour
    {
      if (isCreate)
      {
        if (!((UnityEngine.Object) comp == (UnityEngine.Object) null))
          return;
        comp = go.AddComponent<T>();
      }
      else
      {
        if (!(bool) ((UnityEngine.Object) comp))
          return;
        UnityFunction.DestoryObject((UnityEngine.Object) comp);
      }
    }

    public static GameObject[] GetChilds(this GameObject root)
    {
      int childCount = root.transform.childCount;
      GameObject[] gameObjectArray = new GameObject[childCount];
      for (int index = 0; index < childCount; ++index)
      {
        Transform child = root.transform.GetChild(index);
        gameObjectArray[index] = child.gameObject;
      }
      return gameObjectArray;
    }

    public static void RemoveAllChildren(this GameObject go)
    {
      for (int index = go.transform.childCount - 1; index >= 0; --index)
        go.transform.GetChild(index).gameObject.DestroySelf();
    }

    public static GameObject AddChildToParent(
      this GameObject parent,
      GameObject child,
      string childName = null,
      bool worldPositionStays = false,
      bool isUpdateLayer = true)
    {
      Vector3 localEulerAngles = child.transform.localEulerAngles;
      child.transform.SetParent(parent.transform, false);
      child.transform.localScale = Vector3.one;
      child.transform.localEulerAngles = localEulerAngles;
      if (childName != null)
        child.name = childName;
      if (isUpdateLayer)
        child.layer = parent.layer;
      return child;
    }

    public static GameObject AddPrefabChildTo(
      this GameObject parent,
      GameObject prefab,
      string childName)
    {
      return parent.AddChildToParent(prefab.Clone((string) null), childName, false, true);
    }

    public static GameObject AddChild(
      this GameObject parent,
      GameObject child,
      bool worldPositionStays,
      string childName)
    {
      child.transform.SetParent(parent.transform, worldPositionStays);
      if (childName != null)
        child.name = childName;
      return child;
    }

    public static GameObject AddPrefab(
      this GameObject parent,
      GameObject prefab,
      bool worldPositionStays,
      string childName)
    {
      return parent.AddChild(prefab.Clone((string) null), worldPositionStays, childName);
    }

    public static GameObject AddNewChild(this GameObject parent, string childName = null)
    {
      return parent.AddChild(new GameObject(), false, childName);
    }

    public static RectTransform AddViewToParent(
      this Transform parent,
      GameObject child,
      bool worldPositionStays)
    {
      RectTransform component = child.GetComponent<RectTransform>();
      component.SetParent(parent, worldPositionStays);
      UtilityGameObject.SyncLayer(parent.gameObject, child, false);
      return component;
    }

    public static RectTransform AddViewToParentRecusive(
      this Transform parent,
      GameObject child,
      bool worldPositionStays,
      bool isRecusive)
    {
      RectTransform component = child.GetComponent<RectTransform>();
      component.SetParent(parent, worldPositionStays);
      UtilityGameObject.SyncLayer(parent.gameObject, child, isRecusive);
      return component;
    }

    public static void ReParent(this GameObject go)
    {
      go.transform.SetSiblingIndex(go.transform.parent.childCount - 1);
    }

    public static void SetLayer(GameObject target, int layer, bool recuresively)
    {
      if (!(bool) ((UnityEngine.Object) target))
        return;
      target.layer = layer;
      if (recuresively)
      {
        for (int index = 0; index < target.transform.childCount; ++index)
          UtilityGameObject.SetLayer(target.transform.GetChild(index).gameObject, layer, recuresively);
      }
    }

    public static void SyncLayer(GameObject src, GameObject dst, bool recursive)
    {
      UtilityGameObject.SetLayer(dst, src.layer, recursive);
    }

    public static GameObject AddGameObject(string path)
    {
      GameObject gameObject1 = GameObject.Find(path);
      if ((UnityEngine.Object) gameObject1 != (UnityEngine.Object) null)
        return gameObject1;
      GameObject gameObject2 = (GameObject) null;
      string[] strArray = path.Split('/');
      int length = strArray.Length;
      for (int index = 0; index < length; ++index)
      {
        string str = strArray[index];
        if ((UnityEngine.Object) gameObject2 == (UnityEngine.Object) null)
        {
          gameObject1 = GameObject.Find(str);
          if ((UnityEngine.Object) gameObject1 == (UnityEngine.Object) null)
            gameObject1 = new GameObject(str);
        }
        else
        {
          gameObject1 = gameObject2.GetChild(str);
          if ((UnityEngine.Object) gameObject1 == (UnityEngine.Object) null)
            gameObject1 = gameObject2.AddNewChild(str);
        }
        gameObject2 = gameObject1;
      }
      return gameObject1;
    }

    public static Canvas AddCanvasComponent(GameObject goCanvas)
    {
      Canvas canvas = goCanvas.AddComponent<Canvas>();
      goCanvas.AddComponent<CanvasScaler>();
      goCanvas.AddComponent<GraphicRaycaster>();
      return canvas;
    }

    public static Canvas AddPanelCanvas(GameObject goCanvas)
    {
      Canvas canvas = goCanvas.AddComponent<Canvas>();
      goCanvas.AddComponent<GraphicRaycaster>();
      return canvas;
    }

    public static void PlayAnimByName(GameObject go, string name)
    {
      Animator componentInChildren = go.GetComponentInChildren<Animator>();
      if ((bool) ((UnityEngine.Object) componentInChildren))
      {
        componentInChildren.enabled = true;
        componentInChildren.Play(name);
      }
      else
        Debug.LogError((object) (go.name + "没有动画组件"));
    }

    public static Component AddComponet<T>(GameObject go) where T : Component
    {
      T component = go.GetComponent<T>();
      if ((bool) ((UnityEngine.Object) component))
        return (Component) component;
      return (Component) go.AddComponent<T>();
    }

    public static T AddComponetIfNone<T>(GameObject go) where T : Component
    {
      T component = go.GetComponent<T>();
      if ((bool) ((UnityEngine.Object) component))
        return component;
      return go.AddComponent<T>();
    }

    public static string GetFullPath(GameObject go)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(go.name);
      for (Transform parent = go.transform.parent; (UnityEngine.Object) parent != (UnityEngine.Object) null; parent = parent.transform.parent)
        stringBuilder.Insert(0, parent.name + "/");
      return stringBuilder.ToString();
    }
  }
}
