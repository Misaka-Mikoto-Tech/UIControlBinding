using SDGame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDGame.UITools;

public class UIManager : UnitySingleton<UIManager>
{

	void Start () {
        // TODO get config from xml
        IWindow uiA = Activator.CreateInstance(Type.GetType("UIA")) as IWindow;
        GameObject prefab = Resources.Load<GameObject>("UI/UIA");
        GameObject go = Instantiate(prefab);
        UIControlData ctrlData = go.GetComponent<UIControlData>();
        if(ctrlData != null)
        {
            ctrlData.BindAllFields(uiA);
        }

        (uiA as UIA).CheckBinding();
	}
	
	void Update () {
		
	}
}
