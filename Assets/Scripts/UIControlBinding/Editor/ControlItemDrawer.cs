using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class ControlItemDrawer
{
    private UIControlDataEditor     _container;
    private ControlItem             _item;
    private bool                    _foldout;

    public ControlItemDrawer(UIControlDataEditor container, ControlItem item)
    {
        _container = container;
        _item = item;
    }
    public void Draw()
    {
        Rect rect = EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("变量名 ", UIControlDataEditor.skin.label);
        _item.name = EditorGUILayout.TextField(_item.name, UIControlDataEditor.skin.textField);

        EditorGUILayout.Space();
        _foldout = EditorGUILayout.Foldout(_foldout, _foldout ? "收起" : "展开", true);

        if (GUILayout.Button("+", EditorStyles.miniButton))
        {
            _container.AddControl(this);
        }

        if (GUILayout.Button("-", EditorStyles.miniButton))
        {
            _container.RemoveControl(this);
        }
        
        EditorGUILayout.EndHorizontal();

        
        // 控件列表
        if (_foldout)
        {
            EditorGUILayout.Space();
            for (int i = 0, imax = _item.targets.Count; i < imax; i++)
            {
                Object obj = _item.targets[i];
                EditorGUILayout.BeginHorizontal();
                _item.targets[i] = EditorGUILayout.ObjectField(obj, typeof(Object), true);
                EditorGUILayout.Space(); EditorGUILayout.Space(); EditorGUILayout.Space();
                if (GUILayout.Button("+", EditorStyles.miniButton))
                {
                    _item.targets.Insert(i + 1, new Object());
                    _container.Repaint();
                    return;
                }
                if (GUILayout.Button("-", EditorStyles.miniButton))
                {
                    _item.targets.RemoveAt(i);
                    _container.Repaint();
                    return;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
        }
        

        EditorGUILayout.EndVertical();

        GUI.Box(new Rect(rect.x - 10f, rect.y - 5f, rect.width + 20f, rect.height + 15f), "");

    }
}
