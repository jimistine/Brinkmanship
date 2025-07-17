using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class NPCAudioController : MonoBehaviour
{

    public GameObject gunslideTarget;
    private bool hummingStopped;
    private bool domesticStopped;
    private bool tvStopped;
    private bool spritzStopped;
    private bool typingStopped;

    void OnEnable(){
        CinematicsCues.StartCharacterAmbsEvent += StartBaseQuartersLayer;
    }
    void OnDisable(){
        CinematicsCues.StartCharacterAmbsEvent -= StartBaseQuartersLayer;
    }
// Quarters Ambients
    public void StartBaseQuartersLayer(){
        AkSoundEngine.PostEvent("Play_Quarters_Clean", gameObject);
    }

    public void StartInteractLayer(){
        if(typingStopped){
            AkSoundEngine.PostEvent("Play_Quarters_Comp_Interact_Layer", gameObject);
            typingStopped = false;
        }
    }
    public void StopInteractLayer(){
        AkSoundEngine.PostEvent("Stop_Quarters_Comp_Interact_Layer", gameObject);
        typingStopped = true;
    }
   
    public void StartTVLayer(){
        if(tvStopped){
            AkSoundEngine.PostEvent("Play_Quarters_Soccer_Match", gameObject);
            tvStopped = false;        
        }
    }
    [YarnCommand("Stop_TV_Layer")]
    public void StopTVLayer(){
        AkSoundEngine.PostEvent("Stop_Quarters_Soccer_Match", gameObject);
        tvStopped = true;
    }

    [YarnCommand("Stop_Sprtiz_Layer")]
    public void StopSpritzLayer(){
        AkSoundEngine.PostEvent("Stop_Brinkmanship_Plant_Spritizng_Layer", gameObject);
        spritzStopped = true;
    }
    public void StartSpritzLayer(){
        if(spritzStopped){
            AkSoundEngine.PostEvent("Play_Brinkmanship_Plant_Spritizng_Layer", gameObject);
            spritzStopped = false;
        }
    }

    [YarnCommand("Stop_Domestic_Layer")]
    public void StopDomesticLayer(){
        AkSoundEngine.PostEvent("Stop_Brinkmanship_Domestic_Layer", gameObject);
        domesticStopped = true;
    }
    public void StartDomesticLayer(){
        if(domesticStopped){
            AkSoundEngine.PostEvent("Play_Brinkmanship_Domestic_Layer", gameObject);
            domesticStopped = false;
        }
    }

    [YarnCommand("Stop_Humming_Layer")]
    public void StopHummingLayer(){
        AkSoundEngine.PostEvent("Stop_Prisoner_Humming", gameObject);
        hummingStopped = true;
    }
    public void StartHummingLayer(){
        if(hummingStopped){
            AkSoundEngine.PostEvent("Play_Prisoner_Humming", gameObject);
            hummingStopped = false;
        }
    }

// Foley
    [YarnCommand("Play_Search_Foley")]
    public void PlaySearchFoley(){
        AkSoundEngine.PostEvent("Play_finding_gun_foley", gameObject);
        //Debug.Log("stopping soccer");
    }
    [YarnCommand("Stop_Search_Foley")]
    public void StopSearchFoley(){
        AkSoundEngine.PostEvent("Stop_finding_gun_foley", gameObject);
        //Debug.Log("stopping soccer");
    }

    [YarnCommand("Play_GunSlide")]
    public void PlayGunSlide(){
        AkSoundEngine.PostEvent("Play_gun_slide_draft_3", gunslideTarget);
       
    }
}
