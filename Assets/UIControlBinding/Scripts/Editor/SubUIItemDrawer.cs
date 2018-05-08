using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubUIItemDrawer
{
    private UIControlDataEditor _container;
    private SubUIControlData    _item;

    public SubUIItemDrawer(UIControlDataEditor container, SubUIControlData item)
    {
        _container = container;
        _item = item;
    }

    public bool Draw()
    {
        return false;
    }

    private void PostProcess()
    {

    }
}
