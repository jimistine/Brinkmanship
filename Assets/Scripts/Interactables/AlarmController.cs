using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using TMPro;

public class AlarmController : MonoBehaviour, Interactable, Attackable
{
    public TMP_Text alarmText;
    public bool alarmOn;
    public Animator alarmLight;
    public GameObject offAlarm;
    public GameObject onAlarm;
    private bool alarmShot;

    [SerializeField] private bool _IsIneractable = true;
    [HideInInspector]
    public bool isInteractable {
        get{
            return _IsIneractable;
        }
        set{
            _IsIneractable = value;
        }
    }

    [SerializeField] private bool _IsAttackable = true;
    [HideInInspector]
    public bool isAttackable {
        get{
            return _IsAttackable;
        }
        set{
            _IsAttackable = value;
        }
    }

    private void OnEnable(){
        GunManagerComponent.OnGunStateChangeEvent += TriggerAlarm;
    }
    private void OnDisable() {
        GunManagerComponent.OnGunStateChangeEvent -= TriggerAlarm;
    }

    private void TriggerAlarm(GunState oldState, GunState newState){
        if(!alarmOn && !alarmShot){
            if(newState == GunState.Firing){
                AkSoundEngine.PostEvent("Play_Gun_Alarm", gameObject);
                alarmLight.SetBool("AlarmOn", true);
                alarmText.color = Color.red;
                onAlarm.SetActive(true);
                offAlarm.SetActive(false);
                alarmOn = true;
            }
        }
    }

    public void Interact(){
        if(alarmOn && _IsIneractable){
            TurnOffAlarm();
        }
        else if(!alarmOn && _IsIneractable){
            TriggerAlarm(GunState.Firing, GunState.Firing);
        }
    }

    public void TurnOffAlarm(){
        AkSoundEngine.PostEvent("Stop_Gun_Alarm", gameObject);
        alarmOn = false;
        alarmText.color = Color.white;
    }

    public void Aimed(){
        // Maybe have the captain say something if the alarms are going off
    }

    public void Attacked(){
        alarmShot = true;
        alarmOn = false;
        _IsIneractable = false;
        alarmText.color = Color.black;
        uint callbackType = (uint)AkCallbackType.AK_EndOfEvent;
        onAlarm.SetActive(false);
        offAlarm.SetActive(true);
        AkSoundEngine.PostEvent("Shoot_Gun_Alarm", gameObject, callbackType, AlarmAttackedCallback, null);
    }
    public void AlarmAttackedCallback(object in_cookie, AkCallbackType in_type, object in_info){
        //Destroy(gameObject);
    }
}
