using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;

namespace SDGame.UITools
{
    public class UIBindingPrefabSaveHelper : UnityEditor.AssetModificationProcessor
    {
        /// <summary>
        /// 保存资源时修正控件绑定数据
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        static string[] OnWillSaveAssets(string[] paths)
        {
            GameObject goInHierarchy = Selection.activeGameObject;
            if(goInHierarchy != null)
            {
                UIControlData[] uiControlData = goInHierarchy.GetComponentsInChildren<UIControlData>();
                if (uiControlData != null)
                {
                    foreach (var comp in uiControlData)
                    {
                        comp.CorrectComponents();
                        comp.CheckSubUIs();
                    }
                }
            }

            return paths;
        }

        //static void StartInitializeOnLoadMethod()
        //{
        //    PrefabUtility.prefabInstanceUpdated += ProcessUIControlData;
        //}

        public static void SavePrefab(GameObject goInHierarchy)
        {
            UnityEngine.Object goPrefab = null;
            while (goPrefab == null)
            {
                goPrefab = PrefabUtility.GetPrefabParent(goInHierarchy);
                if (goPrefab != null)
                    break;

                var t = goInHierarchy.transform.parent;
                if (t != null)
                    goInHierarchy = t.gameObject;
                else
                    break;
            }

            if (goPrefab != null)
                PrefabUtility.ReplacePrefab(goInHierarchy, goPrefab, ReplacePrefabOptions.ConnectToPrefab);
            else
                Debug.LogFormat("<color=red>当前对象不属于Prefab, 请将其保存为 Prefab</color>");
        }

        public static void ClearConsole()
        {
    #if UNITY_2017 || UNITY_2018
            var logEntries = Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
    #else
            var logEntries = System.Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
    #endif
            var clearMethod = logEntries.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
            clearMethod.Invoke(null, null);
        }
    }

}
#endif