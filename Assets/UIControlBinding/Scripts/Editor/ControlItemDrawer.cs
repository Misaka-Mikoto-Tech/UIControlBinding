using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class ControlItemDrawer
{
    private UIControlDataEditor     _container;
    private CtrlItemData            _itemData;
    private bool                    _foldout = true;

    public ControlItemDrawer(UIControlDataEditor container, CtrlItemData item)
    {
        _container = container;
        _itemData = item;
    }

    public bool Draw()
    {
        Rect rect = EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("变量名 ", UIControlDataEditor.skin.label);
            _itemData.name = EditorGUILayout.TextField(_itemData.name, UIControlDataEditor.skin.textField);

            EditorGUILayout.Space();
            _foldout = EditorGUILayout.Foldout(_foldout, _foldout ? "收起" : "展开", true);

            if (GUILayout.Button("+", EditorStyles.miniButton))
            {
                _container.AddControlAfter(this);
                return false;
            }

            if (GUILayout.Button("-", EditorStyles.miniButton))
            {
                _container.RemoveControl(this);
                return false;
            }
        }
        EditorGUILayout.EndHorizontal();

        
        // 控件列表
        if (_foldout)
        {
            EditorGUILayout.Space();
            for (int i = 0, imax = _itemData.targets.Length; i < imax; i++)
            {
                Object obj = _itemData.targets[i];
                EditorGUILayout.BeginHorizontal();
                _itemData.targets[i] = EditorGUILayout.ObjectField(obj, typeof(Object), true);
                EditorGUILayout.Space(); EditorGUILayout.Space(); EditorGUILayout.Space();
                if (GUILayout.Button("+", EditorStyles.miniButton))
                {
                    InsertItem(i + 1);
                    return false;
                }
                if (GUILayout.Button("-", EditorStyles.miniButton))
                {
                    if(_itemData.targets.Length == 1)
                    {
                        Debug.LogError("至少应保留一个控件");
                        return false;
                    }
                    else
                    {
                        RemoveItem(i);
                        return false;
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
        }
        

        EditorGUILayout.EndVertical();

        if (EditorGUIUtility.isProSkin)
            GUI.Box(new Rect(rect.x - 10f, rect.y - 5f, rect.width + 20f, rect.height + 15f), "");
        else
            GUI.Box(new Rect(rect.x - 10f, rect.y - 5f, rect.width + 20f, rect.height + 15f), "", UIControlDataEditor.skin.box);

        PostProcess();
        return true;
    }

    private void PostProcess()
    {
        // 默认将新添加的第一个控件的名字作为变量名
        if (_itemData.targets.Length > 0 && _itemData.targets[0] != null && string.IsNullOrEmpty(_itemData.name))
            _itemData.name = _itemData.targets[0].name;
    }

    private void InsertItem(int idx)
    {
        Object[] newArr = new Object[_itemData.targets.Length + 1];
        for(int i = 0; i < idx; i++)
        {
            newArr[i] = _itemData.targets[i];
        }
        newArr[idx] = new Object();
        for(int i = idx + 1; i < newArr.Length; i++)
        {
            newArr[i] = _itemData.targets[i - 1];
        }

        _itemData.targets = newArr;
    }

    private void RemoveItem(int idx)
    {
        Object[] newArr = new Object[_itemData.targets.Length - 1];
        for(int i = 0; i < idx; i++)
        {
            newArr[i] = _itemData.targets[i];
        }

        for(int i = idx; i < newArr.Length; i++)
        {
            newArr[idx] = _itemData.targets[i + 1];
        }

        _itemData.targets = newArr;
    }
}
