using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;
using UnityEngine.Events;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{

    public static GameManager GM { get; set;  }

    public MusicManager musicManager;

    [Header("Scenes to Load")]
    public bool MainMenu;
    public bool Void;
    public bool Bridge;
    public bool None;

    public delegate void OnRestartGame();
    public static event OnRestartGame OnRestartGameEvent;

    [Header("Void Parameters")]
    public VoidSpaceManager voidManager;
    [Header("Bridge State Parameters")]
    public BridgeManager bridgeManager;
    public float timeOnBridge;
    public bool gameEnded;
    [Header("DebugTools")]
    public bool showPointer;
    public GameObject bridgeSkipMarker;
    
    void Awake(){

        GM = this;

        if(MainMenu){
            SceneManager.LoadScene("01_Start",LoadSceneMode.Additive);
        }
        else if(Void){
            SceneManager.LoadScene("02_Void",LoadSceneMode.Additive);
        }
        else if(Bridge){
            SceneManager.LoadScene("03_Bridge",LoadSceneMode.Additive);
        }
        else if(None) {
            return;
        }
        else {
            SceneManager.LoadScene("01_Start", LoadSceneMode.Additive);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //musicManager = GameObject.FindObjectOfType<MusicManager>();
    }

    // Update is called once per frame
    void Update()
    {
    
    // Debug Controls
        // Manual Restarter
        if(Input.GetKey(KeyCode.RightShift)){
            if(Input.GetKeyDown(KeyCode.R)){
                RestartGame();
            }
        }
        // Manual Bridge Restarter
        if(Input.GetKey(KeyCode.RightShift)){
            if(Input.GetKeyDown(KeyCode.B)){
                RestartBridge();
            }
        }
        if(Input.GetKey(KeyCode.RightShift)){
            if(Input.GetKeyDown(KeyCode.M)){
                musicManager.PostMusicEvent("Play_Lvl_3_Loop_Bridge");
            }
        }
        // Manual Bridge Skip
        // if(Input.GetKey(KeyCode.LeftShift)){
        //     if(Input.GetKeyDown(KeyCode.Alpha3)){
        //         // UpdateScenes("02_Void", "03_Bridge");
        //         GameObject player = GameObject.FindGameObjectWithTag("Player");
        //         Debug.Log("Player: " + player.name);
        //         player.transform.position = new Vector3(44.0f, -8.7f, 25.1f);
        //         //StartCoroutine(GameObject.FindGameObjectWithTag("DoorEnterHandler").GetComponent<DoorEnterHandler>().BeginAirlockSequence());
        //     }
        // }
        // Learn Everything Button
        if(Input.GetKey(KeyCode.RightShift)){
            if(Input.GetKeyDown(KeyCode.I)){
                ProgressManager.Instance.LearnAllIntel();
                AkSoundEngine.PostEvent("Play_Ui_2_Hover_b", gameObject);
            }
        }
        // Load Coda
        // if(Input.GetKey(KeyCode.LeftShift)){
        //     if(Input.GetKeyDown(KeyCode.C)){
        //         StartCoda();
        //     }
        // }

        if(showPointer){
            Cursor.visible = true;
        }

        if(GetCurrentScene() == "03_Bridge"){
            if(BridgeTimer.currentTime <= 0 && !gameEnded){
                EndGame();
            }
        }
    }

    public void SetVoidManager(VoidSpaceManager newVoidManager){
        voidManager = newVoidManager;
    }
    public void SetBridgeManager(BridgeManager newBridgeManager){
        bridgeManager = newBridgeManager;
    }
    public void StartGame(){
        UpdateScenes("01_Start", "02b_Airlock");
        SceneManager.LoadScene("02_Void", LoadSceneMode.Additive);
    }
    public void RestartGame(){
        OnRestartGameEvent?.Invoke();
        SceneManager.LoadScene("00_Managers");
    }
    
    public void QuitGame(){
        Application.Quit();
    }
    
    public void UpdateScenes(string sceneToUnload, string sceneToLoad){
        if(sceneToUnload != ""){
            SceneManager.UnloadSceneAsync(sceneToUnload);
        }
        if(sceneToLoad != ""){
            SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
        }
    }
    public void SetTimeOnBridge(float time){
        timeOnBridge = time;
    }
    public static string GetCurrentScene(){
        int countLoaded = SceneManager.sceneCount;
        Scene[] loadedScenes = new Scene[countLoaded];
        string currentScene = "";
 
        for (int i = 0; i < countLoaded; i++)
        {
            loadedScenes[i] = SceneManager.GetSceneAt(i);
        }
        foreach(Scene scene in loadedScenes){
            if(scene.name == "02_Void"){
                currentScene = "02_Void";
            }
            else if(scene.name == "03_Bridge"){
                currentScene = "03_Bridge";
            }
            // else{
            //     currentScene = ""
            // }
        }
        return currentScene;
    }
    // Restarts the game using the debug on the bridge
    public void RestartBridge(){
        Debug.Log("Restarting Bridge");

        AkSoundEngine.PostEvent("Stop_All_Audio", gameObject);
        DialogueManager.Instance.dialogueRunner.Stop();
        ProgressManager.Instance.brNodesVisited.Clear();
        ProgressManager.Instance.storedVariables.Remove("turnComplete");
        ProgressManager.Instance.storedVariables.Remove("turn_status");
        gameEnded = false;
        timeOnBridge = 270f;
        BridgeTimer.currentTime = timeOnBridge;
        UpdateScenes("03_Bridge", "03_Bridge");
        musicManager.PostMusicEvent("Play_Lvl_3_Loop_Bridge");
        bridgeManager.BeginEntry();
        StartCoroutine(InitPlayerDialogueInput());
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerManagerComponent>().bulletCount = 6;
        bridgeManager.ResetPPVals();
    }
    // Restarts the Bridge from the end of the game
    public void RestartBridge_End(){
        Debug.Log("Restarting Bridge");
        AkSoundEngine.PostEvent("Stop_All_Audio", gameObject);
        ProgressManager.Instance.brNodesVisited.Clear();
        ProgressManager.Instance.storedVariables.Remove("turnComplete");
        ProgressManager.Instance.storedVariables.Remove("turn_status");
        gameEnded = false;
        timeOnBridge = 270f;
        BridgeTimer.currentTime = timeOnBridge;
        UpdateScenes("", "02b_Airlock");
        UpdateScenes("04_Coda", "03_Bridge");
        //DialogueManager.InvokeYSEvent("Bridge_Start", null);
        musicManager.PostMusicEvent("Play_Lvl_3_Loop_Bridge");
        bridgeManager.BeginEntry();
        StartCoroutine(InitPlayerDialogueInput());
        bridgeManager.ResetPPVals();
    }

    public IEnumerator InitPlayerDialogueInput(){
        yield return new WaitForSeconds(0.25f);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject parentDia = GameObject.FindGameObjectWithTag("DialogueSystem");
        if (parentDia == null) { Debug.LogWarning("Cannot find the Dialogue!!!!!!!!!!!"); }
        else
        {
            DialogueInputManager dialogueInputManager = player.GetComponent<DialogueInputManager>();
            dialogueInputManager.optionsListView = parentDia.GetComponentInChildren<Br_OptionsListView>();
            dialogueInputManager.DR = parentDia.GetComponent<DialogueRunner>();
            dialogueInputManager.lineView = parentDia.GetComponentInChildren<Br_2DLineView>();
            dialogueInputManager.voidLineView = null;
        }
        DialogueManager.InvokeYSEvent("Bridge_Start", null);
    }
    public void EnableGun(){
        GunManagerComponent gunManager = FindAnyObjectByType<GunManagerComponent>();
        gunManager.gunEnabled = true;
        gunManager.bulletUI.SetActive(true);
        if(GetCurrentScene() == "02_Void"){
            voidManager.floorGun.SetActive(false);
            AkSoundEngine.PostEvent("GunDraw", gameObject);
        }
    }
    public void DisableGun(){
        GunManagerComponent gunManager = FindAnyObjectByType<GunManagerComponent>();
        gunManager.gunEnabled = false;
    }
    public void EndGame(){
        // did they make the turn?
        if(!gameEnded){
            Debug.Log("Ending Game");
            gameEnded = true;
            ProgressManager.Instance.storedVariables.TryGetValue("turnComplete", out string turnStatus);
            if(turnStatus == "true"){
                DialogueManager.SetVariable("gameEnding", "good");
            }
            else if(bridgeManager.turning){
                DialogueManager.SetVariable("gameEnding", "ugly");
            }
            else{
                DialogueManager.SetVariable("gameEnding", "bad");
            }

            ProgressManager.Instance.storedVariables.TryGetValue("gameEnding", out string gameEnding);
            if(gameEnding == "good"){
                bridgeManager.blackScreen.SetActive(true);
                DialogueManager.InvokeYSEvent("END_1", "Game");
            }
            else if(gameEnding == "bad"){
                bridgeManager.blackScreen.SetActive(true);
                DialogueManager.InvokeYSEvent("END_2", "Game");
            }
            else if(gameEnding == "ugly"){
                //blackScreen.SetActive(true);
                bridgeManager.StartUglySequence();
                DialogueManager.InvokeYSEvent("END_3", "Game");
            }
        }
    }
    public void StartCoda(){
        AkSoundEngine.PostEvent("Stop_All_Audio", gameObject);
        SceneManager.UnloadSceneAsync("02b_Airlock");
        Destroy(GameObject.FindGameObjectWithTag("PlayerParent"));
        UpdateScenes("03_Bridge", "04_Coda");
    }
    /*
        Pre-Flight Checklist
            - Submit outstanding changes
            - Set Game Manager load MM
            - Void Manager
                - Disable gun
                - Uncheck load assets at start
                - Uncheck skip into
            - Airlock Manager
                - Uncheck load at start
            - Unload all scenes but Manager Scene
            - Disable frame counter
            - Uncheck evidence learned on Progress manager

            - Check out Wwise Project
            - Generate Soundbanks

            - Run it for a little in editor
            MAC
                - Edit Project Settings -> Enable Copy soundbanks at pre-build step
                - Check out Streaming Assets folder

            PC
                - Verson control -> work offline (remember to reset after!!)
    
            - Build!
    */
}
