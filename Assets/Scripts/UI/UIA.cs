using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SDGame.UITools;

public class UIA : IWindow
{
    #region 控件绑定变量声明，自动生成请勿手改
    [ControlBinding]
    private Text[] lbl_fps;
    [ControlBinding]
    private Button btn_login;
    [ControlBinding]
    private Toggle tg_mute;

    [SubUIBinding]
    private UIControlData LeftUI;
    [SubUIBinding]
    private UIControlData RightUI;
    #endregion







    public void CheckBinding()
    {
        Debug.Assert(lbl_fps != null && lbl_fps.Length == 2);
        Debug.Assert(btn_login != null);
        Debug.Assert(tg_mute != null);
        Debug.Assert(LeftUI != null);
        Debug.Assert(RightUI != null);
        Debug.Log("<color=lime>绑定测试通过</color>");
    }

    public void Close()
    {
        //throw new System.NotImplementedException();
    }

    public void Destroy()
    {
        //throw new System.NotImplementedException();
    }

    public void Hide()
    {
        //throw new System.NotImplementedException();
    }

    public void Initialize()
    {
        //throw new System.NotImplementedException();
    }

    public void Open()
    {
        //throw new System.NotImplementedException();
    }

    public void Refresh()
    {
        //throw new System.NotImplementedException();
    }

    public void Show()
    {
        //throw new System.NotImplementedException();
    }
}
