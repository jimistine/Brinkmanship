using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.VFX;

public class BridgeDoorController : MonoBehaviour
{
    public VisualEffect smoke1;
    public VisualEffect smoke2;
    public GameObject bridgeLight;
    public Animator bridgeDoorAnimator;
    public VoidSpaceManager voidManager;
    public bool timeUpAlarmTriggered;
    // Start is called before the first frame update
    void Start()
    {
        voidManager = FindObjectOfType<VoidSpaceManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.RightShift)){
            if(Input.GetKeyDown(KeyCode.O)){
                OpenDoors();
            }
        }
    }
    void OnEnable(){
        DoorEnterHandler.onLoadBridge += KillFX;
    }
    void OnDisable(){
        DoorEnterHandler.onLoadBridge -= KillFX;
    }

    public void OpenDoors(){
        Debug.Log("Opening Bridge Doors");
        StartCoroutine(OpenBridgeDoors());
    }
    public IEnumerator OpenBridgeDoors(){
        GetComponent<NPCManagerComponent>().isInteractable = false;
        if(!voidManager.timeUp){
            timeUpAlarmTriggered = true;
            AkSoundEngine.PostEvent("BridgeDoorAlarm_TimeUp", gameObject);
            // Turn on flashy lights
            bridgeLight.SetActive(true);
            Tween lightTween = bridgeLight.transform.DORotate(new Vector3(360, 0, 0), 0.5f, RotateMode.FastBeyond360)
                                .SetLoops(-1, LoopType.Restart)
                                .SetRelative()
                                .SetEase(Ease.Linear);
            yield return new WaitForSeconds(3f);
            AkSoundEngine.PostEvent("BridgeDoorAlarm_Opening", gameObject);
        }
        //Start Animation, VFX
        AkSoundEngine.PostEvent("Play_Bridge_Doors_Warmup", gameObject);
        yield return new WaitForSeconds(3f);

        bridgeDoorAnimator.SetBool("Open_BridgeDoors", true);
        
        AkSoundEngine.PostEvent("Play_BridgeDoorsComp_01", gameObject);

        GameManager.GM.musicManager.PostMusicEvent("Play_Lvl_3_Loop");
        GameManager.GM.musicManager.PostMusicEvent("Play_Airlock_In");
        GameManager.GM.musicManager.PostMusicEvent("Mute_Track_01");
        GameManager.GM.musicManager.PostMusicEvent("Mute_Track_02");

        smoke1.Play();
        smoke2.Play();

        // Wait till all that's done then turn off the alarm and smoke
        yield return new WaitForSeconds(10f);
        AkSoundEngine.PostEvent("BridgeDoorAlarm_Stop", gameObject);
        smoke1.Stop();
        smoke2.Stop();
    }

    void KillFX(){
        AkSoundEngine.PostEvent("BridgeDoorAlarm_Stop", gameObject);
        smoke1.Stop();
        smoke2.Stop();
    }
}
