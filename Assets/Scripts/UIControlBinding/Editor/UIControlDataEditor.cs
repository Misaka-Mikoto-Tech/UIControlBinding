using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIControlData))]
public class UIControlDataEditor : Editor
{
    public static GUISkin               skin;
    private List<ControlItem>           _controls;
    private List<ControlItemDrawer>     _drawers;

    private void Awake()
    {
        skin = Resources.Load("Editor/UIControlDataSkin") as GUISkin;
    }

    public override void OnInspectorGUI()
    {
        UIControlData data = target as UIControlData;
        if(data.controls == null)
        {
            data.controls = new List<ControlItem>();
            data.controls.Add(new ControlItem());
        }
        _controls = data.controls;
        CheckDrawers();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("控件绑定", skin.customStyles[0]);

        
        for (int i = 0, imax = _drawers.Count; i < imax; i++)
        {
            GUILayout.Space(10f);
            if (!_drawers[i].Draw())
            {
                Repaint();
                return;
            }
            GUILayout.Space(10f);
        }

        // 如果有拖放则添加一个新的控件

        GUILayout.Space(10f);
        EditorGUILayout.EndVertical();

        this.Repaint();
    }

    private void CheckDrawers()
    {
        if (_drawers == null)
        {
            _drawers = new List<ControlItemDrawer>();
            foreach(var item in _controls)
            {
                ControlItemDrawer drawer = new ControlItemDrawer(this, item);
                _drawers.Add(drawer);
            }
        }
    }

    private void AddControl(int idx)
    {
        ControlItem item = new ControlItem();
        _controls.Insert(idx + 1, item);

        ControlItemDrawer drawer = new ControlItemDrawer(this, item);
        _drawers.Insert(idx + 1, drawer);
    }

    private void RemoveControl(int idx)
    {
        if(_controls.Count == 1)
        {
            Debug.LogError("至少应保留一个变量");
        }
        else
        {
            _controls.RemoveAt(idx);
            _drawers.RemoveAt(idx);
        }
    }

    public void AddControl(ControlItemDrawer drawer)
    {
        int idx = _drawers.IndexOf(drawer);
        Debug.Assert(idx != -1);

        AddControl(idx);
    }

    public void RemoveControl(ControlItemDrawer drawer)
    {
        int idx = _drawers.IndexOf(drawer);
        Debug.Assert(idx != -1);

        RemoveControl(idx);
    }
}
