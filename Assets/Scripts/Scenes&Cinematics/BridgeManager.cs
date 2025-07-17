using System.Collections;
using System.Collections.Generic;
using Yarn.Unity;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using DG.Tweening;
using Cinemachine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering.LookDev;
using UnityEditor;
using Unity.VisualScripting;



public class BridgeManager : MonoBehaviour
{

    private DialogueRunner dialogueRunner;
    [Header("Entry Sequence")]
    public Volume bridgeSkyVol;
    public Volume bridgePPVol;
    private VolumeProfile ppProfile;
    private VolumeProfile skyProfile;
    public Light trado;
    private HDAdditionalLightData tradoLD;
    public float fadeUpTime;
    private PhysicallyBasedSky pbrSky;
    private bool spaceEmissionOverride;
    private float spaceEmissionVal;
    private Tween spaceTween;
    private Bloom ppBloom;
    private Exposure ppExposure;
    [Header("Turning Sequence")]
    public GameObject celestialBodies;
    private Transform cBTransform;
    public GameObject surator;
    public float timeToTurn;
    public bool turning;
    public CinemachineVirtualCamera playerCam;
    private Tween turnTween;
    private Tween freqTween;
    private Tween ampTween;
    private Tween freqSunTween;
    private Tween ampSunTween;
    public bool uglyEndingStarted;
    [Header("Interactables")]
    public GameObject intercomObj;
    public HDAdditionalLightData intercomLight;
    public Material onMat;
    public Material offMat;
    public OverrideController overrideController;
    public GameObject captainOBJ;
    public GameObject captainRagdollOBJ;
    public GameObject pilotObj;
    public GameObject pilotRagdollObj;
    [Header("Timer")]
    public BridgeTimer bridgeTimer;
    [Header("Endings")]
    public GameObject blackScreen;
    public GameObject whiteScreen;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.GM.SetBridgeManager(this);
        GameManager.GM.EnableGun();
        cBTransform = celestialBodies.GetComponent<Transform>();
        dialogueRunner = FindAnyObjectByType<DialogueRunner>();
        ProgressManager.Instance.SetProgManagerVars();
        playerCam = GameObject.FindGameObjectWithTag("PlayerParent").GetComponentInChildren<CinemachineVirtualCamera>();

        tradoLD = trado.GetComponent<HDAdditionalLightData>();
        skyProfile = bridgeSkyVol.sharedProfile;
        ppProfile = bridgePPVol.sharedProfile;

        ppProfile.TryGet<Bloom>(out ppBloom);
        ppProfile.TryGet<Exposure>(out ppExposure);
        skyProfile.TryGet<PhysicallyBasedSky>(out pbrSky);

        ResetPPVals();

        Cursor.lockState = CursorLockMode.Locked;
        
        //StartUglySequence();
        //BeginEntry();
    
    // Command Handlers
        dialogueRunner.AddCommandHandler<string>(
            "updateIntercom",
            SetIntercomRoutine
            );
    }

    public void ResetPPVals(){
        ppBloom.intensity.value = 0.06f;
        ppExposure.compensation.value = 0f;
        pbrSky.spaceEmissionMultiplier.value = 0f;
    }


    // Update is called once per frame
    void Update()
    {
        // Testing only
        // if(Input.GetKeyDown(KeyCode.I)){
        //     if(intercomLight.color == Color.green){
        //         SetIntercomRoutine("off");
        //     }
        //     else{
        //         SetIntercomRoutine("on");
        //     }
        // }
        if(Input.GetKey(KeyCode.RightShift)){
            if(Input.GetKeyDown(KeyCode.T)){
                BeginTurningSequence();
            }
            if(Input.GetKeyDown(KeyCode.Alpha4)){
                GameManager.GM.StartCoda();
            }
            if(Input.GetKeyDown(KeyCode.Alpha0)){
                ResetPPVals();
            }
        }
    }

    public void BeginEntry(){
        Debug.Log("Beginning Entry");
        spaceTween = DOTween.To(() => pbrSky.spaceEmissionMultiplier.value, x => pbrSky.spaceEmissionMultiplier.value = x, 10, fadeUpTime);   
        DOTween.To(() => tradoLD.intensity, x => tradoLD.intensity = x, 0.2f, fadeUpTime);
    }

    public void SetIntercomRoutine(string status){
        // trigger animation
        //play SFX
        if(status == "off"){
            intercomLight.color = Color.red;
            intercomObj.GetComponent<MeshRenderer>().material = offMat;
            AkSoundEngine.PostEvent("PanelDisappear", bridgeTimer.shipSpeaker);
        }
        if(status == "on"){
            intercomLight.color = Color.green;
            intercomObj.GetComponent<MeshRenderer>().material = onMat;
            AkSoundEngine.PostEvent("PanelAppear", bridgeTimer.shipSpeaker);
        }
    }
    public void SwapCaptain(){
        captainOBJ.SetActive(false);
        captainRagdollOBJ.SetActive(true);
        captainRagdollOBJ.GetComponentInChildren<Rigidbody>().AddForce(captainRagdollOBJ.transform.forward * -50, ForceMode.Impulse);
    }
    public void SwapPilot(){
        pilotObj.SetActive(false);
        pilotRagdollObj.SetActive(true);
        pilotRagdollObj.GetComponentInChildren<Rigidbody>().AddForce(captainRagdollOBJ.transform.forward * 25, ForceMode.Impulse);
    }

    [YarnCommand("begin_turn")]
    public void BeginTurn()
    {
        BeginTurningSequence();
    }
    [YarnFunction("get_time_left")]
    public static int GetTimeLeft(){
        return Mathf.RoundToInt(BridgeTimer.currentTime);
    }

    public void BeginTurningSequence(){
        StartCoroutine(TurningSequence());
    }
    public IEnumerator TurningSequence(){
        // probably want to update the timer screen to communicate status
        // if pilot's dead, it'll take a lot longer
        if(DialogueManager.GetVariable("pilot") == "dead" || overrideController.overrideEngaged){
            timeToTurn *= 2;
        }

        Debug.Log("Turning");
        DialogueManager.SetVariable("turn_status", "turning");
        if(GetTimeLeft() >= 20f){
            AkSoundEngine.PostEvent("Ship_turning", bridgeTimer.shipSpeaker);
        }
        AkSoundEngine.PostEvent("Play_turning_announcement", bridgeTimer.shipSpeaker);
        yield return new WaitForSeconds(2.0f);
        ActiveEngineManager.StartEngines();
        turning = true;
        ampTween = DOTween.To(()=> playerCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain, x => playerCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = x, 1, 3);
        freqTween = DOTween.To(()=> playerCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain, x => playerCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = x, 5, 3);
        turnTween = celestialBodies.transform.DORotate(new Vector3(0.0f, 148.0f, 0.0f), timeToTurn, RotateMode.Fast);
        
        if(!uglyEndingStarted){
            // Turn is over
            yield return new WaitForSeconds(timeToTurn - 2);
            ampTween = DOTween.To(()=> playerCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain, x => playerCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = x, 0.5f, 3);
            freqTween = DOTween.To(()=> playerCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain, x => playerCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = x, 0.3f, 3);
            ProgressManager.Instance.storedVariables.TryAdd("turnComplete", "true");
            DialogueManager.SetVariable("turn_status", "complete");
            AkSoundEngine.PostEvent("Ship_turning_stop", bridgeTimer.shipSpeaker);
            ActiveEngineManager.EndEngines();
            turning = false;
            yield return new WaitForSeconds(4f);
            GameManager.GM.EndGame();
        }
    }
    public void StartGoodSequence(){
        blackScreen.SetActive(true);
    }
    public void StartBadSequence(){
        blackScreen.SetActive(true);
    }
    public void StopTurn(){
        //uh-oh!!
        // stops the current turning tweens and starts rotating towards center
        turnTween.Kill();
        ampTween.Kill();
        freqTween.Kill();
        StopCoroutine(TurningSequence());
        turnTween = celestialBodies.transform.DORotate(new Vector3(0.0f, 119.0f, 0.0f), timeToTurn/2, RotateMode.Fast);
    }
    public void StartUglySequence(){
        uglyEndingStarted = true;
        StopTurn();
        StartCoroutine(UglySequence()); 
    }

    public IEnumerator UglySequence(){
        AkSoundEngine.PostEvent("Play_ship_voice_ugly_ending", bridgeTimer.shipSpeaker);
        
        yield return new WaitForSeconds(3);
        surator.GetComponent<Animator>().SetBool("Ending_Ugly", true);
        ampSunTween = DOTween.To(()=> playerCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain, x => playerCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = x, 3, 15);
        freqSunTween = DOTween.To(()=> playerCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain, x => playerCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = x, 10, 15);
        
        captainOBJ.GetComponent<Animator>().SetBool("Shield_Eyes", true);
        yield return new WaitForSeconds(3);
        DOTween.To(() => ppExposure.compensation.value, x => ppExposure.compensation.value = x, 100, 25);
        DOTween.To(() => ppBloom.intensity.value, x => ppBloom.intensity.value = x, 1, 25).OnComplete(ResetPPVals);
        yield return new WaitForSeconds(17f);
        GameManager.GM.StartCoda();

    }


    /*

        VOID > doors open > player enters > doors close > ULNOAD VOID, 
        LOAD BRIDGE > airlock convo > bridge doors open + fade up lights > player enters bridge > doors close > start bridge convo

    */    
    // [InitializeOnLoad]
    // public class PlayStateNotifier
    // {
        
    //      PlayStateNotifier()
    //     {
    //         EditorApplication.playModeStateChanged += ModeChanged;
    //     }
    
    //      void ModeChanged(PlayModeStateChange playModeState)
    //     {
    //         if (playModeState == PlayModeStateChange.EnteredEditMode) 
    //         {
    //             Debug.Log("Entered Edit mode.");
    //             ResetVals();
    //         }
    //     }
    // }
    //  void ResetVals(){
    //     ppExposure.compensation.value = 0f;
    //             ppBloom.intensity.value = 0.14f;
    // }
}
