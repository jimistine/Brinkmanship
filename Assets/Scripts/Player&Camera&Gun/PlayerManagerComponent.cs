using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityPhysics;
using UnityEngine;
using Cinemachine;
using UnityEngine.VFX;


public class PlayerManagerComponent : MonoBehaviour
{
    [System.NonSerialized]
    public GunManagerComponent gunManagerComponent;
    [System.NonSerialized]
    public CameraRayComponent cameraRayComponent;

    [Header("Gun FX")]
    public CinemachineVirtualCamera playerVCam;
    public CinemachineImpulseSource impulseSource;
    public VisualEffect muzzleFlash;
    public GameObject gunLight;
    

    [SerializeField] private int _bulletCount = 6;
    public int bulletCount {
        get {
            return _bulletCount;
        }
        set {
            if (value != _bulletCount) {
                OnBulletCountChangeEvent?.Invoke(value);
                _bulletCount = value;

                if (_bulletCount == 0) {
                    OnBulletCountZeroEvent?.Invoke();
                }
            }
        }
    }
    /// <summary>
    /// This delegate is triggered when the player's bullet count changes.
    /// </summary>
    /// <param name="bulletCount"> bullet count </param>
    public delegate void OnBulletCountChange(int bulletCount);
    public static event OnBulletCountChange OnBulletCountChangeEvent;
    
    /// <summary>
    /// This delegate is triggered when the player's bullet count reaches zero.
    /// </summary>
    public delegate void OnBulletCountZero();
    public static event OnBulletCountZero OnBulletCountZeroEvent;

    [SerializeField] private List<GameObject> _inventory = new List<GameObject>();
    public List<GameObject> collectedObjects {
        get {
            return _inventory;
        }
        set {
            if (value != _inventory) {
                if (_inventory.Count > value.Count && value.Count > 0) {
                    OnInventoryChangeEvent?.Invoke(value, value[value.Count - 1]);
                } else {
                    OnInventoryChangeEvent?.Invoke(value);
                }
                _inventory = value;
            }
        }
    }

    /// <summary>
    /// This delegate is triggered when the player's inventory changes.
    /// </summary>
    /// <param name="inventory"> a list of gameobject in the inventory</param>
    /// <param name="addedObject"> the object that was added to the inventory </param>
    public delegate void OnInventoryChange(List<GameObject> inventory, GameObject addedObject = null);
    public static event OnInventoryChange OnInventoryChangeEvent;

    private void OnEnable(){
        GunManagerComponent.OnGunStateChangeEvent += PostGunVFX;
    }
    private void OnDisable() {
        GunManagerComponent.OnGunStateChangeEvent -= PostGunVFX;
    }

    void PostGunVFX(GunState oldState, GunState newState){
        if(newState == GunState.Firing){
            impulseSource.GenerateImpulse();
            muzzleFlash.Play();
            StartCoroutine(GunFX());
        }
    }
    IEnumerator GunFX(){
        gunLight.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gunLight.SetActive(false);
    }

    public void AddToInventory(GameObject o)
    {
        collectedObjects.Add(o);
    }

    static GameObject playerObject = null;

    void Start()
    {

        if (playerObject != null)
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            DontDestroyOnLoad(transform.parent.gameObject);
            playerObject = transform.parent.gameObject;
        }
    }
}
