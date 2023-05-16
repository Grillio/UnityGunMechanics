using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands.Import;
using Codice.CM.Common;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using EGHiddenNamespace;
using PlasticPipe.PlasticProtocol.Messages;
using UnityEditor.Experimental;
using UnityEditor.Graphs;
using UnityEditor.VersionControl;

[CustomEditor(typeof(EasyGun), true), Serializable]
public class EasyGunEditor : Editor
{
    private SerializedObject script;
    private VisualElement root;
    private EasyGun EasyGun;
    private Vector3 red;
    private Vector3 green;

    private const string Shotgun = "Shotgun Properties";
    private const string Automatic = "Automatic Properties";
    private const string Semiautomatic = "Semiautomatic Properties";
    private const string Burst = "Burst Properties";
    private const string Bow = "Bow Properties";
    private const string Flamethrower = "Flamethrower Properties";
    private const string Laser = "Laser Gun Properties";
    private const string RailGun = "Rail Gun Properties";

    private const string DrawbackForce = "Drawback Force: ";
    
    
    
    private void OnEnable()
    {
        Color greencol = Color.green;
        green = new Vector3(greencol.r, greencol.g, greencol.b);
        red = new Vector3(255, 0, 0);
        
        script = serializedObject;
        EasyGun = serializedObject.targetObject as EasyGun;

        root = new VisualElement();

        VisualTreeAsset tree = Resources.Load<VisualTreeAsset>("UXML/EasyGunUI");
        tree.CloneTree(root);

        StyleSheet styleSheet = Resources.Load<StyleSheet>("StyleSheets/EasyGunStyle");
        root.styleSheets.Add(styleSheet);
    }
    
    public override VisualElement CreateInspectorGUI()
    {
        var WholeContainer = root.Q<VisualElement>("WholeContainer");
        var WholeInnerContainer = root.Q<VisualElement>("WholeInnerContainer");

        #region ConfigureGun

        var ConfigGunButton = root.Q<Button>("ConfigureEasyGunButton");
        var IsConfigured = root.Q<Toggle>("IsConfigured");
        IsConfigured.style.display = DisplayStyle.None;
        IsConfigured.BindProperty(script.FindProperty("IsConfigured"));

        ConfigGunButton.clickable.clicked += ConfigureGun;

        WholeInnerContainer.style.display = DisplayStyle.None;

        void ConfigureGun()
        {
            if (PrefabUtility.IsPartOfPrefabAsset(EasyGun))
            {
                AssetDatabase.OpenAsset(EasyGun);
                GameObject root = PrefabUtility.LoadPrefabContents(AssetDatabase.GetAssetPath(EasyGun));
            }
            else
            {
                
                EasyGunMakeAssetWindow.Open();
            }
        }
        
        IsConfigured.RegisterCallback<ChangeEvent<bool>>(e =>
        {
            WholeInnerContainer.style.display = DisplayStyle.None;
            ConfigGunButton.style.display = DisplayStyle.Flex;
            if (e.newValue)
            {
                WholeInnerContainer.style.display = DisplayStyle.Flex;
                ConfigGunButton.style.display = DisplayStyle.None;
            }
        });

        #endregion

        #region EasyGunSpecs

        VisualElement specContainer = root.Q<VisualElement>("SpecsContainer");
        var WeaponType = root.Q<EnumField>("WeaponType");
        WeaponType.BindProperty(serializedObject.FindProperty("HiddenGunProperties.WeaponType"));
        var ShotType = root.Q<EnumField>("ShotType");
        ShotType.BindProperty(script.FindProperty("HiddenGunProperties.ShotType"));
        var ShootingProperty = root.Q<EnumField>("ShootingProperty");
        ShootingProperty.BindProperty(script.FindProperty("HiddenGunProperties.ShotProperty"));

        var BarrelTip = root.Q<ObjectField>("BarrelLocation");
        BarrelTip.objectType = typeof(GameObject);
        BarrelTip.BindProperty(script.FindProperty("HiddenGunProperties.BarrelTip"));

        #endregion

        #region AccuracyVariables

        
        
        #endregion
        
        #region ShotDependentVariables

        var ProjectileContainer = root.Q<VisualElement>("ProjectileDependents");
        var BulletAsset = root.Q<ObjectField>("BulletAsset");
        BulletAsset.BindProperty(script.FindProperty("HiddenGunProperties.bulletAsset"));
        BulletAsset.objectType = typeof(GameObject);
        BulletAsset.allowSceneObjects = false;
        var ExitVelocity = root.Q<FloatField>("ExitVelocity");
        ExitVelocity.BindProperty(script.FindProperty("HiddenGunProperties.ExitVelocity"));

        var HitScanContainer = root.Q<VisualElement>("HitscanDependents");
        var Range = root.Q<FloatField>("Range");
        Range.BindProperty(script.FindProperty("HiddenGunProperties.Range"));
        
        //checks when the shot type changes, displays the necessary variables depending on the new value
        ShotType.RegisterCallback<ChangeEvent<Enum>>(e =>
        {
            HitScanContainer.style.display = DisplayStyle.None;
            ProjectileContainer.style.display = DisplayStyle.None;

            if(e.newValue != null)
            if (e.newValue.ToString() == ShootingType.Hitscan.ToString())
            {
                HitScanContainer.style.display = DisplayStyle.Flex;
            }
            else
            {
                ProjectileContainer.style.display = DisplayStyle.Flex;
            }
            else
            {
                if (e.previousValue.ToString() == ShootingType.Hitscan.ToString())
                {
                    HitScanContainer.style.display = DisplayStyle.Flex;
                }
                else
                {
                    ProjectileContainer.style.display = DisplayStyle.Flex;
                }
            }
        });
        
        #endregion
        
        #region MagAndCanVariables
        
        var MagazineandCanisterContainer = root.Q<VisualElement>("MagazineAndCanisterContainer");
        var MagazineAndCanisterFoldout = root.Q<Foldout>("MagazineAndCanisterFoldout");
        MagazineAndCanisterFoldout.BindProperty(script.FindProperty("MagAndCanBool"));
        
        MagazineAndCanisterFoldout.RegisterCallback<ChangeEvent<bool>>(e =>
        {

            MagazineandCanisterContainer.style.display = DisplayStyle.None;
            if (e.newValue)
            {
                MagazineandCanisterContainer.style.display = DisplayStyle.Flex;
            }

        });

        #region Magazine
        
        var MagazineContainer = root.Q<VisualElement>("MagazineContainer");
        var MagazineDisplay = root.Q<VisualElement>("MagazineDisplay");
        MagazineDisplay.style.width = root.style.width;
        var MagazineDisplayFoldout = root.Q<Foldout>("MagazineDisplayFoldout");
        MagazineDisplayFoldout.BindProperty(script.FindProperty("MagDisplayBool"));
        var MaxBullets = root.Q<IntegerField>("MaxBullets");
        MaxBullets.BindProperty(script.FindProperty("HiddenGunProperties.MagazineCapacity"));
        var CurrentBulletCount = root.Q<IntegerField>("CurrentBulletCount");
        CurrentBulletCount.BindProperty(script.FindProperty("HiddenGunProperties.BulletsRemaining"));
        CurrentBulletCount.style.display = DisplayStyle.None;
        BulletTemplate[] allBullets = new BulletTemplate[]{};
        float BulletSize = 4;

        //update the magazine visual display when the maximum capacity changes
        MagazineDisplayFoldout.RegisterCallback<ChangeEvent<bool>>(e =>
        {
            MagazineDisplay.style.display = DisplayStyle.None;
            if (e.newValue)
            {
                MagazineDisplay.style.display = DisplayStyle.Flex;
            }
            
        });

        #endregion

        #region Canister

        var CanisterContainer = root.Q<VisualElement>("CanisterContainer");
        var CanisterDisplayContainerFoldout = root.Q<Foldout>("CanisterDisplayFoldout");
        var CanisterDisplayContainer = root.Q<VisualElement>("CanisterDisplayContainer");
        
        //toggle the view of the canister container
        CanisterDisplayContainerFoldout.RegisterCallback<ChangeEvent<bool>>(e =>
        {
            CanisterDisplayContainer.style.display = DisplayStyle.None;
            if (e.newValue)
            {
                CanisterDisplayContainer.style.display = DisplayStyle.Flex;
            }
        });

        var MaxCanisterCapacityLabel = root.Q<Label>("MaxCanisterCapacityLabel");
        var MaxCanisterCapacity = root.Q<FloatField>("MaxCanisterCapacity");
        MaxCanisterCapacity.BindProperty(script.FindProperty("HiddenGunProperties.CanisterCapacity"));
        var RemainingFuel = root.Q<FloatField>("RemainingFuel");
        RemainingFuel.BindProperty(script.FindProperty("HiddenGunProperties.RemainingFuel"));
        RemainingFuel.style.display = DisplayStyle.None;
        var CanisterInsideDisplay = root.Q<VisualElement>("CanisterDisplayInner");
        string CanisterRemainingString = "";

        //toggle to show the charge container
        MaxCanisterCapacity.RegisterCallback<ChangeEvent<float>>(e =>
        {

            RemainingFuel.value = e.newValue;

        });
        
        //adjust and change the color of the remaining fuel bar
        RemainingFuel.RegisterCallback<ChangeEvent<float>>(e =>
        {

            CanisterDisplayContainerFoldout.text = CanisterRemainingString + e.newValue;
            float value = e.newValue / MaxCanisterCapacity.value;
            AdjustFuelDisplay(value, CanisterInsideDisplay);

        });

        #endregion
        
        #endregion

        #region WeaponTypeDependentsVaraibles

        var DependencyFoldout = root.Q<Foldout>("DependencyFoldout");
        DependencyFoldout.BindProperty(script.FindProperty("WeaponSpecsBool"));
        var DependencyContainer = root.Q<VisualElement>("Dependencies");

        var RateOfFire = root.Q<FloatField>("RateOfFire");
        RateOfFire.BindProperty(script.FindProperty("HiddenGunProperties.RateOfFire"));
        
        var DepletionRate = root.Q<FloatField>("DepletionPerSecond");
        DepletionRate.BindProperty(script.FindProperty("HiddenGunProperties.DepletionPerSecond"));
        var TimeBetweenShots = root.Q<FloatField>("TimeBetweenShots");
        TimeBetweenShots.BindProperty(script.FindProperty("HiddenGunProperties.TimeBetweenShots"));
        var ReloadTime = root.Q<FloatField>("ReloadTime");
        ReloadTime.BindProperty(serializedObject.FindProperty("HiddenGunProperties.ReloadTime"));
        
        var Accuracy = root.Q<Slider>("AccuracyFloat");
        Accuracy.BindProperty(script.FindProperty("HiddenGunProperties.Accuracy"));
        Accuracy.highValue = 100;

        Accuracy.RegisterCallback<ChangeEvent<float>>(e =>
        {
            Accuracy.label = $"Accuracy% {e.newValue}";
        });

        #region Automatic

        var AutomaticContainer = root.Q<VisualElement>("AutomaticDependents");

        #endregion

        #region Semiautomatic

        var SemiautomaticContainer = root.Q<VisualElement>("SemiDependents");

        #endregion

        #region Shotgun
        
        var ShotgunContainer = root.Q<VisualElement>("ShotgunDependents");
        var PelletsPerShot = root.Q<IntegerField>("PelletsPerShot");
        PelletsPerShot.BindProperty(script.FindProperty("HiddenGunProperties.PelletsPerShot"));

        #endregion

        #region BurstGun

        var BurstContainer = root.Q<VisualElement>("BurstDependents");
        var ShotsPerBurst = root.Q<IntegerField>("ShotsPerBurst");
        ShotsPerBurst.BindProperty(script.FindProperty("HiddenGunProperties.ShotsPerBurst"));
        var RapidTimeBetweenShots = root.Q<FloatField>("RapidTimeBetweenShots");
        RapidTimeBetweenShots.BindProperty(script.FindProperty("HiddenGunProperties.RapidTimeBetweenShots"));
        RapidTimeBetweenShots.style.marginLeft = 15;
        
        ShotsPerBurst.RegisterCallback<ChangeEvent<int>>(e =>
        {
            ConfigureBurstMagDisplay(ShotsPerBurst.value, allBullets);
        });

        #endregion

        #region Bow

        var BowContainer = root.Q<VisualElement>("BowDependents");
        var BowMinCharge = root.Q<FloatField>("MinForce");
        BowMinCharge.BindProperty(script.FindProperty("HiddenGunProperties.BowMinCharge"));
        var BowMaxCharge = root.Q<FloatField>("MaxForce");
        BowMaxCharge.BindProperty(script.FindProperty("HiddenGunProperties.BowMaxCharge"));
        var BowForceLabel = root.Q<Label>("ChargePower");
        var BowForce = root.Q<FloatField>("BowForce");
        BowForce.BindProperty(script.FindProperty("HiddenGunProperties.ExitVelocity"));
        BowForce.style.display = DisplayStyle.None;
        var TimeToCharge = root.Q<FloatField>("TimeToCharge");
        TimeToCharge.BindProperty(script.FindProperty("HiddenGunProperties.TimeToCharge"));
        var BowChargeInner = root.Q<VisualElement>("BowChargeInner");
        
        //Changes the label of the bows force when the force changes in the EasyGun script
        BowForce.RegisterCallback<ChangeEvent<float>>(e =>
        {
            BowForceLabel.text = DrawbackForce + (Mathf.Round(BowForce.value * 10.0f) * .1f);
        });
        
        BowForce.RegisterCallback<ChangeEvent<float>>(e =>
        {
            if (Application.isPlaying)
            {
                float value = (e.newValue - BowMinCharge.value) / (BowMaxCharge.value - BowMinCharge.value);
                AdjustCharge(value, BowChargeInner);
            }
            else
            {
                AdjustCharge(0, BowChargeInner);
            }
        });

        #endregion

        #region Flamethrower

        var FlamethrowerContainer = root.Q<VisualElement>("FlamethrowerDependents");

        #endregion
        
        #region Laser

        var LaserContainer = root.Q<VisualElement>("LaserDependents");
        var ReloadProperty = root.Q<EnumField>("ReloadProperty");
        ReloadProperty.BindProperty(script.FindProperty("HiddenGunProperties.ReloadingProperty"));
        var RechargeRate = root.Q<FloatField>("RechargeRate");
        RechargeRate.BindProperty(script.FindProperty("HiddenGunProperties.RechargeRate"));
        var RechargeDelay = root.Q<FloatField>("RechargeDelay");
        RechargeDelay.BindProperty(script.FindProperty("HiddenGunProperties.RechargeDelay"));
        
        ReloadProperty.RegisterCallback<ChangeEvent<Enum>>(e =>
        {
            ReloadTime.style.display = DisplayStyle.None;
            RechargeDelay.style.display = DisplayStyle.None;
            RechargeRate.style.display = DisplayStyle.None;

            if (e.newValue != null)
            {
                if (e.newValue.ToString() == ReloadingProperty.Reload.ToString())
                {
                    ReloadTime.style.display = DisplayStyle.Flex;
                }
                else
                {
                    RechargeDelay.style.display = DisplayStyle.Flex;
                    RechargeRate.style.display = DisplayStyle.Flex;
                }
            }
            else
            {
                if (e.previousValue.ToString() == ReloadingProperty.Reload.ToString())
                {
                    ReloadTime.style.display = DisplayStyle.Flex;
                }
                else
                {
                    RechargeDelay.style.display = DisplayStyle.Flex;
                    RechargeRate.style.display = DisplayStyle.Flex;
                }
            }

        });

        #endregion

        #region RailGun

        

        #endregion

        #endregion

        #region Audio

        var AudioContainerFoldout = root.Q<Foldout>("AudioContainerFoldout");
        var AudioContainer = root.Q<VisualElement>("AudioContainer");

        var ShotAudio = root.Q<ObjectField>("ShotAudio");
        var FailedShotAudio = root.Q<ObjectField>("FailedShotAudio");
        var ReloadAudio = root.Q<ObjectField>("ReloadAudio");

        ShotAudio.objectType = typeof(AudioClip);
        FailedShotAudio.objectType = typeof(AudioClip);
        ReloadAudio.objectType = typeof(AudioClip);

        ShotAudio.BindProperty(script.FindProperty("HiddenGunProperties.ShotAudio"));
        FailedShotAudio.BindProperty(script.FindProperty("HiddenGunProperties.FailedShotAudio"));
        ReloadAudio.BindProperty(script.FindProperty("HiddenGunProperties.ReloadAudio"));

        ShotAudio.allowSceneObjects = false;
        FailedShotAudio.allowSceneObjects = false;
        ReloadAudio.allowSceneObjects = false;
        
        AudioContainerFoldout.RegisterCallback<ChangeEvent<bool>>(e =>
        {
            AudioContainer.style.display = DisplayStyle.None;
            if (e.newValue)
            {
                AudioContainer.style.display = DisplayStyle.Flex;
            }
        });

        #endregion

        #region Methods
        
        #region Magazine
        
        string MagazineDisplayName = "";

        MagazineContainer.RegisterCallback<ChangeEvent<int>>(e =>
        {
            MagazineDisplay.Clear();
            try
            {
                CurrentBulletCount.value= e.newValue;
                allBullets = new BulletTemplate[e.newValue];
                for (int i = 0; i < e.newValue; i++)
                {
                    BulletTemplate tempBullet = new BulletTemplate();
                    MagazineDisplay.Add(tempBullet);
                    if (WeaponType.value.ToString() != EGHiddenNamespace.WeaponType.Bow.ToString())
                    {
                        tempBullet.style.width = BulletSize;
                    }
                    else
                    {
                        tempBullet.style.height = 10;
                    }
                    allBullets[i] = tempBullet;
                }
            }
            catch
            {
                // Debug.LogWarning("Magazine capacity cannot be less than 0!");
            }

            if (WeaponType.value.ToString() == EGHiddenNamespace.WeaponType.Burst.ToString())
            {
                
                ConfigureBurstMagDisplay(ShotsPerBurst.value, allBullets);
                
            }
        });
        
        //updates the magazine visual display when the bullets remaining changes
        CurrentBulletCount.RegisterCallback<ChangeEvent<int>>(e =>
        {
            try
            {
                DrawMagazineVisualDisplay(e.newValue, allBullets);
                MagazineDisplayFoldout.text = MagazineDisplayName + e.newValue;
            }
            catch
            {
                // Debug.LogWarning("Cannot have less than 0 bullets remaining in a magazine!");
            }


        });
        
        #endregion
        
        //changes the shotgun variables around depending on whether it is supposed to be automatic or semiautomatic
        ShootingProperty.RegisterCallback<ChangeEvent<Enum>>(e =>
        {
            if(e.newValue != null)
            if (WeaponType.value.ToString() == EGHiddenNamespace.WeaponType.Shotgun.ToString())
            {
                if (e.newValue.ToString() == ShotProperty.Auto.ToString())
                {
                    TimeBetweenShots.style.display = DisplayStyle.None;
                    RateOfFire.style.display = DisplayStyle.Flex;
                    RateOfFire.label = "Shots per minute";
                }
                else
                {
                    RateOfFire.style.display = DisplayStyle.None;
                    TimeBetweenShots.style.display = DisplayStyle.Flex;
                    TimeBetweenShots.tooltip = "Seconds between shells getting fired off";
                    TimeBetweenShots.label = "Time between shots";
                }
            }
        });
        
        //Function to toggle showing the dependencies when the foldout changes/toggles
        DependencyFoldout.RegisterCallback<ChangeEvent<bool>>(e =>
        {
            DependencyContainer.style.display = DisplayStyle.None;
            if (e.newValue)
            {
                DependencyContainer.style.display = DisplayStyle.Flex;
            }
        });
        
        //displays the proper dependencies based on the WeaponType, changes when the weapontype changes
        WeaponType.RegisterCallback<ChangeEvent<Enum>>(e =>
        {
            AutomaticContainer.style.display = DisplayStyle.None;
            SemiautomaticContainer.style.display = DisplayStyle.None;
            ShotgunContainer.style.display = DisplayStyle.None;
            BurstContainer.style.display = DisplayStyle.None;
            BowContainer.style.display = DisplayStyle.None;
            FlamethrowerContainer.style.display = DisplayStyle.None;
            LaserContainer.style.display = DisplayStyle.None;
            MagazineContainer.style.display = DisplayStyle.Flex;
            CanisterContainer.style.display = DisplayStyle.None;
            ReloadProperty.style.display = DisplayStyle.None;
            ShootingProperty.style.display = DisplayStyle.None;

            ExitVelocity.style.display = DisplayStyle.Flex;
            DepletionRate.style.display = DisplayStyle.None;
            RateOfFire.style.display = DisplayStyle.Flex;
            TimeBetweenShots.style.display = DisplayStyle.None;
            ShotType.style.display = DisplayStyle.Flex;
            BulletAsset.label = "Bullet Asset";
            MagazineAndCanisterFoldout.text = "Magazine";
            BarrelTip.label = "Bullet Spawn Point";

            if (e.newValue != null)
            {
                ShotsPerBurst.value = 0;
                TimeBetweenShots.value = 0;
                RateOfFire.value = 0;
                MaxBullets.value = 0;
                CurrentBulletCount.value = 0;
                Range.value = 0;
                BowMinCharge.value = 0;
                BowMaxCharge.value = 0;
                BowForce.value = 0;
                ExitVelocity.value = 0;
                RapidTimeBetweenShots.value = 0;
                RemainingFuel.value = 0;
                MaxCanisterCapacity.value = 0;
                ReloadTime.value = 0;
                RechargeRate.value = 0;
                ReloadProperty.value = ReloadingProperty.Reload;
                RechargeDelay.value = 0;
                Accuracy.value = 0;
                BulletAsset.value = null;
                BarrelTip.value = null;
                TimeToCharge.value = 0;
                ShotAudio.value = null;
                FailedShotAudio.value = null;
                ReloadAudio.value = null;
                PelletsPerShot.value = 0;

                if (e.newValue.ToString() == EGHiddenNamespace.WeaponType.Automatic.ToString())
                {
                    AutomaticContainer.style.display = DisplayStyle.Flex;
                    DependencyFoldout.text = Automatic;
                    BulletSize = 6;
                    MagazineDisplay.style.flexDirection = FlexDirection.Row;
                    MaxBullets.label = "Max Bullets";
                    MagazineDisplayName = "Bullets Remaining: ";
                    MagazineDisplayFoldout.text = "Bullets Remaining: 0";
                }
                else if (e.newValue.ToString() == EGHiddenNamespace.WeaponType.Semiautomatic.ToString())
                {
                    SemiautomaticContainer.style.display = DisplayStyle.Flex;
                    DependencyFoldout.text = Semiautomatic;
                    RateOfFire.style.display = DisplayStyle.None;
                    TimeBetweenShots.style.display = DisplayStyle.Flex;
                    TimeBetweenShots.tooltip = "Seconds between each shot";
                    TimeBetweenShots.label = "Time Between Shots";
                    BulletSize = 12;
                    MagazineDisplay.style.flexDirection = FlexDirection.Row;
                    MaxBullets.label = "Max Bullets";
                    MagazineDisplayName = "Bullets Remaining: ";
                    MagazineDisplayFoldout.text = "Bullets Remaining: 0";
                }
                else if (e.newValue.ToString() == EGHiddenNamespace.WeaponType.Shotgun.ToString())
                {
                    ShotgunContainer.style.display = DisplayStyle.Flex;
                    DependencyFoldout.text = Shotgun;
                    MaxBullets.label = "Max Shells";
                    RateOfFire.style.display = DisplayStyle.None;
                    TimeBetweenShots.style.display = DisplayStyle.None;
                    if (ShootingProperty.value.ToString() == ShotProperty.Auto.ToString())
                    {
                        RateOfFire.style.display = DisplayStyle.Flex;
                        RateOfFire.label = "Shots per minute";
                    }
                    else
                    {
                        TimeBetweenShots.style.display = DisplayStyle.Flex;
                        TimeBetweenShots.tooltip = "Seconds between shells getting fired off";
                        TimeBetweenShots.label = "Time between shots";
                    }
                    BulletSize = 30;
                    MagazineDisplay.style.flexDirection = FlexDirection.Row;
                    MagazineDisplayName = "Shells Remaining: ";
                    MagazineDisplayFoldout.text = "Shells Remaining: 0";
                    ShootingProperty.style.display = DisplayStyle.Flex;
                    
                }
                else if (e.newValue.ToString() == EGHiddenNamespace.WeaponType.Burst.ToString())
                {
                    BurstContainer.style.display = DisplayStyle.Flex;
                    DependencyFoldout.text = Burst;
                    RateOfFire.style.display = DisplayStyle.None;
                    TimeBetweenShots.style.display = DisplayStyle.None;
                    TimeBetweenShots.style.display = DisplayStyle.Flex;
                    TimeBetweenShots.label = "Time Between Bursts";
                    BulletSize = 6;
                    MagazineDisplay.style.flexDirection = FlexDirection.Row;
                    MaxBullets.label = "Max Bullets";
                    MagazineDisplayName = "Bullets Remaining: ";
                    MagazineDisplayFoldout.text = "Bullets Remaining: 0";
                    ShootingProperty.style.display = DisplayStyle.Flex;
                }
                else if (e.newValue.ToString() == EGHiddenNamespace.WeaponType.Bow.ToString())
                {
                    BowContainer.style.display = DisplayStyle.Flex;
                    DependencyFoldout.text = Bow;
                    ExitVelocity.style.display = DisplayStyle.None;
                    ShotType.value = ShootingType.Projectile;
                    BulletAsset.label = "Arrow Asset";
                    ShotType.style.display = DisplayStyle.None;
                    MagazineAndCanisterFoldout.text = "Quiver";
                    RateOfFire.style.display = DisplayStyle.None;
                    TimeBetweenShots.style.display = DisplayStyle.Flex;
                    TimeBetweenShots.tooltip = "Seconds to arm bow after shooting";
                    MagazineDisplay.style.flexDirection = new StyleEnum<FlexDirection>();
                    MaxBullets.label = "Max Arrows";
                    MagazineDisplayName = "Arrows Remaining: ";
                    MagazineDisplayFoldout.text = "Arrows Remaining: 0";
                    BarrelTip.label = "Arrow Spawn Point";
                }
                else if (e.newValue.ToString() == EGHiddenNamespace.WeaponType.LaserGun.ToString())
                {
                    LaserContainer.style.display = DisplayStyle.Flex;
                    DependencyFoldout.text = Laser;
                    ExitVelocity.style.display = DisplayStyle.None;
                    DepletionRate.style.display = DisplayStyle.Flex;
                    ShotType.style.display = DisplayStyle.None;
                    ShotType.value = ShootingType.Hitscan;
                    RateOfFire.style.display = DisplayStyle.None;
                    MagazineContainer.style.display = DisplayStyle.None;
                    CanisterContainer.style.display = DisplayStyle.Flex;
                    MagazineAndCanisterFoldout.text = "PowerCell";
                    CanisterRemainingString = "Energy Remaining: ";
                    CanisterDisplayContainerFoldout.text = "Energy Remaining: 0";
                    MaxCanisterCapacityLabel.text = "Max Energy";
                    ReloadProperty.style.display = DisplayStyle.Flex;
                    BarrelTip.label = "Laser Spawn Point";
                }
                else if (e.newValue.ToString() == EGHiddenNamespace.WeaponType.Flamethrower.ToString())
                {
                    FlamethrowerContainer.style.display = DisplayStyle.Flex;
                    DependencyFoldout.text = Flamethrower;
                    ExitVelocity.style.display = DisplayStyle.None;
                    DepletionRate.style.display = DisplayStyle.Flex;
                    ShotType.style.display = DisplayStyle.None;
                    ShotType.value = ShootingType.Hitscan;
                    RateOfFire.style.display = DisplayStyle.None;
                    MagazineContainer.style.display = DisplayStyle.None;
                    CanisterContainer.style.display = DisplayStyle.Flex;
                    MagazineAndCanisterFoldout.text = "Fuel Tank";
                    CanisterRemainingString = "Fuel Remaining: ";
                    CanisterDisplayContainerFoldout.text = "Fuel Remaining: 0";
                    MaxCanisterCapacityLabel.text = "Max Fuel";
                    BarrelTip.label = "Flame Spawn Point";
                }
                else if (e.newValue.ToString() == EGHiddenNamespace.WeaponType.RailGun.ToString())
                {
                    DependencyFoldout.text = RailGun;
                }
            }
            else
            {
                if (e.previousValue.ToString() == EGHiddenNamespace.WeaponType.Automatic.ToString())
                {
                    AutomaticContainer.style.display = DisplayStyle.Flex;
                    DependencyFoldout.text = Automatic;
                    MagazineDisplay.style.flexDirection = FlexDirection.Row;
                    MaxBullets.label = "Max Bullets";
                    MagazineDisplayName = "Bullets Remaining: ";
                    MagazineDisplayFoldout.text = "Bullets Remaining: 0";
                }
                else if (e.previousValue.ToString() == EGHiddenNamespace.WeaponType.Semiautomatic.ToString())
                {
                    SemiautomaticContainer.style.display = DisplayStyle.Flex;
                    DependencyFoldout.text = Semiautomatic;
                    RateOfFire.style.display = DisplayStyle.None;
                    TimeBetweenShots.style.display = DisplayStyle.Flex;
                    TimeBetweenShots.tooltip = "Seconds between each shot";
                    TimeBetweenShots.label = "Time Between Shots";
                    MagazineDisplay.style.flexDirection = FlexDirection.Row;
                    MaxBullets.label = "Max Bullets";
                    MagazineDisplayName = "Bullets Remaining: ";
                    MagazineDisplayFoldout.text = "Bullets Remaining: 0";
                }
                else if (e.previousValue.ToString() == EGHiddenNamespace.WeaponType.Shotgun.ToString())
                {
                    ShotgunContainer.style.display = DisplayStyle.Flex;
                    DependencyFoldout.text = Shotgun;
                    MaxBullets.label = "Max Shells";
                    RateOfFire.style.display = DisplayStyle.None;
                    TimeBetweenShots.style.display = DisplayStyle.None;
                    if (ShootingProperty.value.ToString() == ShotProperty.Auto.ToString())
                    {
                        RateOfFire.style.display = DisplayStyle.Flex;
                        RateOfFire.label = "Shots per minute";
                    }
                    else
                    {
                        TimeBetweenShots.style.display = DisplayStyle.Flex;
                        TimeBetweenShots.tooltip = "Seconds between shells getting fired off";
                        TimeBetweenShots.label = "Time between shots";
                    }
                    BulletSize = 30;
                    MagazineDisplay.style.flexDirection = FlexDirection.Row;
                    MagazineDisplayName = "Shells Remaining: ";
                    MagazineDisplayFoldout.text = "Shells Remaining: 0";
                    ShootingProperty.style.display = DisplayStyle.Flex;
                }
                else if (e.previousValue.ToString() == EGHiddenNamespace.WeaponType.Burst.ToString())
                {
                    BurstContainer.style.display = DisplayStyle.Flex;
                    DependencyFoldout.text = Burst;
                    RateOfFire.style.display = DisplayStyle.None;
                    TimeBetweenShots.style.display = DisplayStyle.None;
                    if(ShootingProperty.ToString() == ShotProperty.Auto.ToString())
                        RateOfFire.style.display = DisplayStyle.Flex;
                    else
                        TimeBetweenShots.style.display = DisplayStyle.Flex;
                    TimeBetweenShots.tooltip = "Seconds between each burst cycle";
                    TimeBetweenShots.label = "Time Between Bursts";
                    MagazineDisplay.style.flexDirection = FlexDirection.Row;
                    MaxBullets.label = "Max Bullets";
                    MagazineDisplayName = "Bullets Remaining: ";
                    MagazineDisplayFoldout.text = "Bullets Remaining: 0";
                    ShootingProperty.style.display = DisplayStyle.Flex;
                }
                else if (e.previousValue.ToString() == EGHiddenNamespace.WeaponType.Bow.ToString())
                {
                    BowContainer.style.display = DisplayStyle.Flex;
                    DependencyFoldout.text = Bow;
                    ExitVelocity.style.display = DisplayStyle.None;
                    ShotType.value = ShootingType.Projectile;
                    BulletAsset.label = "Arrow Asset";
                    ShotType.style.display = DisplayStyle.None;
                    MagazineAndCanisterFoldout.text = "Quiver";
                    RateOfFire.style.display = DisplayStyle.None;
                    TimeBetweenShots.style.display = DisplayStyle.Flex;
                    TimeBetweenShots.tooltip = "Seconds to arm bow after shooting";
                    MagazineDisplay.style.flexDirection = new StyleEnum<FlexDirection>();
                    MaxBullets.label = "Max Arrows";
                    MagazineDisplayName = "Arrows Remaining: ";
                    MagazineDisplayFoldout.text = "Arrows Remaining: 0";
                    BarrelTip.label = "Arrow Spawn Point";
                }
                else if (e.previousValue.ToString() == EGHiddenNamespace.WeaponType.LaserGun.ToString())
                {
                    LaserContainer.style.display = DisplayStyle.Flex;
                    DependencyFoldout.text = Laser;
                    ExitVelocity.style.display = DisplayStyle.None;
                    DepletionRate.style.display = DisplayStyle.Flex;
                    ShotType.style.display = DisplayStyle.None;
                    ShotType.value = ShootingType.Hitscan;
                    RateOfFire.style.display = DisplayStyle.None;
                    MagazineContainer.style.display = DisplayStyle.None;
                    CanisterContainer.style.display = DisplayStyle.Flex;
                    MagazineAndCanisterFoldout.text = "PowerCell";
                    CanisterRemainingString = "Energy Remaining: ";
                    CanisterDisplayContainerFoldout.text = "Energy Remaining: 0";
                    MaxCanisterCapacityLabel.text = "Max Energy";
                    ReloadProperty.style.display = DisplayStyle.Flex;
                    BarrelTip.label = "Laser Spawn Point";
                }
                else if (e.previousValue.ToString() == EGHiddenNamespace.WeaponType.Flamethrower.ToString())
                {
                    FlamethrowerContainer.style.display = DisplayStyle.Flex;
                    DependencyFoldout.text = Flamethrower;
                    ExitVelocity.style.display = DisplayStyle.None;
                    DepletionRate.style.display = DisplayStyle.Flex;
                    ShotType.style.display = DisplayStyle.None;
                    RateOfFire.style.display = DisplayStyle.None;
                    MagazineContainer.style.display = DisplayStyle.None;
                    CanisterContainer.style.display = DisplayStyle.Flex;
                    MagazineAndCanisterFoldout.text = "Fuel Tank";
                    CanisterRemainingString = "Fuel Remaining: ";
                    CanisterDisplayContainerFoldout.text = "Fuel Remaining: 0";
                    MaxCanisterCapacityLabel.text = "Max Fuel";
                    BarrelTip.label = "Flame Spawn Point";
                }
                else if (e.previousValue.ToString() == EGHiddenNamespace.WeaponType.RailGun.ToString())
                {
                    DependencyFoldout.text = RailGun;
                }
            }
        });

        #endregion

        return root;
    }
    
    private void AdjustCharge(float Value, VisualElement childElement)
    {
        Vector3 colorValue = Vector3.Lerp(green, red, Value);
        Color childColor = new Color(colorValue.x, colorValue.y, colorValue.z);
        childElement.style.backgroundColor = childColor;
        childElement.style.flexGrow = Value;
    }

    private void AdjustFuelDisplay(float value, VisualElement childElement)
    {
        Vector3 colorValue = Vector3.Lerp(red, green, value);
        Color childColor = new Color(colorValue.x, colorValue.y, colorValue.z);
        childElement.style.backgroundColor = childColor;
        childElement.style.flexGrow = value;
    }

    private void DrawMagazineVisualDisplay(int bulletsRemaining, BulletTemplate[] allBullets)
    {
        int index = 0;
        foreach (var VARIABLE in allBullets)
        {
            if (index < bulletsRemaining)
            {
                VARIABLE.style.display = DisplayStyle.Flex;
            }
            else
            {
                VARIABLE.style.display = DisplayStyle.None;
            }
        }
    }

    private void ConfigureBurstMagDisplay(int shotsPerBurst, BulletTemplate[] allBullets)
    {
        int cycle = 1;
        for (int i = 0; i < allBullets.Length; i++)
        {
            allBullets[i].style.marginRight = 1;
            if (cycle == shotsPerBurst && shotsPerBurst != 1)
            {
                cycle = 0;
                allBullets[i].style.marginRight = 5;
            }
            cycle++;
        }
    }
}
