using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class EasyGunMakeAssetWindow : EditorWindow
{
    public static EditorWindow Window;


    public static void Open()
    {
        Window = GetWindowWithRect<EasyGunMakeAssetWindow>(new Rect(Screen.width / 2, Screen.height / 2, 300, 300), false, "EasyGun Warning");
        Window.Show();
    }


    public void CreateGUI()
    {
        
    }
}
