using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class AirlockManager : MonoBehaviour
{
    public bool loadAtStart;
    public GameObject invisCube;
    public GameObject bridgeDoors_Bridge;
    public GameObject airlockObj;
    public Animator bridgeSideDoorsAnim;
    
    // void Update(){
    //     if(Input.GetKeyDown(KeyCode.N)){
    //         StartCoroutine(OpenBridgeSideDoors(true));
    //     }
    // }

    void OnEnable(){
        DoorEnterHandler.onLoadBridge += UpdateScene;
    }
    void OnDisable(){
        DoorEnterHandler.onLoadBridge -= UpdateScene;
    }

    void UpdateScene(){
       // invisCube.SetActive(false);
        bridgeDoors_Bridge.SetActive(true);
        StartCoroutine(TriggerAirlockCovo());
    }
    IEnumerator TriggerAirlockCovo(){
        yield return new WaitForSeconds(1.5f);
        DialogueManager.InvokeYSEvent("3-1_Airlock", null);
    }

    void Start(){
        if(!loadAtStart){
            invisCube.SetActive(false);
            airlockObj.SetActive(false);
        }
    }

    public void AddAirlock(){
        invisCube.SetActive(true);
        airlockObj.SetActive(true);
    }

    [YarnCommand("OpenBridgeSideDoors")]
    public void OpenBridgeSideDoors(){
        StartCoroutine(OpenBridgeSideDoors(true));
    }
    IEnumerator OpenBridgeSideDoors(bool open){
        BridgeManager BM = FindObjectOfType<BridgeManager>();
        if (open){
            BM.BeginEntry();
            bridgeSideDoorsAnim.SetBool("BridgeDoor_Bridge_Open", true);
            AkSoundEngine.PostEvent("Play_BridgeDoorsComp_01", gameObject);

            // AkSoundEngine.PostEvent("Unmute_Track_01", gameObject);
            // AkSoundEngine.PostEvent("Unmute_Track_02", gameObject);
            GameManager.GM.musicManager.PostMusicEvent("Play_Airlock_Out");
            GameManager.GM.musicManager.PostMusicEvent("Unmute_Track_01");
            GameManager.GM.musicManager.PostMusicEvent("Unmute_Track_02");

            //GameManager.GM.EnableGun();
        }
        else if(!open){
            bridgeSideDoorsAnim.SetBool("BridgeDoor_Bridge_Open", false);
            AkSoundEngine.PostEvent("Door_Spawn", gameObject);

        }
        yield return new WaitForSeconds(0.0f);
    }
    private bool triggered;
    private void OnTriggerEnter(Collider other)
    {   
        if(!triggered){
            if (other.CompareTag("Player"))
            {
                StartCoroutine(OpenBridgeSideDoors(false));
            }
            //DialogueManager.InvokeYSEvent("Bridge_Start", null);
            GameObject captain = GameObject.FindGameObjectWithTag("NPC");
            if(captain.name == "Captain"){
                captain.GetComponent<Animator>().SetTrigger("PlayIntro");
            }
            triggered = true;
        }
    }
}
