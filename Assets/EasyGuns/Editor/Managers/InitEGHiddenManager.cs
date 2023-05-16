using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace EGHiddenNamespace
{
    public static class MakeHiddenManager
        {
            [InitializeOnLoadMethod]
            private static void CheckForAndMakeHiddenEGManager()
            {
                var tempObj = GameObject.Find("EG_HiddenManager");
                if (tempObj == null)
                {
                   // Debug.Log("Made an EG Hidden Manager");
                    EGHiddenManager.Manager = new GameObject("EG_HiddenManager");
                    EGHiddenManager.Manager.AddComponent<EGExecute>();
                }
                else
                {
                   // Debug.Log("Assigned editor EGHManager");
                    EGHiddenManager.Manager = tempObj;
                }
//                Debug.Log(EGHiddenManager.Manager);
            }
        }
}
