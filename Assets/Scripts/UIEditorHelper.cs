using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI 的编辑器帮助类，用于各种的自动化编辑；
/// </summary>
public class UIEditorHelper : MonoBehaviour
{
        /// <summary>
        /// UI默认的材质；
        /// </summary>
        [ContextMenuItem("替换UI材质球", "SetDefautMat")]
        public Material UIDefaultMat;

        /// <summary>
        /// 设置成默认材质；
        /// </summary>
        [ExecuteInEditMode]
        public void SetDefautMat()
        {
                if (UIDefaultMat == null)
                {
                        Debug.LogError("选择的默认材质球为空！");
                        return;
                }
                //获取所有的图片和文本；
                Image[] mArrImg = transform.GetComponentsInChildren<Image>();
                Text[] mArrText = transform.GetComponentsInChildren<Text>();
                Debug.Log("图片数量：" + mArrImg.Length);
                Debug.Log("文本数量：" + mArrText.Length);
                //如果他们身上没有材质球（使用的默认材质球），那么替换成我们选定的材质球；
                for (int i = 0; i < mArrImg.Length; i++)
                {
                        var img = mArrImg[i];
                        if (img.material == img.defaultMaterial || img.material.name == "MyUIMaterial" || img.material.name == "ui_default")
                        {
                                //Mask不能用这个材质球，所以跳过； 
                                if (img.transform.GetComponent<Mask>() == null)
                                        img.material = UIDefaultMat;
                        }
                }
                for (int i = 0; i < mArrText.Length; i++)
                {
                        var text = mArrText[i];
                        if (text.material == text.defaultMaterial)
                        {
                                text.material = UIDefaultMat;
                        }
                }
                Debug.Log("替换完成！");
        }

}