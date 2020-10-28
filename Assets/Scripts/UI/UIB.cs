using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SDGame.UITools;

public class UIB : IBindableUI
{
    #region 控件绑定变量声明，自动生成请勿手改
#pragma warning disable 0649
    [ControlBinding]
    private Button btn_OK;
    [ControlBinding]
    private Text[] txt_group;
    [ControlBinding]
    private Dropdown sel_country;
#pragma warning restore 0649
    #endregion



    public void Close()
    {
        throw new System.NotImplementedException();
    }

    public void Destroy()
    {
        throw new System.NotImplementedException();
    }

    public void Hide()
    {
        throw new System.NotImplementedException();
    }

    public void Initialize()
    {
        throw new System.NotImplementedException();
    }

    public void Open()
    {
        throw new System.NotImplementedException();
    }

    public void Refresh()
    {
        throw new System.NotImplementedException();
    }

    public void Show()
    {
        throw new System.NotImplementedException();
    }
}
