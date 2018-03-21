using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWindow
{
    void Initialize();
    void Destroy();
    void Open();
    void Close();
    void Show();
    void Hide();
    void Refresh();
}