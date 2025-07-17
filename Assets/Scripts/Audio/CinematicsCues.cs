using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CinematicsCues : MonoBehaviour
{
    public GameObject bioPanel;
    private GameObject player;

    // Static events
    public delegate void StartCharacterAmbs();
    public static event StartCharacterAmbs StartCharacterAmbsEvent;
    private VoidSpaceManager voidManager;

    void Start(){
        player = GameObject.FindGameObjectWithTag("Player");
        if(GameManager.GetCurrentScene() == "02_Void"){
            voidManager = GameObject.Find("VoidSpaceManager").GetComponent<VoidSpaceManager>();
        }
        //voidManager.Toggle_Ailyn_Ping();

    }

    public void PlayTypes(){
        AkSoundEngine.PostEvent("Play_Types", gameObject);
    }

    public void PlayType_Single(){
        AkSoundEngine.PostEvent("Play_Type", gameObject);
    }
    public void PlayType_Single_Dialogue(){
        AkSoundEngine.PostEvent("Play_Type_Dialogue", gameObject);
    }
    public void PlayType_Single_Dialogue_Bridge(){
        AkSoundEngine.PostEvent("Play_Type_Dialogue_Bridge", gameObject);
    }
    public void PlayType_Single_Bio(){
        AkSoundEngine.PostEvent("Play_Type_Bio", bioPanel);
    }

    public void StopAmbs(){
        AkSoundEngine.PostEvent("stop_engine_ambient", gameObject);
        AkSoundEngine.PostEvent("stop_void_amb_01", gameObject);
    }
    
    public void StartAmbs(){
        AkSoundEngine.PostEvent("play_engine_ambient", gameObject);
        AkSoundEngine.PostEvent("play_void_amb_01", gameObject);
    }

    public void EnablePlayerInput(){
        player.GetComponent<PlayerInput>().enabled = true;
    }
    public void DisablePlayerInput(){
        player.GetComponent<PlayerInput>().enabled = false;
    }

    public void TogglePing(){
        voidManager.Toggle_Ailyn_Ping();
    }
}
