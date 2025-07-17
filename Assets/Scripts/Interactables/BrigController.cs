using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BrigController : MonoBehaviour, Interactable
{
    public GameObject brigGeo;
    public GameObject elevatorDoor;
    public GameObject elevatorDoorReturn;
    public GameObject prisonersDoor;
    private bool inBrig;
    public GameObject blackScreen;
    public AK.Wwise.State inBrigState;
    public AK.Wwise.State outOfBrigState;


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

    void Start(){
        outOfBrigState.SetValue();
    }

    public void Interact(bool isThreatened){
        StartCoroutine(BrigTransitionSequence());
    }

    public IEnumerator BrigTransitionSequence(){
        if(!inBrig){
        // Enter Brig Deck
            inBrig = true;
            inBrigState.SetValue();
            brigGeo.SetActive(true);
            elevatorDoor.SetActive(false);
            elevatorDoorReturn.SetActive(true);
            prisonersDoor.SetActive(true);
            gameObject.GetComponents<CapsuleCollider>()[0].enabled = false;
            gameObject.GetComponents<CapsuleCollider>()[1].enabled = true;
        }
        else{
        // Exit Brig Deck
            inBrig = false;
            outOfBrigState.SetValue();
            brigGeo.SetActive(false);
            elevatorDoor.SetActive(true);
            elevatorDoorReturn.SetActive(false);
            gameObject.GetComponentInChildren<NPCAudioController>().StopHummingLayer();
            prisonersDoor.SetActive(false);
            gameObject.GetComponents<CapsuleCollider>()[0].enabled = true;
            gameObject.GetComponents<CapsuleCollider>()[1].enabled = false;
        }
        blackScreen.SetActive(true);
        AkSoundEngine.PostEvent("Play_elevator_hum", gameObject);
        yield return new WaitForSeconds(7f);
        blackScreen.SetActive(false);

    }

    
    public void Stared(){
        //do nothing
    }
}
