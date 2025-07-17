using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Yarn.Unity;

public class PlayerAudioManager : MonoBehaviour
{
    public StarterAssetsInputs input;
    public FirstPersonController playerController;
    [Header("FootSteps")]
    public float defaultFSTiming;
    public float sprintFSTiming;
    private float fsTimer = 0;

    private void OnEnable(){
        GunManagerComponent.OnGunStateChangeEvent += PostGunSFX;
    }
    private void OnDisable() {
        GunManagerComponent.OnGunStateChangeEvent -= PostGunSFX;
    }
    
    void Awake()
    {
        input = FindObjectOfType<StarterAssetsInputs>();
        playerController = FindObjectOfType<FirstPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        DoFootSteps();

        // Testing
        // if(Input.GetKeyDown(KeyCode.Alpha0)){
        //     AkSoundEngine.PostEvent("GunFireNoAmmo", gameObject);
        // }
    }

    public void DoFootSteps(){
        if (input.move == Vector2.zero){
            fsTimer = 0f;
            return;
        }
        if(!playerController.Grounded) return;

        fsTimer -= Time.deltaTime;
        if(fsTimer <= 0){
            AkSoundEngine.PostEvent("Plr_Walk", gameObject);
            if(input.sprint){
                fsTimer = sprintFSTiming;
            }
            else{
                fsTimer = defaultFSTiming;
            }
        }
    }

    void PostGunSFX(GunState oldState, GunState newState){
        if(newState == GunState.Firing){
            AkSoundEngine.PostEvent("GunFire", gameObject);
        }
        else if(newState == GunState.Raised && oldState == GunState.Holstered){
            AkSoundEngine.PostEvent("GunDraw", gameObject);
        }
        else if(newState == GunState.Holstered){
            AkSoundEngine.PostEvent("GunStow", gameObject);
        }
        else if(newState == GunState.FiringNoAmmo){
            AkSoundEngine.PostEvent("GunFireNoAmmo", gameObject);
            Debug.Log("Fire no ammo *click*");
        }
    }

    [YarnCommand("Knock_Random")]
    public void Knock_Random()
    {
        AkSoundEngine.PostEvent("Knock_Random", gameObject);
    }
    [YarnCommand("Knock_2")]
    public void Knock_2()
    {
        AkSoundEngine.PostEvent("Knock_2", gameObject);
    }
    [YarnCommand("Knock_Random_Deeper")]
    public void Knock_Rand_Deeper()
    {
        AkSoundEngine.PostEvent("Knock_Rand_Deeper", gameObject);
    }
}
