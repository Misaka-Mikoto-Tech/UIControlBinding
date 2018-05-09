using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIControlData))]
public class UIControlDataEditor : Editor
{
    public static GUISkin               skin;

    private List<CtrlItemData>          _ctrlItemDatas;
    private List<SubUIItemData>         _subUIItemDatas;

    private List<ControlItemDrawer>     _ctrlItemDrawers;
    private List<SubUIItemDrawer>       _subUIItemDrawers;

    void Awake()
    {
        if (EditorGUIUtility.isProSkin)
            skin = Resources.Load("Editor/UIControlDataSkinPro") as GUISkin;
        else
            skin = Resources.Load("Editor/UIControlDataSkinPersonal") as GUISkin;
    }

    public override void OnInspectorGUI()
    {
        if (skin == null || skin.customStyles == null || skin.customStyles.Length == 0)
        {
            base.OnInspectorGUI();
            return;
        }

        UIControlData data = target as UIControlData;
        if(data.ctrlItemDatas == null)
        {
            data.ctrlItemDatas = new List<CtrlItemData>();
            data.ctrlItemDatas.Add(new CtrlItemData());
        }
        if(data.subUIItemDatas == null)
        {
            data.subUIItemDatas = new List<SubUIItemData>();
        }

        _ctrlItemDatas = data.ctrlItemDatas;
        _subUIItemDatas = data.subUIItemDatas;
        CheckDrawers();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space();


        // 绘制控件绑定
        EditorGUILayout.LabelField("控件绑定", skin.customStyles[0]);
        foreach (var drawer in _ctrlItemDrawers)
        {
            GUILayout.Space(10f);
            if (!drawer.Draw())
            {
                Repaint();
                return;
            }
            GUILayout.Space(10f);
        }

        GUILayout.Space(10f);

        // 绘制子UI
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("子UI绑定", skin.customStyles[0]);
        if(_subUIItemDrawers.Count == 0)
        {
            if (GUILayout.Button("+", EditorStyles.miniButton))
            {
                AddSubUIAfter(-1);
                Repaint();
                return;
            }
        }
        EditorGUILayout.EndHorizontal();
        foreach(var drawer in _subUIItemDrawers)
        {
            GUILayout.Space(10f);
            if (!drawer.Draw())
            {
                Repaint();
                return;
            }
            GUILayout.Space(10f);
        }

        EditorGUILayout.EndVertical();

        this.Repaint();
    }

    public void AddControlAfter(ControlItemDrawer drawer)
    {
        int idx = _ctrlItemDrawers.IndexOf(drawer);
        Debug.Assert(idx != -1);

        AddControlAfter(idx);
    }

    public void AddSubUIAfter(SubUIItemDrawer drawer)
    {
        int idx = _subUIItemDrawers.IndexOf(drawer);
        Debug.Assert(idx != -1);

        AddSubUIAfter(idx);
    }

    public void RemoveControl(ControlItemDrawer drawer)
    {
        int idx = _ctrlItemDrawers.IndexOf(drawer);
        Debug.Assert(idx != -1);

        RemoveControl(idx);
    }

    public void RemoveSubUI(SubUIItemDrawer drawer)
    {
        int idx = _subUIItemDrawers.IndexOf(drawer);
        Debug.Assert(idx != -1);

        RemoveSubUI(idx);
    }

#region Private
    private void CheckDrawers()
    {
        if (_ctrlItemDrawers == null)
        {
            _ctrlItemDrawers = new List<ControlItemDrawer>();
            foreach(var item in _ctrlItemDatas)
            {
                ControlItemDrawer drawer = new ControlItemDrawer(this, item);
                _ctrlItemDrawers.Add(drawer);
            }
        }

        if(_subUIItemDrawers == null)
        {
            _subUIItemDrawers = new List<SubUIItemDrawer>();
            foreach(var item in _subUIItemDatas)
            {
                SubUIItemDrawer drawer = new SubUIItemDrawer(this, item);
                _subUIItemDrawers.Add(drawer);
            }
        }
    }

    private void AddControlAfter(int idx)
    {
        CtrlItemData itemData = new CtrlItemData();
        _ctrlItemDatas.Insert(idx + 1, itemData);

        ControlItemDrawer drawer = new ControlItemDrawer(this, itemData);
        _ctrlItemDrawers.Insert(idx + 1, drawer);
    }

    private void AddSubUIAfter(int idx)
    {
        SubUIItemData itemData = new SubUIItemData();
        _subUIItemDatas.Insert(idx + 1, itemData);

        SubUIItemDrawer drawer = new SubUIItemDrawer(this, itemData);
        _subUIItemDrawers.Insert(idx + 1, drawer);
    }

    private void RemoveControl(int idx)
    {
        if(_ctrlItemDatas.Count == 1)
        {
            Debug.LogError("至少应保留一个变量"); // TODO 去除此限制
        }
        else
        {
            _ctrlItemDatas.RemoveAt(idx);
            _ctrlItemDrawers.RemoveAt(idx);
        }
    }

    private void RemoveSubUI(int idx)
    {
        _subUIItemDatas.RemoveAt(idx);
        _subUIItemDrawers.RemoveAt(idx);
    }

#endregion
}
