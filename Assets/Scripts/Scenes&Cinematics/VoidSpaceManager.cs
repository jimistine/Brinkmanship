using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using Unity.Mathematics;
using UnityEngine.UI;
using UnityEngine.Playables;
using DG.Tweening;

public class VoidSpaceManager : MonoBehaviour
{
    private GameManager GM;
    private DialogueRunner dialogueRunner;
    private GameObject doorForrester;
    public GameObject blackScreen;
    [Header("Debug Options")]
    public bool loadAssetsAtStart;
    public bool skipIntro;
    public bool enableGun;
    [Header("Intro")]
    public GameObject cinematicsParent;
    public PlayableDirector introDir;
    public Image mapImageActive;
    // public Sprite mapImage1;
    // public Sprite mapImage2;
    // public Sprite mapImage3;
    public GameObject cubicleDoor;
    public GameObject doorCollider;
    [Header("Doors")]
    public GameObject parentsParent;
    public GameObject parentsWalkaway;
    public GameObject tavParent;
    public GameObject forresterParent;
    public GameObject urnstParent;
    public GameObject bridgeParent;
    private BridgeDoorController bdController;
    public GameObject brineParent;
    public GameObject brigElevatorParent;
    private AirlockManager alManager;
    public AK.Wwise.State void0State;
    public AK.Wwise.State void0pState;
    public AK.Wwise.State void1State;
    public AK.Wwise.State void2State;
    [Header("Sun and Timer")]
    public bool countingDown;
    public float maxTime;
    public float timeInVoid;
    private bool halfWay;
    public bool timeUp;
    public GameObject halfMarker;
    public GameObject fullMarker;
    public Animator animSurator;
    public AnimationClip suratorGrowClip;
    [Header("Other")]
    public GameObject shipVoice1;
    public GameObject shipVoice2;
    public GameObject bridgeLight;
    public GameObject musicObject;
    public GameObject cubicleWalls;
    public GameObject ailynLight;
    public Animator ailynLightAnim;
    public bool ailynPinging;
    public GameObject floorGun;
    
    // Static events
    public delegate void OnTimeUp();
    public static event OnTimeUp onTimeUp;

    // Start is called before the first frame update
    void Start()
    {
        GM = FindAnyObjectByType<GameManager>();
        GM.SetVoidManager(this); 
        alManager = GameObject.FindObjectOfType<AirlockManager>();
        doorForrester = GameObject.FindGameObjectWithTag("Forrester's Door");   
        dialogueRunner = FindAnyObjectByType<DialogueRunner>();
        DialogueManager.SetVariable("void_timer_running", "true");
        maxTime = suratorGrowClip.length;
        bdController = bridgeParent.GetComponent<BridgeDoorController>();

        dialogueRunner.AddCommandHandler(
            "enter_bridge",
            EnterBridge
            );

        // Set all doors inactive, should probably be a loop but oh well
        if(!loadAssetsAtStart){
            parentsParent.SetActive(false);
            tavParent.SetActive(false);
            forresterParent.SetActive(false);
            urnstParent.SetActive(false);
            brineParent.SetActive(false);
            brigElevatorParent.SetActive(false);
            bridgeParent.SetActive(false);
            halfMarker.SetActive(false);
            fullMarker.SetActive(false);
            void0State.SetValue();
        }
        else{
           void1State.SetValue();
        }
        if(skipIntro){
            cinematicsParent.SetActive(false);
            introDir.playOnAwake = false;
        }
        else{
            introDir.Play();
        }
        if(enableGun){
            FindObjectOfType<GunManagerComponent>().gunEnabled = true;
        }
        else{
            FindObjectOfType<GunManagerComponent>().gunEnabled = false;
            FindObjectOfType<GunManagerComponent>().bulletUI.SetActive(false);  
        }

        ProgressManager.Instance.SetProgManagerVars();
    }

    void OnEnable(){
        ProgressManager.OnNewSequenceEvent += SequenceUpdater;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.RightShift)){
            if(Input.GetKeyDown(KeyCode.D)){
                parentsParent.SetActive(true);
                tavParent.SetActive(true);
                forresterParent.SetActive(true);
                urnstParent.SetActive(true);
                brineParent.SetActive(true);
                brigElevatorParent.SetActive(true);
                bridgeParent.SetActive(true);
                halfMarker.SetActive(true);
                fullMarker.SetActive(true);
                alManager.AddAirlock();
                void0State.SetValue();
            }
            if(Input.GetKeyDown(KeyCode.P)){
                Toggle_Ailyn_Ping();
            }
        }

    // Void Timer
        if(countingDown){
            timeInVoid += Time.deltaTime;
            if(timeInVoid >= maxTime/2 && !halfWay){
                // Remind the player they're being timed, and are half way to the end
                halfWay = true;
                StartCoroutine(HalfWaySequence());
            }
            else if(timeInVoid >= maxTime && !timeUp){
                // Time's Up!!
                timeUp = true;
                onTimeUp?.Invoke();
                DialogueManager.SetVariable("void_timer_running", "false");
                StartCoroutine(TimeUpSequence());
            }
        }
    }

    public IEnumerator HalfWaySequence(){
        void2State.SetValue();
        GM.musicManager.PostMusicEvent("Fade_In_Lvl_02");
        //AkSoundEngine.PostEvent("Fade_In_Lvl_02", musicObject);
        AkSoundEngine.PostEvent("Play_Intercom_Halfway_Warning", shipVoice1);
        AkSoundEngine.PostEvent("Play_Intercom_Halfway_Warning", shipVoice2);
        if(DialogueManager.Instance.isDialogueRunning){
            StopCoroutine(HalfWaySequence());
        }
        else{
            DialogueManager.Instance.UpdateConvoTargets("Kase");
            DialogueManager.InvokeYSEvent("HalfWay_Void", "Kase");
            yield return new WaitForSeconds(3f);
        }

   }

    public IEnumerator TimeUpSequence(){
        Debug.Log("Time's up!");
        // If you're already talking to someone, we transition out.
        if(dialogueRunner.IsDialogueRunning){
            if(DialogueManager.activeNPC == "BridgeDoor"){
                // let the convo go
            }
            else{
                DialogueManager.InvokeYSEvent("times_up", "Kase");
            }
        }
        else{
            DialogueManager.Instance.UpdateConvoTargets("Ailyn");
            DialogueManager.InvokeYSEvent("TimesUp", "Ailyn");
        }
    // Music cue
        GM.musicManager.PostMusicEvent("Play_Lvl_3_Loop");
    // If bridge doors aren't there, pop em in
        if(!bridgeParent.activeSelf){
            AkSoundEngine.PostEvent("Door_Spawn", bridgeParent);
            AkSoundEngine.PostEvent("Door_Spawn", bridgeParent);
            bridgeParent.SetActive(true);
        }
    // Bridge doors flash and play alarm to bring players over
        if(!bdController.timeUpAlarmTriggered){
            AkSoundEngine.PostEvent("BridgeDoorAlarm_TimeUp", bridgeParent);
            // Turn on flashy lights
                bridgeLight.SetActive(true);
                Tween lightTweenTU = bridgeLight.transform.DORotate(new Vector3(0, 360, 0), 0.5f, RotateMode.FastBeyond360)
                                    .SetLoops(-1, LoopType.Restart)
                                    .SetRelative()
                                    .SetEase(Ease.Linear);
        }

        yield return new WaitForSeconds(3f);

    // Other doors are removed
        AkSoundEngine.PostEvent("Door_Despawn", parentsParent);
        yield return new WaitForSeconds(1.2f);
        parentsParent.SetActive(false);
        AkSoundEngine.PostEvent("Door_Despawn", tavParent);
        yield return new WaitForSeconds(1.2f);
        tavParent.SetActive(false);
        AkSoundEngine.PostEvent("Door_Despawn", forresterParent);
        yield return new WaitForSeconds(1.2f);
        forresterParent.SetActive(false);
        AkSoundEngine.PostEvent("Door_Despawn", urnstParent);
        yield return new WaitForSeconds(1.2f);
        urnstParent.SetActive(false);
        AkSoundEngine.PostEvent("Door_Despawn", brineParent);
        yield return new WaitForSeconds(1.2f);
        brineParent.SetActive(false);
        AkSoundEngine.PostEvent("Door_Despawn", brigElevatorParent);
        yield return new WaitForSeconds(1.2f);
        brigElevatorParent.SetActive(false);
        
        // Stop dialogue to make sure they're out of it and can talk to Ailyn.
        if(dialogueRunner.IsDialogueRunning && DialogueManager.activeNPC != "BridgeDoor"){
            DialogueManager.Instance.dialogueRunner.Stop();
        }

    // BRtodo: Lighting change?


    // Set bridge timer lowest amount
        GameManager.GM.timeOnBridge = 240f;

   }
    

    public void SequenceUpdater(Sequence oldSequence, Sequence newSequnce){
        Debug.Log("Starting sequence: " + newSequnce);
        if(newSequnce == Sequence.TheDiscovery){
            //StartCoroutine(LockdownSequence());
        }
        else if(newSequnce == Sequence.FirstSteps){
            StartCoroutine(FirstStepsSequence());
        }
        else if(newSequnce == Sequence.FollowingUp){
            StartCoroutine(FollowingUpSequence());
        }
        else if(newSequnce == Sequence.TheInvestigationBegins){
            StartCoroutine(InvestigationBeginsSequence());
        }
        else if(newSequnce == Sequence.Prisoner){
            StartCoroutine(PrisonerSequence());
        }
    }
    
// Coroutines for each new sequence
    public IEnumerator FirstStepsSequence(){
        // We're through the intro and you now need to talk to your folks
        parentsParent.SetActive(true);
        AkSoundEngine.PostEvent("Door_Spawn", parentsParent);
        void0pState.SetValue();
        yield return null;
    }
    public IEnumerator FollowingUpSequence(){
        // You've talked to your parents and now must check in with Ailyn
        parentsWalkaway.SetActive(true);
        //AkSoundEngine.PostEvent("Play_Lvl_1_Loop_Filtered", musicObject);
        GM.musicManager.PostMusicEvent("Play_Lvl_1_Loop_Filtered");
        
        Toggle_Ailyn_Ping();
        yield return null;
    }

    public IEnumerator InvestigationBeginsSequence(){
        // You've followed up with Ailyn and now have access to the rest of the ship
        void1State.SetValue();
        //AkSoundEngine.PostEvent("Play_Lvl_1_Loop", musicObject);
        GM.musicManager.PostMusicEvent("Play_Lvl_1_Loop");

        tavParent.SetActive(true);
        AkSoundEngine.PostEvent("Door_Spawn", tavParent);
        yield return new WaitForSecondsRealtime(1);

        forresterParent.SetActive(true);
        AkSoundEngine.PostEvent("Door_Spawn", forresterParent);
        yield return new WaitForSecondsRealtime(1);
        
        urnstParent.SetActive(true);
        AkSoundEngine.PostEvent("Door_Spawn", urnstParent);
        yield return new WaitForSecondsRealtime(1);
        
        AkSoundEngine.PostEvent("Door_Spawn", brineParent);
        brineParent.SetActive(true);
        yield return new WaitForSecondsRealtime(3);
        //Add Bridge
        bridgeParent.SetActive(true);
        // this should be a unique effect, probably a music que but for now we just double it lol
        AirlockManager alManager = GameObject.FindObjectOfType<AirlockManager>();
        alManager.AddAirlock();
        AkSoundEngine.PostEvent("Door_Spawn", bridgeParent);
        AkSoundEngine.PostEvent("Door_Spawn", bridgeParent);
        yield return null;
    }
    
    public IEnumerator PrisonerSequence(){
        // You talked to JB and now have access to the brig
        AkSoundEngine.PostEvent("Door_Spawn", brigElevatorParent);
        brigElevatorParent.SetActive(true);
        yield return null;
    }

    // Yarn Commands
    // [YarnCommand("Enable_Map1")]
    // public void Enable_Map1(){
    //     mapImageActive.sprite = mapImage1;
    // }
    // [YarnCommand("Enable_Map2")]
    // public void Enable_Map2(){
    //     mapImageActive.sprite = mapImage2;
    // }
    // [YarnCommand("Enable_Map3")]
    // public void Enable_Map3(){
    //     mapImageActive.sprite = mapImage3;
    // }

    [YarnCommand("Lockdown_que")]
    public void Lockdown_que(){
        StartCoroutine(LockdownSequence());
    }
    public IEnumerator LockdownSequence(){
        //Close the door
        //doorCollider.SetActive(true);
        cubicleDoor.GetComponent<Animator>().SetBool("CloseDoor", true);
        AkSoundEngine.PostEvent("CubicleDoor_Close", cubicleDoor);
        yield return new WaitForSeconds(0.5f);

        // SFX of other doors closing
        AkSoundEngine.PostEvent("Lockdown_Doors_Fade", gameObject);
        for (int i = 20; i > 0; i--){
            AkSoundEngine.PostEvent("Lockdown_Doors", gameObject);
            float waitTime = UnityEngine.Random.Range(0.25f, 0.5f);
            yield return new WaitForSecondsRealtime(waitTime);
        }
    }
    [YarnCommand("ActivateRings")]
    public void ActivateRings(){
        StartCoroutine(MarkerIntro());
    }
    IEnumerator MarkerIntro(){
        halfMarker.SetActive(true);
        AkSoundEngine.PostEvent("MarkerIntro", parentsParent);
        yield return new WaitForSeconds(1.5f);
        fullMarker.SetActive(true);
        AkSoundEngine.PostEvent("MarkerIntro", parentsParent);
     
        // Start animations and timer
        animSurator.SetBool("Start_Grow_Surator", true);
        halfMarker.GetComponent<Animator>().SetBool("Max_Grow", true);
        fullMarker.GetComponent<Animator>().SetBool("Max_Grow", true);
        countingDown = true;
    }
    [YarnCommand("UnlockDoor")]
    public void UnlockDoor(){
        cubicleDoor.GetComponent<CubicleDoor>().UnlockDoor();
    }
    [YarnCommand("Play_Intercom_Lockdown")]
    public void Play_Intercom_Lockdown(){
        AkSoundEngine.PostEvent("Play_Intercom_GAM_Lockdown", shipVoice1);
        AkSoundEngine.PostEvent("Play_Intercom_GAM_Lockdown", shipVoice2);
    }
    [YarnCommand("Toggle_Ailyn_Ping")]
    public void Toggle_Ailyn_Ping(){
        if(!ailynPinging){
            ailynPinging = true;
            //AkSoundEngine.PostEvent("Play_ailyn_ping", ailynLight);
            ailynLightAnim.SetBool("Pinging", true);
        }
        else{
            ailynPinging = false;
            //AkSoundEngine.PostEvent("Stop_ailyn_ping", ailynLight);
            ailynLightAnim.SetBool("Pinging", false);
        }
    }
    [YarnCommand("Show_Gun")]
    public void ShowGun(){
        floorGun.SetActive(true);
    }

    
    
    // Yarn Spinner calls the function and it comes here to play the door open sound
    public void EnterBridge(){
        bdController.OpenDoors();
        // Debug.Log("Start enter the bridge routine");
        // uint callbackType = (uint)AkCallbackType.AK_EndOfEvent;
        // AkSoundEngine.PostEvent("Door_A_Open", bridgeParent, callbackType, EnterBridgeCallback, null);
    }
    // When that sound is done, we run the coroutine
    public void EnterBridgeCallback(object in_cookie, AkCallbackType in_type, object in_info){
        StartCoroutine(EnterBridgeRoutine());
    }
    public IEnumerator EnterBridgeRoutine(){
        blackScreen.SetActive(true);
        AkSoundEngine.PostEvent("stop_void_amb_01", GameObject.Find("Ambient"));
        AkSoundEngine.PostEvent("Door_A_Close", bridgeParent);
        //AkSoundEngine.PostEvent("Fade_Music", musicObject);
        
        yield return new WaitForSeconds(4);
        GM.UpdateScenes("02_Void", "03_Bridge");
    }


}
