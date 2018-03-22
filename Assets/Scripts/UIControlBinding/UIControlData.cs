using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public struct ControlItem
{
    public string                   name;
    [HideInInspector]
    public string                   type;
    public UnityEngine.Object[]     targets;
}

[DisallowMultipleComponent]
public class UIControlData : MonoBehaviour
{
    /// <summary>
    /// 所有绑定的组件，不允许重名
    /// </summary>
    public List<ControlItem>    controls;
    private static Dictionary<string, Type> _typeMap = new Dictionary<string, Type>()
    {
        { "Text", typeof(Text)},
        { "RawImage", typeof(RawImage)},
        { "Button", typeof(Button)},
        { "Toggle", typeof(Toggle)},
        { "Slider", typeof(Slider)},
        { "Scrollbar", typeof(Scrollbar)},
        { "Dropdown", typeof(Dropdown)},
        { "InputField", typeof(InputField)},
        { "Canvas", typeof(Canvas)},
        { "ScrollRect", typeof(ScrollRect)},
        { "Image", typeof(Image)},
        { "RectTransform", typeof(RectTransform)},
        { "Transform", typeof(Transform)},
    };

    public void BindAllFields(IWindow window)
    {
        if (window == null)
            return;

        FieldInfo[] fis = window.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        for(int i = 0, imax = fis.Length; i < imax; i++)
        {
            FieldInfo fi = fis[i];
            Type fieldType = fi.FieldType;
            if (fi.GetCustomAttributes(typeof(ControlBindingAttribute), false).Length == 0)
                continue;

            var objs = controls[GetIndex(fi.Name)];
            Type objType;
            if (_typeMap.TryGetValue(objs.type, out objType))
            {
                if (fieldType.IsArray)
                {
                    Array arrObj = Array.CreateInstance(objType, objs.targets.Length);
                    
                    // 给数组元素设置数据
                    for (int j = 0, jmax = objs.targets.Length; j < jmax; j++)
                    {
                        arrObj.SetValue(objs.targets[j], j);
                    }
                    fi.SetValue(window, arrObj);
                }
                else
                {
                    UnityEngine.Object component = GetComponent(fi.Name);
                    fi.SetValue(window, component);
                }
            }
        }
    }

    #region Get,不建议使用

    /// <summary>
    /// 找到指定名称的第一个组件, 不存在返回 null
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public T GetComponent<T>(string name) where T : Component
    {
        int idx = GetIndex(name);
        if (idx == -1)
            return null;

        Component[] targets = controls[idx].targets as Component[];
        if (targets.Length == 0)
            return null;

        return targets[0] as T;
    }

    public new UnityEngine.Object GetComponent(string name)
    {
        int idx = GetIndex(name);
        if (idx == -1)
            return null;

        var targets = controls[idx].targets;
        if (targets.Length == 0)
            return null;

        return targets[0];
    }

    public UnityEngine.Object[] GetComponents(string name)
    {
        int idx = GetIndex(name);
        if (idx == -1)
            return null;

        return controls[idx].targets;
    }

    

    private int  GetIndex(string name)
    {
        for (int i = 0, imax = controls.Count; i < imax; i++)
        {
            ControlItem item = controls[i];
            if (item.name == name)
            {
                return i;
            }
        }
        return -1;
    }

    #endregion

#if UNITY_EDITOR

    [InitializeOnLoadMethod]
    static void Start()
    {
        // 好像不太好使
        PrefabUtility.prefabInstanceUpdated += (GameObject instance) =>
        {
            UIControlData[] uiControlData = instance.GetComponentsInChildren<UIControlData>();
            if(uiControlData != null)
            {
                foreach(var comp in uiControlData)
                {
                    bool isOK = comp.CorrectComponents();
                    // TODO 出错时如何拦截 Apply?
                }
            }
        };
    }

    [ContextMenu("修正组件")]
    private bool CorrectComponents()
    {
        ClearConsole();

        bool isOK = true;
        for(int i = 0, imax = controls.Count; i < imax; i++)
        {
            for(int j = controls.Count - 1; j >= 0; j--)
            {
                if(controls[i].name == controls[j].name && i != j)
                {
                    Debug.LogErrorFormat("UI [{0}] 控件名字 [{1}] 第 {2} 项与第 {3} 项重复，请修正", gameObject.name, controls[i].name, i + 1, j + 1);
                    return false;
                }
            }
        }

        isOK = ReplaceTargetsToUIComponent();
        if(isOK)
            Debug.LogFormat("UI [{0}] 控件绑定修正完毕", gameObject.name);

        return isOK;
    }

    /// <summary>
    /// 由于自动拖上去的对象永远都是 GameObject，所以我们需要把它修正为正确的对象类型
    /// </summary>
    private bool ReplaceTargetsToUIComponent()
    {
        for(int i = 0, imax = controls.Count; i < imax; i++)
        {
            UnityEngine.Object[] objs = controls[i].targets;
            Type type = null;
            for(int j = 0, jmax = objs.Length; j < jmax; j++)
            {
                if(objs[j] == null)
                {
                    Debug.LogErrorFormat("UI [{0}] 控件名字 [{1}] 第 {2} 项为空，请修正", gameObject.name, controls[i].name, j + 1);
                    return false;
                }

                GameObject go = objs[j] as GameObject;
                if (go == null)
                    go = (objs[j] as Component).gameObject;

                var correctValue = FindCorrectComponent(go);
                if (type == null)
                    type = correctValue.Key;
                else if(type != correctValue.Key)
                {
                    Debug.LogErrorFormat("UI [{0}] 控件名字 [{1}] 第 {2} 项与第 1 项的类型不同，请修正", gameObject.name, controls[i].name, j + 1);
                    return false;
                }

                objs[j] = correctValue.Value;
            }

            controls[i] = new ControlItem() { name = controls[i].name, type = type.Name, targets = objs };
        }
        return true;
    }

    private KeyValuePair<Type, UnityEngine.Object> FindCorrectComponent(GameObject go)
    {
        List<Component> components = new List<Component>();
        go.GetComponents(components);

        Component newComp = null;

        for (int i = 0, imax = components.Count; i < imax; i++)
        {
            Component tmp = components[i];
            
            Type type = tmp.GetType();

            if (type == typeof(Text))
                newComp = go.GetComponent<Text>();
            else if (type == typeof(RawImage))
                newComp = go.GetComponent<RawImage>();
            else if (type == typeof(Button))
                newComp = go.GetComponent<Button>();
            else if (type == typeof(Toggle))
                newComp = go.GetComponent<Toggle>();
            else if (type == typeof(Slider))
                newComp = go.GetComponent<Slider>();
            else if (type == typeof(Scrollbar))
                newComp = go.GetComponent<Scrollbar>();
            else if (type == typeof(Dropdown))
                newComp = go.GetComponent<Dropdown>();
            else if (type == typeof(InputField))
                newComp = go.GetComponent<InputField>();
            else if (type == typeof(Canvas))
                newComp = go.GetComponent<Canvas>();
            //else if (type == typeof(Panel))
            //    newComp = go.GetComponent<Panel>();
            else if (type == typeof(ScrollRect))
                newComp = go.GetComponent<ScrollRect>();
            else if (type == typeof(Image))
                newComp = go.GetComponent<Image>();

            // 很多组件上都有 Image，因此即使找到 Image 也不停止
            if (newComp != null && type != typeof(Image))
                break;
        }

        if(newComp == null)
        {
            newComp = go.GetComponent<RectTransform>();
            if (newComp == null)
                newComp = go.GetComponent<Transform>();
        }

        return new KeyValuePair<Type, UnityEngine.Object>(newComp.GetType(), newComp);
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

    [ContextMenu("复制代码到剪贴板")]
    public void CopyCodeToClipBoard()
    {
        CorrectComponents();

        StringBuilder sb = new StringBuilder(1024);
        sb.Append("#region 控件绑定变量声明，自动生成请勿手改\r\n");

        for(int i = 0, imax = controls.Count; i < imax; i++)
        {
            ControlItem ctrl = controls[i];
            if (ctrl.targets.Length == 0)
                continue;


            if(ctrl.targets.Length == 1)
            {
                sb.AppendFormat("\t\t[ControlBinding]\r\n\t\tprivate {0} {1};\r\n", ctrl.type, ctrl.name);
            }
            else
            {
                // 由于没有找到有效修改数组长度的API，此处先写死长度
                sb.AppendFormat("\t\t[ControlBinding]\r\n\t\tprivate {0}[] {1};\r\n", ctrl.type, ctrl.name);
            }
        }
        sb.Append("#endregion\r\n\r\n");

        GUIUtility.systemCopyBuffer = sb.ToString();

        UnityEngine.Object go = PrefabUtility.GetPrefabParent(gameObject);
        PrefabUtility.ReplacePrefab(gameObject, go, ReplacePrefabOptions.Default);
    }

#endif
    }
