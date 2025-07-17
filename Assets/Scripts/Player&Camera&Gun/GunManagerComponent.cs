using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using DG.Tweening;

/// <summary>
/// Enum for gun state.
/// </summary>
public enum GunState
{
    Holstered,
    Raised,
    Aiming,
    Firing,
    FiringNoAmmo
}

public class GunManagerComponent : MonoBehaviour
{

    public GameObject gun;
    public bool gunEnabled;
    private bool playedNoAmmo;
    private VisualEffect muzzleFlash;
    private Tween recoilTween;
    public GameObject bulletUI;
    
    [System.NonSerialized]
    public Animator animator;
    public Animator viewAnimator;
    public Animator viewParentAnimator;
    private Transform gunTransform;
    private GameObject player;
    public StarterAssetsInputs input;
    /// <summary> Obselted, please use Fire() instead. </summary> 
    private bool shouldCheckFire = true;
    private PlayerManagerComponent playerManagerComponent;
    [SerializeField] public GunState _gunState;

    public GunState gunState {
        get {
            return _gunState;
        }
        set {
            if (value != _gunState) {
                OnGunStateChangeEvent?.Invoke(_gunState, value);
                _gunState = value;
                //Debug.Log("Gun state changed to " + value);
            }
        }
    }
    /// <summary>
    /// This delegate is triggered when the player's gun state changes.
    /// </summary>
    /// <param name="gunState"> Enum of gunstate </param>
    public delegate void OnGunStateChange(GunState oldState, GunState newState);
    public static event OnGunStateChange OnGunStateChangeEvent;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnEnable(){
        GunManagerComponent.OnGunStateChangeEvent += _OnGunStateChange;
    }
    private void OnDisable() {
        GunManagerComponent.OnGunStateChangeEvent -= _OnGunStateChange;
    }

    // Start is called before the first frame update
    void Start()
    {
        input = player.GetComponent<StarterAssetsInputs>();
        animator = gun.GetComponent<Animator>();
        gunTransform = gun.GetComponent<Transform>();
        playerManagerComponent = player.GetComponent<PlayerManagerComponent>();
        playerManagerComponent.gunManagerComponent = gameObject.GetComponent<GunManagerComponent>();
        muzzleFlash = GetComponentInChildren<VisualEffect>();

    }

    // Update is called once per frame
    void Update()
    {
        Weapon();
        gunState = GetGunState();
    }

    /// <summary>
    /// Update weapon and weapon animator.
    /// </summary>
    private void Weapon()
    {
        // Raise weapon when 'aim' is pressed.
        // if (input.aim) {
        //     animator.SetBool("raise", true);
        // }

        // Raise weapon when 'r' or right click is pressed and held.
        //Debug.Log(input.raise);
        if(!gunEnabled){
            input.raise = false;
        }
        if (input.raise && gunState != GunState.Raised) {
            animator.SetBool("raise", true);
            viewAnimator.SetBool("GunDrawn", true);
            input.raise = false;
            //Debug.Log("Trigger");
        }

        bool isReadyToAim = animator.GetCurrentAnimatorStateInfo(0).IsName("Gun_idle")     || 
                            animator.GetCurrentAnimatorStateInfo(0).IsName("Gun_aim")      || 
                            animator.GetCurrentAnimatorStateInfo(0).IsName("Gun_aim_idle");

        // Aim weapon when 'aim' is pressed and weapon is already raised
        if (input.aim && isReadyToAim) {
            animator.SetBool("aim", true);
        } else {
            // Lower when aim when 'aim' is released
            animator.SetBool("aim", false);
        }

        // Fire weapon when 'fire' is pressed
        if (input.fire && (animator.GetBool("raise") || animator.GetBool("fire")) && playerManagerComponent.bulletCount > 0) {
            animator.SetBool("fire", true);
            viewAnimator.SetTrigger("GunFired");
            viewParentAnimator.SetTrigger("GunFired_Parent");
            //recoilTween = viewAnimator.gameObject.transform.DORotate(new Vector3(-10,0,0), 0.15f, RotateMode.LocalAxisAdd).OnComplete(ResetFPVRotation).SetAutoKill(false); 
            shouldCheckFire = false;
        }
        else if(input.fire && playerManagerComponent.bulletCount <= 0){
            Debug.Log("fire no ammo");
            if(!playedNoAmmo){
                playedNoAmmo = true;
                AkSoundEngine.PostEvent("GunFireNoAmmo", gameObject);
            }
            //_OnGunStateChange(GunState.Firing, GunState.FiringNoAmmo);
        }
        else {
            animator.SetBool("fire", false);
            viewAnimator.ResetTrigger("GunFired");
            viewParentAnimator.ResetTrigger("GunFired_Parent");
            playedNoAmmo = false;
        }

        // When player is sprinting, jumping, or 'raise' is pressed again, lower the weapon
        if ((input.sprint || input.jump) && animator.GetBool("raise")) {
            animator.SetBool("raise", false);
            input.raise = false;
        }
    }
    public void ResetFPVRotation(){
        //recoilTween.PlayBackwards();
        //viewAnimator.gameObject.transform.DORotate(new Vector3(10,0,0), 0.2f, RotateMode.LocalAxisAdd);
    }

    /// <summary>
    /// Fire check the rotation of the gun, when gun is pivioting up it will call FireRay() function
    /// </summary>
    [Obsolete("FireCheck is deprecated, please use Fire instead.")]
    private void FireCheck()
    {
        //print(transform.localRotation.x);
        if (gunTransform.localRotation.x < -0.85f && shouldCheckFire && playerManagerComponent.bulletCount > 0) {
            FireRay();
            shouldCheckFire = false;
            playerManagerComponent.bulletCount --;
        } else if (gunTransform.localRotation.x > -0.8f) {
            shouldCheckFire = true;
        }
    }
    
    void _OnGunStateChange(GunState oldState, GunState newState) {
        if (newState == GunState.Firing) {
            Fire();
        } 
        else if (newState == GunState.FiringNoAmmo){
            InvokeYSGunThreatEvent("firing-no-ammo"); 
        }
        else if (newState == GunState.Aiming && oldState == GunState.Raised){ 
            InvokeYSGunThreatEvent("aimed"); 
        } 
        else if (newState == GunState.Holstered){
            InvokeYSGunThreatEvent("holstered");
        }
        else if (newState == GunState.Raised && oldState == GunState.Holstered){
            InvokeYSGunThreatEvent("raised"); 
        }
    }
    
    // holstered, raised, aimed, firing-no-ammo, hit, missed, hit-attackable-disabled
    void InvokeYSGunThreatEvent(String stateValue) {
        DialogueManager.SetVariable(variableName: "gun-threat-state", value: stateValue);
        DialogueManager.InvokeYSEvent(eventName: stateValue, null);
    }

    public void Fire()
    {
        if (playerManagerComponent.bulletCount > 0) {
            FireRay();
            playerManagerComponent.bulletCount --;
        }
    }

    private void FireRay()
    {
        Ray ray =  Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hitData;
        if (Physics.Raycast(ray, out hitData)) 
        {
            
            GameObject targetObject = hitData.transform.gameObject;
            Debug.Log("Hit " + hitData.collider.material.name);
            if(hitData.collider.material.name == "Metal (Instance)"){
                AkSoundEngine.PostEvent("Hit_Metal", targetObject);
                Debug.Log("Played metal impact on" + targetObject.name);
            }
            
            Attackable attacktable = targetObject.GetComponent<Attackable>();
            if (attacktable == null) {
                Debug.Log("Gun hit something but that's not attackable");
                InvokeYSGunThreatEvent("missed");
                return;
            }
            else if (!attacktable.isAttackable) {
                Debug.Log("Gun hit an attackable that's disabled attackable");
                InvokeYSGunThreatEvent("hit-attackable-disabled");
                return;
            }
            print("Gun hit attackable object");
            DialogueManager.SetVariable(variableName: "gun-threat-state", value: "hit");
            attacktable?.Attacked();
        }
        else
        {
            InvokeYSGunThreatEvent("missed");
        }
    }

    /// <summary>
    /// Get the current gun state. This function is tied to the animator state.
    /// Note, firing state is not detect here, firing state is detect in in Weapon() function to minimize delay().
    /// This is because Weapon is tied to player input, while this function is tied to animator state.
    /// </summary>
    /// <returns>Gunstate</returns>
    private GunState GetGunState() {

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Gun_raise") || stateInfo.IsName("Gun_idle")) {
            return GunState.Raised;
        }

        if (stateInfo.IsName("Gun_aim") || stateInfo.IsName("Gun_aim_idle")) {
            return GunState.Aiming;
        }
        
        if (stateInfo.IsName("Gun_fire")) {
            if (playerManagerComponent.bulletCount <= 0) {
                return GunState.FiringNoAmmo;
            }
            return GunState.Firing;
        }

        return GunState.Holstered;
    }
}