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
    public Type                     type;
    public UnityEngine.Object[]     targets;
}

[DisallowMultipleComponent]
public class UIControlData : MonoBehaviour
{
    /// <summary>
    /// 所有绑定的组件，不允许重名
    /// </summary>
    public List<ControlItem>    controls;

    public void BindAllFields(IWindow window)
    {
        if (window == null)
            return;

        FieldInfo[] fis = window.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        for(int i = 0, imax = fis.Length; i < imax; i++)
        {
            FieldInfo fi = fis[i];
            Type fieldType = fi.FieldType;
            if (fi.FieldType.GetCustomAttributes(typeof(ControlBindingAttribute), false).Length == 0)
                continue;

            if (fieldType.IsArray)
            {
                Component[] components = GetComponents(fi.Name);
                fi.SetValue(window, components);
            }
            else
            {
                Component component = GetComponent(fi.Name);
                fi.SetValue(window, component);
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

    public new Component GetComponent(string name)
    {
        int idx = GetIndex(name);
        if (idx == -1)
            return null;

        Component[] targets = controls[idx].targets as Component[];
        if (targets.Length == 0)
            return null;

        return targets[0];
    }

    public Component[] GetComponents(string name)
    {
        int idx = GetIndex(name);
        if (idx == -1)
            return null;

        return controls[idx].targets as Component[];
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

            controls[i] = new ControlItem() { name = controls[i].name, type = type, targets = objs };
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
#if UNITY_2017
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
        sb.Append("\t\t#region 控件绑定变量声明\r\n");
        sb.AppendFormat("\t\tprivate List<ControlItem> uiCtrls = view.GetComponent<UIControlData>().controls;\r\n");

        for(int i = 0, imax = controls.Count; i < imax; i++)
        {
            ControlItem ctrl = controls[i];
            if (ctrl.targets.Length == 0)
                continue;


            if(ctrl.targets.Length == 1)
            {
                sb.AppendFormat("\t\tprivate {0} {1} = uiCtrls[{2}].targets[0] as {3};\r\n", ctrl.type.Name, ctrl.name, i, ctrl.type.Name);
            }
            else
            {
                sb.AppendFormat("\t\tprivate {0}[] {1} = new {2}[{3}];\r\n", ctrl.type.Name, ctrl.name, ctrl.type.Name, ctrl.targets.Length);
                for(int j = 0; j < ctrl.targets.Length; j++)
                {
                    sb.AppendFormat("\t\t{0}[{1}] = uiCtrls[{2}].targets[{3}] as {4};\r\n", ctrl.name, j, i, j, ctrl.type.Name);
                }
            }
        }
        sb.Append("\t\t#endregion\r\n\r\n");

        GUIUtility.systemCopyBuffer = sb.ToString();
    }

#endif
    }
