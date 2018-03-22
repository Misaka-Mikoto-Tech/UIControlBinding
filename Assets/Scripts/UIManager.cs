using SDGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : UnitySingleton<UIManager>
{

	void Start () {
        // TODO get config from xml
        UIA uiA = new UIA();
        GameObject prefab = Resources.Load<GameObject>("UI/UIA");
        GameObject go = Instantiate(prefab);
        UIControlData ctrlData = go.GetComponent<UIControlData>();
        if(ctrlData != null)
        {
            ctrlData.BindAllFields(uiA);
        }

        uiA.CheckBinding();
	}
	
	void Update () {
		
	}
}
