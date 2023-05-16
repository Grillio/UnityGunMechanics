using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EasyGunsSceneEvents
{
    public static bool ExecuteCurrentEvent;

    /// <summary>
    /// Use this to see when an EasyGun has been shot, CAN CANCEL THIS EVENT
    /// </summary>
    public static Action<EasyGun> onEasyGunShoot;
   
    /// <summary>
    /// Use this to see when an EasyGun has started reloading, CAN CANCEL THIS EVENT
    /// </summary>
    public static Action<EasyGun> onEasyGunStartedReload;
    
    
}
