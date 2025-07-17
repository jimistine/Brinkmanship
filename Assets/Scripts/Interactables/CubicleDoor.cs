using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CubicleDoor : MonoBehaviour, Interactable
{

    public GameObject lockedBlocker;
    public GameObject unlockedBlocker;
    public GameObject cubicleWalls;
    public bool doorLocked = true;


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
    public void Interact(bool isThreatened){
        if(isInteractable){
            if(doorLocked){
                StartCoroutine(LockedReminder());
            }
            else if (!doorLocked){
                gameObject.GetComponent<Animator>().SetBool("CloseDoor", false);
                AkSoundEngine.PostEvent("CubicleDoor_Open", gameObject);
                StartCoroutine(LowerCubicle());
            }
        }
    }
    
    public void Stared(){
        //do nothing

    }
    // Start is called before the first frame update
    void Start()
    {
        //gameObject.GetComponent<Animator>().SetBool("CloseDoor", true);
        
    }

    IEnumerator LockedReminder(){
        AkSoundEngine.PostEvent("CubicleDoor_Locked", gameObject);
        lockedBlocker.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        lockedBlocker.SetActive(false);
        yield return new WaitForSeconds(0.25f);
        lockedBlocker.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        lockedBlocker.SetActive(false);
        yield return new WaitForSeconds(0.25f);
    }

    public void UnlockDoor(){
        AkSoundEngine.PostEvent("CubicleDoor_Unlocked", gameObject);
        doorLocked = false;
        lockedBlocker.SetActive(true);
        unlockedBlocker.SetActive(false);
    }

    public IEnumerator LowerCubicle(){
        yield return new WaitForSeconds(7.0f);
        cubicleWalls.transform.DOLocalMoveY(-3.0f, 20);
    }
}
