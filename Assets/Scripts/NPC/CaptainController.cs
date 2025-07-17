using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptainController : MonoBehaviour
{

    private Animator captainAnimator;

    private void OnEnable(){
        GunManagerComponent.OnGunStateChangeEvent += UpdateCapState;
    }
    private void OnDisable() {
        GunManagerComponent.OnGunStateChangeEvent -= UpdateCapState;
    }

    void Awake(){
        captainAnimator = GetComponent<Animator>();
        // GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        // playerVCam = mainCamera.GetComponentInChildren<CinemachineVirtualCamera>();
    }

    void UpdateCapState(GunState oldState, GunState newState){
        if(DialogueManager.GetVariable("captain") != "dead"){
            if(newState == GunState.Raised){
                captainAnimator.SetBool("Raise_Hands", true);
                //Debug.Log("Put em up!!!");
            }
            if(newState == GunState.Holstered){
                captainAnimator.SetBool("Raise_Hands", false);
            }
            if(newState == GunState.Firing){
                captainAnimator.SetBool("Shield_Body", true);
                StartCoroutine(SetAnimBool("Shield_Body", false));
            }
        }
    }
    IEnumerator SetAnimBool(string name, bool value){
        yield return new WaitForSeconds(0.25f);
        captainAnimator.SetBool(name, value);   
    }
}
