using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGHiddenNamespace;

namespace EGHiddenNamespace
{
    public enum WeaponType
    {
        Automatic,
        Semiautomatic,
        Burst,
        Shotgun,
        Bow,
        Flamethrower,
        LaserGun,
        RailGun
    }

    public enum ShootingType
    {
        Hitscan,
        Projectile,
    }

    public enum ShotProperty
    {
        Auto,
        Semi
    }

    public enum ReloadingProperty
    {
        Reload,
        Recharge
    }
}


[Serializable, RequireComponent(typeof(AudioSource))]
public class EasyGun : MonoBehaviour
{

    #region Variables
    
    [SerializeField] private bool IsConfigured;
    private EGExecute Execute;

    //Commands that control the weapon, can be used outside the script
    #region WeaponCommands

    /// <summary>
    /// Use this to execute a weapons actions normally, this will still follow scripts you created to control the EasyGunEventSystem
    /// </summary>
    public Actions EasyGun_Actions;
    
    /// <summary>
    /// Use this to execute an action ALWAYS! This disregards any code you created for the event system
    /// </summary>
    public ActionsOverride EasyGun_OverrideActions;

    #endregion

    //Properties that are ties to this gun, also includes the properties of each type of weapon
    #region WeaponProperties
    
    [SerializeField] public GunProperties GunProperties;
    [SerializeField] private HiddenGunProperties HiddenGunProperties;

    #endregion

    //Things for the EG Weapon Specifics
    #region EGWeaponSpecifics

    [SerializeField] private bool WeaponSpecsBool;

    #endregion

    //Variables for these specific weapon types
    #region Automatic,Semiautomatic,Shotgun

    private bool canShoot;

    #endregion

    //Things for reloading the gun
    #region Reloading
    
    private bool reloading;

    #endregion

    //The variables that include the guns magazine and canister and ammo count
    #region Magazine/Canister

    [SerializeField] private bool MagAndCanBool;
    [SerializeField] private bool MagDisplayBool;

    #endregion

    #endregion
    
    #region UnityDefinedMethods

    //assigns things before the game starts
    private void Awake()
    {
        Execute = EGHiddenManager.Manager.GetComponent<EGExecute>();
        canShoot = true;
    }

    private void Start()
    {
       // Debug.Log(HiddenGunProperties.bulletAsset);
    }

    //assigns the actions to the gun
    private void OnEnable()
    {
        EGExecuteables.ShootGun += ShotBuffer;
        EGExecuteables.ReloadGun += ReloadBuffer;
    }

    //checks for player inputs
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            EasyGun_Actions.Reload();
        }

        if (canShoot)
        {
            if (HiddenGunProperties.BulletsRemaining > 0)
            {
                if (HiddenGunProperties.WeaponType == WeaponType.Automatic)
                {
                    if (Input.GetKey(KeyCode.Mouse0))
                    {
                        EasyGun_Actions.Shoot();
                    }
                }
                else if (HiddenGunProperties.WeaponType == WeaponType.Semiautomatic)
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        EasyGun_Actions.Shoot();
                    }
                }
                else if (HiddenGunProperties.WeaponType == WeaponType.Burst)
                {
                    if (HiddenGunProperties.ShotProperty == ShotProperty.Semi && Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        EasyGun_Actions.Shoot();
                    }
                    else if (HiddenGunProperties.ShotProperty == ShotProperty.Auto && Input.GetKey(KeyCode.Mouse0))
                    {
                        EasyGun_Actions.Shoot();
                    }
                }
                else if (HiddenGunProperties.WeaponType == WeaponType.Shotgun)
                {
                    if (HiddenGunProperties.ShotProperty == ShotProperty.Semi && Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        EasyGun_Actions.Shoot();
                    }
                    else if (HiddenGunProperties.ShotProperty == ShotProperty.Auto && Input.GetKey(KeyCode.Mouse0))
                    {
                        EasyGun_Actions.Shoot();
                    }
                }
                else if (HiddenGunProperties.WeaponType == WeaponType.Bow)
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        StartCoroutine(DrawBack());
                    }
                }
            }
            else if (HiddenGunProperties.WeaponType == WeaponType.Flamethrower)
            {
                
            }
            else if (HiddenGunProperties.WeaponType == WeaponType.LaserGun)
            {
                
            }
            else if (HiddenGunProperties.WeaponType == WeaponType.RailGun)
            {
                
            }
        }
    }

    private IEnumerator DrawBack()
    {
        float ChargeTime = 0;
        while (Input.GetKey(KeyCode.Mouse0))
        {
            yield return null;
            ChargeTime += Time.deltaTime;
            HiddenGunProperties.ExitVelocity = HiddenGunProperties.BowMinCharge +
                ((ChargeTime / HiddenGunProperties.TimeToCharge) * (HiddenGunProperties.BowMaxCharge - HiddenGunProperties.BowMinCharge));
            HiddenGunProperties.ExitVelocity = Mathf.Clamp(HiddenGunProperties.ExitVelocity,
                HiddenGunProperties.BowMinCharge, HiddenGunProperties.BowMaxCharge);
        }
        HiddenGunProperties.BulletsRemaining--;
        Execute.Shoot(this);
        HiddenGunProperties.ExitVelocity = HiddenGunProperties.BowMinCharge;
    }

    //unassign the events from the gun
    private void OnDisable()
    {
        EGExecuteables.ShootGun -= ShotBuffer;
        EGExecuteables.ReloadGun -= ReloadBuffer;
    }

    #endregion
    
    //default constructor
    private EasyGun()
    {
        HiddenGunProperties = new HiddenGunProperties();
        GunProperties = new GunProperties(HiddenGunProperties);
        EasyGun_Actions = new Actions(this);
        EasyGun_OverrideActions = new ActionsOverride(this);
    }

    #region ActionMethods

    //checks if the event is for this gun, if yes then delay time between shots
    private void ShotBuffer(EasyGun EG)
    {
        //Debug.Log("Shot Gun");
        if (EG == this)
        {
            if (HiddenGunProperties.WeaponType == WeaponType.Automatic)
            {
                StartCoroutine(ShootShot());
            }
            else if (HiddenGunProperties.WeaponType == WeaponType.Semiautomatic)
            {
                StartCoroutine(ShootSingle());
            }
            else if (HiddenGunProperties.WeaponType == WeaponType.Burst)
            {
                StartCoroutine(ShootBurst());
            }
            else if (HiddenGunProperties.WeaponType == WeaponType.Shotgun)
            {
                StartCoroutine(ShootShotgun());
            }
            else if (HiddenGunProperties.WeaponType == WeaponType.Bow)
            {
                
            }
            else if (HiddenGunProperties.WeaponType == WeaponType.Flamethrower)
            {
                
            }
            else if (HiddenGunProperties.WeaponType == WeaponType.LaserGun)
            {
                
            }
            else if (HiddenGunProperties.WeaponType == WeaponType.RailGun)
            {
                
            }
        }
    }

    //delays the time in between shots
    private IEnumerator ShootShot()
    {
        canShoot = false;
        Execute.Shoot(this);
        HiddenGunProperties.BulletsRemaining--;
        yield return new WaitForSeconds(1 / HiddenGunProperties.RateOfFire);
        canShoot = true;
    }

    private IEnumerator ShootSingle()
    {
        canShoot = false;
        Execute.Shoot(this);
        HiddenGunProperties.BulletsRemaining--;
        yield return new WaitForSeconds(HiddenGunProperties.TimeBetweenShots);
        canShoot = true;
    }

    private IEnumerator ShootBurst()
    {
        canShoot = false;
        for (int i = 0; i < HiddenGunProperties.ShotsPerBurst; i++)
        {
            if(HiddenGunProperties.BulletsRemaining > 0)
            Execute.Shoot(this);
            yield return new WaitForSeconds(HiddenGunProperties.RapidTimeBetweenShots);
            HiddenGunProperties.BulletsRemaining--;
        }
        yield return new WaitForSeconds(HiddenGunProperties.TimeBetweenShots);
        canShoot = true;
    }

    private IEnumerator ShootShotgun()
    {
        canShoot = false;
        for (int i = 0; i < HiddenGunProperties.PelletsPerShot; i++)
        {
            Execute.Shoot(this);
        }
        HiddenGunProperties.BulletsRemaining--;

        if (HiddenGunProperties.ShotProperty == ShotProperty.Semi)
            yield return new WaitForSeconds(HiddenGunProperties.TimeBetweenShots);
        else
            yield return new WaitForSeconds(1 / HiddenGunProperties.RateOfFire);

        canShoot = true;
    }

    //Reads if the reload event is for this gun, if yes then toggle reload state

    private Coroutine reloadingCoroutine;
    private void ReloadBuffer(EasyGun EG)
    {
        if (EG == this)
        {
            if (reloading)
            {
                Debug.Log("cancelled reloading");
                reloading = false;
                StopCoroutine(reloadingCoroutine);
            }
            else
            {
                Debug.Log("reloading");
                reloadingCoroutine = StartCoroutine(Reloading());
            }
        }
            
    }

    //reloads the weapon
    private IEnumerator Reloading()
    {
        reloading = true;
        yield return new WaitForSeconds(HiddenGunProperties.ReloadTime);
        HiddenGunProperties.BulletsRemaining = HiddenGunProperties.MagazineCapacity;
        reloading = false;
    }

    #endregion
    
}

namespace EGHiddenNamespace
{
    [Serializable]
    public class HiddenGunProperties
    {
        #region Variables
        [SerializeField] public WeaponType WeaponType;
        [SerializeField] public ShootingType ShotType;
        [SerializeField] public ShotProperty ShotProperty;
        [SerializeField] public ReloadingProperty ReloadingProperty;

        [SerializeField] public float ReloadTime;
        [SerializeField] public float ExitVelocity;
        [SerializeField] public float Range;

        [SerializeField] public float RateOfFire;
        [SerializeField] public float DepletionPerSecond;
        [SerializeField] public float Accuracy;

        [SerializeField] public float TimeBetweenShots;
        [SerializeField] public int PelletsPerShot;
        [SerializeField] public int ShotsPerBurst;
        [SerializeField] public float RapidTimeBetweenShots;
        
        [SerializeField] public float BowMinCharge;
        [SerializeField] public float BowMaxCharge;
        [SerializeField] public float TimeToCharge;

        [SerializeField] public GameObject bulletAsset;
        [SerializeField] public GameObject BarrelTip;

        [SerializeField] public int MagazineCapacity;
        [SerializeField] public int BulletsRemaining;
        [SerializeField] public float CanisterCapacity;
        [SerializeField] public float RemainingFuel;

        [SerializeField] public float RechargeRate;
        [SerializeField] public float RechargeDelay;

        [SerializeField] public AudioClip ShotAudio;
        [SerializeField] public AudioClip FailedShotAudio;
        [SerializeField] public AudioClip ReloadAudio;

        #endregion

        #region Methods



        #endregion

    }

    [Serializable]
    public class GunProperties
    {
        /// <summary>
        /// Access the general properties of the EasyGun
        /// </summary>
        [SerializeField] public GeneralProperties GeneralProperties;
        
        /// <summary>
        /// Access the automatic properties
        /// </summary>
        [SerializeField] public Automatic AutomaticProperties;

        /// <summary>
        /// Access the semiautomatic properties
        /// </summary>
        [SerializeField] public Semiautomatic SemiautomaticProperties;
        
        /// <summary>
        /// Access burst properties
        /// </summary>
        [SerializeField] public Burst BurstGunProperties;
        
        /// <summary>
        /// Access the shotgun properties
        /// </summary>
        [SerializeField] public Shotgun ShotgunProperties;

        /// <summary>
        /// Access the bow properties
        /// </summary>
        /// <returns></returns>
        [SerializeField] public Bow BowProperties;

        /// <summary>
        /// Access the flamethrower properties
        /// </summary>
        [SerializeField]
        public Flamethrower FlamethrowerProperties;

        /// <summary>
        /// Access the laser guns properties
        /// </summary>
        [SerializeField] public LaserGun LaserGunProperties;
        
        /// <summary>
        /// Access the rail guns properties 
        /// </summary>
        [SerializeField] public RailGun RailGunProperties;

        public GunProperties(HiddenGunProperties hiddenGunProperties)
        {
            GeneralProperties = new GeneralProperties(hiddenGunProperties);
            AutomaticProperties = new Automatic(hiddenGunProperties);
            SemiautomaticProperties = new Semiautomatic(hiddenGunProperties);
            BurstGunProperties = new Burst(hiddenGunProperties);
            ShotgunProperties = new Shotgun(hiddenGunProperties);
            BowProperties = new Bow(hiddenGunProperties);
            FlamethrowerProperties = new Flamethrower(hiddenGunProperties);
            LaserGunProperties = new LaserGun(hiddenGunProperties);
            RailGunProperties = new RailGun(hiddenGunProperties);
        }
    }
    
    [Serializable]
    public class Automatic
    {
        private HiddenGunProperties Properties;
        public Automatic(HiddenGunProperties GP)
        {
            Properties = GP;
        }

        /// <summary>
        /// The seconds it takes to reload
        /// </summary>
        public float ReloadTime
        {
            get => Properties.ReloadTime;
            set => Properties.ReloadTime = value;
        }

        /// <summary>
        /// The way the gun shoots bullets: Projectile or Hitscan
        /// </summary>
        public ShootingType ShotType
        {
            get => Properties.ShotType;
        }

        /// <summary>
        /// How fast the projectile leaves the gun
        /// </summary>
        public float ExitVelocity
        {
            get => Properties.ExitVelocity;
            set => Properties.ExitVelocity = value;
        }

        /// <summary>
        /// The guns bullet asset
        /// </summary>
        public GameObject BulletAsset
        {
            get => Properties.bulletAsset;
            set => Properties.bulletAsset = value;
        }

        /// <summary>
        /// How many bullets can be stored in the guns magazine
        /// </summary>
        public int MagazineCapacity
        {
            get => Properties.MagazineCapacity;
            set => Properties.MagazineCapacity = value;
        }

        /// <summary>
        /// How many bullets are left in the guns magazine
        /// </summary>
        public int BulletsRemaining
        {
            get => Properties.BulletsRemaining;
            set => Properties.BulletsRemaining = value;
        }

        /// <summary>
        /// How far a Hitscan shot will go
        /// </summary>
        public float Range
        {
            get => Properties.Range;
            set => Properties.Range = value;
        }

        /// <summary>
        /// How many shots are fired off in a minute
        /// </summary>
        public float RateOfFire
        {
            get => Properties.RateOfFire;
            set => Properties.RateOfFire = value;
        }
    }

    [Serializable]
    public class Semiautomatic
    {
        private HiddenGunProperties Properties;
        public Semiautomatic(HiddenGunProperties GP)
        {
            Properties = GP;
        }

        /// <summary>
        /// How many seconds are between shots
        /// </summary>
        public float TimeBetweenShots
        {
            get => Properties.TimeBetweenShots;
            set => Properties.TimeBetweenShots = value;
        }

        /// <summary>
        /// The guns bullet asset
        /// </summary>
        public GameObject BulletAsset
        {
            get => Properties.bulletAsset;
            set => Properties.bulletAsset = value;
        }

        /// <summary>
        /// The way the gun shoots bullets: Projectile or Hitscan
        /// </summary>
        public ShootingType ShotType
        {
            get => Properties.ShotType;
        }

        /// <summary>
        /// How many bullets can be stored in the guns magazine
        /// </summary>
        public int MagazineCapacity
        {
            get => Properties.MagazineCapacity;
            set => Properties.MagazineCapacity = value;
        }

        /// <summary>
        /// How many bullets are left in the guns magazine
        /// </summary>
        public int BulletsRemaining
        {
            get => Properties.BulletsRemaining;
            set => Properties.BulletsRemaining = value;
        }

        /// <summary>
        /// The seconds it takes to reload
        /// </summary>
        public float ReloadTime
        {
            get => Properties.ReloadTime;
            set => Properties.ReloadTime = value;
        }

        /// <summary>
        /// How fast the projectile leaves the gun
        /// </summary>
        public float ExitVelocity
        {
            get => Properties.ExitVelocity;
            set => Properties.ExitVelocity = value;
        }

        /// <summary>
        /// How far a Hitscan shot will go
        /// </summary>
        public float Range
        {
            get => Properties.Range;
            set => Properties.Range = value;
        }
    }

    [Serializable]
    public class Burst
    {
        private HiddenGunProperties Properties;

        public Burst(HiddenGunProperties GP)
        {
            Properties = GP;
        }

        /// <summary>
        /// The way the gun shoots bullets: Projectile or Hitscan
        /// </summary>
        public ShootingType ShotType
        {
            get => Properties.ShotType;
        }

        /// <summary>
        /// The seconds it takes to reload
        /// </summary>
        public float ReloadTime
        {
            get => Properties.ReloadTime;
            set => Properties.ReloadTime = value;
        }

        /// <summary>
        /// How fast the projectile leaves the gun
        /// </summary>
        public float ExitVelocity
        {
            get => Properties.ExitVelocity;
            set => Properties.ExitVelocity = value;
        }

        /// <summary>
        /// The guns bullet asset
        /// </summary>
        public GameObject bulletAsset
        {
            get => Properties.bulletAsset;
            set => Properties.bulletAsset = value;
        }

        /// <summary>
        /// How far a Hitscan shot will go
        /// </summary>
        public float Range
        {
            get => Properties.Range;
            set => Properties.Range = value;
        }

        /// <summary>
        /// How many bullets are shot in a burst cycle
        /// </summary>
        public int ShotsPerBurst
        {
            get => Properties.ShotsPerBurst;
            set => Properties.ShotsPerBurst = value;
        }

        /// <summary>
        /// How many seconds are between burst cycles
        /// </summary>
        public float TimeBetweenBurstCycles
        {
            get => Properties.TimeBetweenShots;
            set => Properties.TimeBetweenShots = value;
        }

        /// <summary>
        /// How shooting the burst gun works: Automatic(can hold fire) or Semiautomatic(must press fire)
        /// </summary>
        public ShotProperty ShotProperty
        {
            get => Properties.ShotProperty;
            set => Properties.ShotProperty = value;
        }

        /// <summary>
        /// Time between the rounds shot inside the burst cycle
        /// </summary>
        public float TimeBetweenRounds
        {
            get => Properties.RapidTimeBetweenShots;
            set => Properties.RapidTimeBetweenShots = value;
        }

        /// <summary>
        /// How many bullets can be stored in the guns magazine
        /// </summary>
        public int MagazineCapacity
        {
            get => Properties.MagazineCapacity;
            set => Properties.MagazineCapacity = value;
        }

        /// <summary>
        /// How many bullets are left in the guns magazine
        /// </summary>
        public int BulletsRemaining
        {
            get => Properties.BulletsRemaining;
            set => Properties.BulletsRemaining = value;
        }
    }

    [Serializable]
    public class Shotgun
    {
        private HiddenGunProperties Properties;

        public Shotgun(HiddenGunProperties GP)
        {
            Properties = GP;
        }

        /// <summary>
        /// The way the gun shoots bullets: Projectile or Hitscan
        /// </summary>
        public ShootingType ShotType
        {
            get => Properties.ShotType;
        }

        /// <summary>
        /// How shooting the shotgun works: Automatic(can hold fire) or Semiautomatic(must press fire)
        /// </summary>
        public ShotProperty ShotProperty
        {
            get => Properties.ShotProperty;
        }

        /// <summary>
        /// The seconds it takes to reload
        /// </summary>
        public float ReloadTime
        {
            get => Properties.ReloadTime;
            set => Properties.ReloadTime = value;
        }

        /// <summary>
        /// How many pellets are in each shell
        /// </summary>
        public int PelletsPerShot
        {
            get => Properties.PelletsPerShot;
            set => Properties.PelletsPerShot = value;
        }

        /// <summary>
        /// How many seconds are between shots
        /// </summary>
        public float TimeBetweenShots
        {
            get => Properties.RateOfFire;
            set => Properties.RateOfFire = value;
        }

        /// <summary>
        /// How far a Hitscan shot will go
        /// </summary>
        public float Range
        {
            get => Properties.Range;
            set => Properties.Range = value;
        }

        /// <summary>
        /// The guns bullet asset
        /// </summary>
        public GameObject BulletAsset
        {
            get => Properties.bulletAsset;
            set => Properties.bulletAsset = value;
        }

        /// <summary>
        /// How many shells can be stored in the guns magazine
        /// </summary>
        public int MagazineCapacity
        {
            get => Properties.MagazineCapacity;
            set => Properties.MagazineCapacity = value;
        }

        /// <summary>
        /// How many shells are left in the guns magazine
        /// </summary>
        public int ShellsRemaining
        {
            get => Properties.BulletsRemaining;
            set => Properties.BulletsRemaining = value;
        }

        /// <summary>
        /// How fast the pellets leaves the gun
        /// </summary>
        public float ExitVelocity
        {
            get => Properties.ExitVelocity;
            set => Properties.ExitVelocity = value;
        }

        
    }
    
    [Serializable]
    public class Bow
    {
        private HiddenGunProperties Properties;

        public Bow(HiddenGunProperties GP)
        {
            Properties = GP;
        }

        /// <summary>
        /// The bows arrow asset
        /// </summary>
        public GameObject ArrowAsset
        {
            get => Properties.bulletAsset;
            set => Properties.bulletAsset = value;
        }

        /// <summary>
        /// How many seconds are between arrow shot and rearm
        /// </summary>
        public float TimeBetweenShots
        {
            get => Properties.TimeBetweenShots;
            set => Properties.TimeBetweenShots = value;
        }

        /// <summary>
        /// The minimum force the arrow will have when released
        /// </summary>
        public float MinimumForce
        {
            get => Properties.BowMinCharge;
            set => Properties.BowMinCharge = value;
        }

        /// <summary>
        /// The maximum force the arrow will have when released
        /// </summary>
        public float MaximumForce
        {
            get => Properties.BowMaxCharge;
            set => Properties.BowMaxCharge = value;
        }

        /// <summary>
        /// How many arrows can be stored in the bows quiver
        /// </summary>
        public int QuiverCapacity
        {
            get => Properties.MagazineCapacity;
            set => Properties.MagazineCapacity = value;
        }

        /// <summary>
        /// How many arrows are left in the bows quiver
        /// </summary>
        public int RemainingArrows
        {
            get => Properties.BulletsRemaining;
            set => Properties.BulletsRemaining = value;
        }

        /// <summary>
        /// How fast the arrow leaves the bow
        /// </summary>
        public float ReleaseForce
        {
            get => Properties.ExitVelocity;
        }

        /// <summary>
        /// The seconds it takes to reload
        /// </summary>
        public float ReloadTime
        {
            get => Properties.ReloadTime;
            set => Properties.ReloadTime = value;
        }
        
        /// <summary>
        /// How long it takes to charge the bow to max force
        /// </summary>
        public float TimeToCharge
        {
            get => Properties.TimeToCharge;
            set => Properties.TimeToCharge = value;
        }
        
    }

    [Serializable]
    public class Flamethrower
    {
        private HiddenGunProperties Properties;

        public Flamethrower(HiddenGunProperties GP)
        {
            Properties = GP;
        }

        public float Range
        {
            get => Properties.Range;
            set => Properties.Range = value;
        }

        public float DepletionPerSecond
        {
            get => Properties.DepletionPerSecond;
            set => Properties.DepletionPerSecond = value;
        }

        public float MaxFuel
        {
            get => Properties.CanisterCapacity;
            set => Properties.CanisterCapacity = value;
        }

        public float RemainingFuel
        {
            get => Properties.RemainingFuel;
            set => Properties.RemainingFuel = value;
        }

        /// <summary>
        /// The seconds it takes to reload
        /// </summary>
        public float ReloadTime
        {
            get => Properties.ReloadTime;
            set => Properties.ReloadTime = value;
        }

    }

    [Serializable]
    public class LaserGun
    {
        private HiddenGunProperties Properties;

        public LaserGun(HiddenGunProperties GP)
        {
            Properties = GP;
        }

        public float Range
        {
            get => Properties.Range;
            set => Properties.Range = value;
        }
        
        public float DepletionRate
        {
            get => Properties.DepletionPerSecond;
            set => Properties.DepletionPerSecond = value;
        }

        public float MaximumEnergy
        {
            get => Properties.CanisterCapacity;
            set => Properties.CanisterCapacity = value;
        }

        public float RemainingEnergy
        {
            get => Properties.RemainingFuel;
            set => Properties.RemainingFuel = value;
        }

        public float ReloadTime
        {
            get => Properties.ReloadTime;
            set => Properties.ReloadTime = value;
        }

        public ReloadingProperty ReloadingProperty
        {
            get => Properties.ReloadingProperty;
        }

        public float RechargeRate
        {
            get => Properties.RechargeRate;
            set => Properties.RechargeRate = value;
        }

        public float RechargeDelay
        {
            get => Properties.RechargeDelay;
            set => Properties.RechargeDelay = value;
        }
        
    }

    [Serializable]
    public class RailGun
    {
        private HiddenGunProperties Properties;

        public RailGun(HiddenGunProperties GP)
        {
            Properties = GP;
        }

        public float ReloadTime
        {
            get => Properties.ReloadTime;
            set => Properties.ReloadTime = value;
        }

        public ShootingType ShotType
        {
            get => Properties.ShotType;
        }

        /// <summary>
        /// How fast the projectile leaves the gun
        /// </summary>
        public float ExitVelocity
        {
            get => Properties.ExitVelocity;
            set => Properties.ExitVelocity = value;
        }

        public GameObject BulletAsset
        {
            get => Properties.bulletAsset;
            set => Properties.bulletAsset = value;
        }


    }

    [Serializable]
    public class GeneralProperties
    {
        private HiddenGunProperties Properties;

        public GeneralProperties(HiddenGunProperties GP)
        {
            Properties = GP;
        }

        /// <summary>
        /// The type of weapon the EasyGun is
        /// </summary>
        public WeaponType WeaponType
        {
            get => Properties.WeaponType;
        }

        /// <summary>
        /// 
        /// </summary>
        public GameObject BarrelTip
        {
            get => Properties.BarrelTip;
            set => Properties.BarrelTip = value;
        }

        public float Accuracy
        {
            get => Properties.Accuracy;
            set => Properties.Accuracy = value;
        }


    }

    //Actions class is a class that sends out gun events when called
    [Serializable]
    public class Actions
    {
        private EasyGun EG;

        public Actions(EasyGun _eg)
        {
            EG = _eg;
        }

        /// <summary>
        /// Sends an event to shoot the EasyGun
        /// </summary>
        public void Shoot()
        {
            EGExecuteables.EasyGunShot(EG);
        }

        /// <summary>
        /// Sends an event to reload the EasyGun
        /// </summary>
        public void Reload()
        {
            EGExecuteables.EasyGunStartedReloading(EG);
        }
    }

    //ActionsOverride class will surpass the event system and force the gun to commit an action
    [Serializable]
    public class ActionsOverride
    {
        private EasyGun EG;

        public ActionsOverride(EasyGun _eg)
        {
            EG = _eg;
        }

        /// <summary>
        /// Forces the EasyGun to shoot
        /// </summary>
        public void Shoot()
        {
            Debug.Log("force shot");
            EGExecuteables.Execute.Shoot(EG);
        }

        /// <summary>
        /// Forces the EasyGun to reload
        /// </summary>
        public void Reload()
        {
            //Debug.Log(EG.EasyGun_Actions);
            Debug.Log("force reload");
            //EGExecuteables.Execute.Reload(EG);
        }
    }

}

