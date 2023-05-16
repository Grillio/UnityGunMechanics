using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace EGHiddenNamespace
{
    [Serializable]
    public static class EGExecuteables
    {
        public static readonly EGExecute Execute = EGHiddenManager.Manager.GetComponent<EGExecute>();
        public static Action<EasyGun> ShootGun;
        public static Action<EasyGun> ReloadGun;

        public static void EasyGunStartedReloading(EasyGun EG)
        {
            EasyGunsSceneEvents.ExecuteCurrentEvent = true;
           // Debug.Log("Executing Reloading Event");
                if (EasyGunsSceneEvents.onEasyGunStartedReload != null)
                {
                    EasyGunsSceneEvents.onEasyGunStartedReload(EG);
                }

                if (EasyGunsSceneEvents.ExecuteCurrentEvent && ReloadGun != null)
                    ReloadGun(EG);
        }
        
            public static void EasyGunShot(EasyGun EG)
            {
                EasyGunsSceneEvents.ExecuteCurrentEvent = true;
               // Debug.Log("Executing Shot Event");
                if (EasyGunsSceneEvents.onEasyGunShoot != null)
                {
                    EasyGunsSceneEvents.onEasyGunShoot(EG);
                }

                if (EasyGunsSceneEvents.ExecuteCurrentEvent && ShootGun != null)
                    ShootGun(EG);
            }
    }

    [Serializable]
    public class EGExecute : MonoBehaviour
    {
        public void Shoot(EasyGun EG)
        {
            Debug.Log("Shoot");
            GameObject BulletAsset = EG.GunProperties.AutomaticProperties.BulletAsset;
            float Accuracy = EG.GunProperties.GeneralProperties.Accuracy;
            GameObject BarrelTip = EG.GunProperties.GeneralProperties.BarrelTip;
            Vector3 bulletRot = CalculateShotDirection(BarrelTip);
            GameObject NewBullet = Instantiate(BulletAsset, BarrelTip.transform.position,
                Quaternion.LookRotation(BarrelTip.transform.forward * 40 + bulletRot * (2 - ((Accuracy / 100f) * 2))));
            NewBullet.GetComponent<Rigidbody>()
                .AddForce(NewBullet.transform.forward * EG.GunProperties.AutomaticProperties.ExitVelocity,
                    ForceMode.Impulse);
        }


        public void Reload(EasyGun EG, GameObject bulletAsset)
        {
            Debug.Log(bulletAsset);
        }

        public Vector3 CalculateShotDirection(GameObject BarrelTip)
        {
            Vector3 spot = BarrelTip.transform.forward * 40;
            spot = Quaternion.LookRotation(BarrelTip.transform.forward * 40) * Random.insideUnitCircle;
            return spot;
        }
    }
}
