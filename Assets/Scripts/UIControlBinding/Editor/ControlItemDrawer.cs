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
            for (int i = 0, imax = _item.targets.Length; i < imax; i++)
            {
                Object obj = _item.targets[i];
                EditorGUILayout.BeginHorizontal();
                _item.targets[i] = EditorGUILayout.ObjectField(obj, typeof(Object), true);
                EditorGUILayout.Space(); EditorGUILayout.Space(); EditorGUILayout.Space();
                if (GUILayout.Button("+", EditorStyles.miniButton))
                {
                    InsertItem(i + 1);
                    _container.Repaint();
                    return;
                }
                if (GUILayout.Button("-", EditorStyles.miniButton))
                {
                    RemoveItem(i);
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

    private void InsertItem(int idx)
    {
        Object[] newArr = new Object[_item.targets.Length + 1];
        for(int i = 0; i < idx; i++)
        {
            newArr[i] = _item.targets[i];
        }
        newArr[idx] = new Object();
        for(int i = idx + 1; i < newArr.Length; i++)
        {
            newArr[i] = _item.targets[i - 1];
        }

        _item.targets = newArr;
    }

    private void RemoveItem(int idx)
    {
        Object[] newArr = new Object[_item.targets.Length - 1];
        for(int i = 0; i < idx; i++)
        {
            newArr[i] = _item.targets[i];
        }

        for(int i = idx; i < newArr.Length; i++)
        {
            newArr[idx] = _item.targets[i + 1];
        }

        _item.targets = newArr;
    }
}
