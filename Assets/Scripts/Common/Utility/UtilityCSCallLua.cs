using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSoft.Utility
{
        public class UtilityCSCallLua
        {
                /// <summary>
                /// 返回对应窗口是否显示
                /// </summary>
                /// <param name="pWindowName"></param>
                /// <returns></returns>
                public static bool IsUIWindowShow(string pWindowName)
                {
                        var returnObj = XLuaManager.Instance.CallLuaFunction("UtilityCSCallLua", "IsUIWindowShow", pWindowName);
                        if (returnObj == null)
                                return false;
                        else
                                return (bool)returnObj[0];
                }
                
                public static bool ShowUIWindow(string pWindowName, params object[] args)
                {
                        var returnObj = XLuaManager.Instance.CallLuaFunction("UtilityCSCallLua", "ShowUIWindow", pWindowName, args);
                        if (returnObj == null)
                        {
                                return false;
                        }
                        else
                        {
                                return returnObj[0] != null;
                        }
                }

                public static bool CloseUIWindow(string pWindowName)
                {
                        var returnObj = XLuaManager.Instance.CallLuaFunction("UtilityCSCallLua", "CloseUIWindow", pWindowName);
                        if (returnObj == null)
                        {
                                return false;
                        }
                        else
                        {
                                return returnObj[0] != null;
                        }
                }

                public static bool PassiveSwitchLuaScene(string pSceneName)
                {
                        var returnObj = XLuaManager.Instance.CallLuaFunction("UtilityCSCallLua", "PassiveSwitchLuaSceneByName", pSceneName);
                        if (returnObj == null)
                        {
                                return false;
                        }
                        else
                        {
                                return returnObj[0] != null;
                        }
                }

                public static bool SwitchLuaScene(string pSceneName)
                {
                        var returnObj = XLuaManager.Instance.CallLuaFunction("UtilityCSCallLua", "SwitchLuaSceneByName", pSceneName);
                        if (returnObj == null)
                        {
                                return false;
                        }
                        else
                        {
                                return returnObj[0] != null;
                        }
                }
        }
}
