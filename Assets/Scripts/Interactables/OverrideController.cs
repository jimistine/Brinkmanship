using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using TMPro;

public class OverrideController : MonoBehaviour, Interactable, Attackable
{
    public bool overrideEngaged;
    private bool overrideShot;
    private BridgeManager bridgeManager;
    public TMP_Text overrideText;
    public Material onMat;
    public MeshRenderer lightMesh;
    public GameObject onLight;
    public GameObject offLight;

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

   

    void Awake(){
        bridgeManager = FindAnyObjectByType<BridgeManager>();
    }

    public void Interact(bool isThreatened){
        if(isInteractable){
            if(DialogueManager.GetVariable("override_code_intel") == "true"){
                EngageOverride();
            }
            else{
                StartCoroutine(LockedReminder());
            }
        }
    }

    private void EngageOverride(){
        Debug.Log("Override Engaged");
        overrideEngaged = true;
        bridgeManager.BeginTurningSequence();
        offLight.SetActive(false);
        onLight.SetActive(true);
        lightMesh.material = onMat;
        if(DialogueManager.GetVariable("pilot") == "living"){
            DialogueManager.InvokeYSEvent("override_pilot_living", "Kase");
        }
    }

    IEnumerator LockedReminder(){
        AkSoundEngine.PostEvent("CubicleDoor_Locked", gameObject);
        offLight.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        offLight.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        offLight.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        offLight.SetActive(true);
    }

    public void Attacked(){
        overrideShot = true;
        _IsIneractable = false;
        overrideText.color = Color.black;
        AkSoundEngine.PostEvent("Hit_Metal", gameObject);
    }
    
    public void Stared(){
        //do nothing

    }
    public void Aimed(){
        // Maybe have the captain say something if the alarms are going off
    }
}
