using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yarn.Unity;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set;  }
    
    public DialogueRunner dialogueRunner;
    public string stoppedNode = "";
    public static string activeNPC;
    public bool isDialogueRunning;
    private Br_OptionsListView optionListView;
    private Br_3DLineView lineView;
    private List<string> namesOfNpcThatDied = new List<string>();

    public static GunManagerComponent gunManager;
    
    // public IDictionary<string, string> storedVariables = new Dictionary<string, string>();
    
    private void Awake()
    {
        // if (Instance != null && Instance != this)
        // {
        //     Debug.LogWarning("More than one instance of NPCSystem found in scene. This is not allowed. Destroying this (" + gameObject.name + ") instance. Please check your scene.");
        //     Destroy(this);
        //     return;
        // }
        // else
        // {
        //     Instance = this; 
        // }
            Instance = this; 
    }
    void OnEnable()
    {
        NPCManagerComponent.OnNPCDeathInSceneEvent += NPCDeathInScene;   
    }
    void OnDisable()
    {
        NPCManagerComponent.OnNPCDeathInSceneEvent -= NPCDeathInScene;
    }
    // Start is called before the first frame update
    void Start()
    {
        dialogueRunner = GetComponent<DialogueRunner>();
        gunManager = FindObjectOfType<GunManagerComponent>();


        foreach(DialogueViewBase view in dialogueRunner.dialogueViews){
            if(view.GetType() == typeof(Br_OptionsListView)){
                optionListView = (Br_OptionsListView)view;
            }
        }
        if(GameManager.GetCurrentScene() == "02_Void"){
            lineView = GameObject.FindWithTag("LineView").GetComponent<Br_3DLineView>();
        }
    }
    void Update(){
        //Debug.Log("active npc: " + activeNPC);
        isDialogueRunning = dialogueRunner.IsDialogueRunning;
        if(!isDialogueRunning){
            activeNPC = "";
        }


        // Get out of dialogue free input
        if(isDialogueRunning && Input.GetKeyDown(KeyCode.L)){
            dialogueRunner.Stop();
        }
        else if(!isDialogueRunning && GameManager.GetCurrentScene() == "02_Void"){
            stoppedNode = "";
        }
        //Debug.Log("Stopped node: " + stoppedNode);
        
    }
    private void NPCDeathInScene(GameObject npc)
    {
        InvokeYSEvent("event-npc-death", npc.name);
    }
    
    public static void InvokeYSEvent(string eventName, string npcName){
        // don't interrupt stuff when the game is ending/over (unless it's the game itself)
        if(GameManager.GM.gameEnded && npcName != "Game"){
            return;
        }

        Debug.Log("Invoking YS event: " + eventName + ". From: " + npcName);
        SetVariable("eventOwner", npcName);
        string currentNode = Instance.dialogueRunner.CurrentNodeName;

        // is there a version of the node we're on with the event name on the end?
        if(Instance.dialogueRunner.NodeExists(currentNode + "_" + eventName)){
            // start interrupting and kill the other node checks
            Instance.InterruptAndStartDialogue(currentNode + "_" + eventName);
            //Debug.Log("Starting " + eventName);
            return;
        }
        // is there a version of the event with the Character name on the end?
        else if(Instance.dialogueRunner.NodeExists(eventName + "_" + npcName)){
            // start interrupting and kill the other node checks
            Instance.InterruptAndStartDialogue(eventName + "_" + npcName);
            //Debug.Log("Starting " + eventName);
            return;
        }
        else if (Instance.dialogueRunner.NodeExists(eventName)){   // if we call an exact node name, just run that
            if (Instance.dialogueRunner.CurrentNodeName == null) { // check if we're saying anything at the moment
                // if not, do nothing
            }
            // if the node we're on isn't an event node, mark it as the node we'll resume later
            else if (!Instance.dialogueRunner.CurrentNodeName.Contains("event")){
                Debug.Log("Stopped node: " + Instance.stoppedNode);   
            }
            // if there's no node specific event node
            
            // start interrupting and kill the other node checks
            Instance.InterruptAndStartDialogue(eventName);
            Debug.Log("Starting " + eventName);
            return;
            

        }
    // VOID
        // If we didn't call an exact name, and we're in the void
        else if(GameManager.GetCurrentScene() == "02_Void"){
            // Raised
            if(eventName == "raised"){
                if(Instance.dialogueRunner.IsDialogueRunning){
                    if(Instance.dialogueRunner.NodeExists("gun_draw_event_" + activeNPC)){
                        //Instance.stoppedNode = Instance.dialogueRunner.CurrentNodeName;   
                        Instance.InterruptAndStartDialogue("gun_draw_event_" + activeNPC);
                    }
                    else{
                        Debug.LogWarning("An event has been called but YS node " + "gun_draw_event_" + activeNPC + " does not exist.");
                    }
                }
                else{
                    // Not in conversation. Maybe we have Kase say something to himself here.
                    return;
                }
            }
            // Firing handled on the NPCManagerComponent -> line 95 here
        }
        
    // BRIDGE
        else if(GameManager.GetCurrentScene() == "03_Bridge"){ // we didn't call an exact node name and we're on the bridge
            // Raised
            if(eventName == "raised"){
                if(GetVariable("tension_lvl") != "3"){
                    if(Instance.dialogueRunner.NodeExists("gun_draw_bridge_event")){
                        Instance.InterruptAndStartDialogue("gun_draw_bridge_event");
                    }
                    else{
                        Debug.LogWarning("An event has been called but YS node " + "gun_draw_event_" + activeNPC + " does not exist.");
                    }
                }
            }
            if(eventName == "holstered"){
                if(GetVariable("captain") == "living" && GetVariable("tension_lvl") != "3"){
                    if( GetVariable("tension_lvl") != "2.5"){ // 2.5 is when you fire a round but no one dies
                        if(Instance.dialogueRunner.NodeExists("gun_holster_bridge_event")){
                            Instance.InterruptAndStartDialogue("gun_holster_bridge_event");
                        }
                        else{
                            Debug.LogWarning("An event has been called but YS node " + "gun_holster_bridge_event" + activeNPC + " does not exist.");
                        }
                    }
                }
            }
            else if(eventName == "missed"){
                if(Instance.dialogueRunner.NodeExists("gun_shoot_bridge_event")){
                    Instance.InterruptAndStartDialogue("gun_shoot_bridge_event");
                }
                else{
                    Debug.LogWarning("An event has been called but YS node " + "gun_shoot_bridge_event" + activeNPC + " does not exist.");
                }
            }
        }

        else{ // Maaaaan we ain't found shit!
            Debug.LogWarning("An event has been called but YS node " + eventName + " does not exist.");
        }
    }
    
    public void InterruptAndStartDialogue(string nodeName)
    {
        Debug.Log("Starting interrupt check with: " + nodeName);
        if(Instance.dialogueRunner.CurrentNodeName != null){
            if (!Instance.dialogueRunner.CurrentNodeName.Contains("event")){
                Instance.stoppedNode = Instance.dialogueRunner.CurrentNodeName;
                Debug.Log("Stopped node is: " + Instance.stoppedNode);
            }
        }
        // count how many times we've run this node
        int timesRun = 0;
        foreach(string node in ProgressManager.Instance.brNodesVisited){
            if(node == nodeName){
                timesRun ++;
                Debug.Log("Times Run " + nodeName + ":" + timesRun);
            }
        }
        // get and check the tags on the node
        // if there is a number as first tag, that's the max number of times it should run
        IEnumerable<string> nodeTags = dialogueRunner.GetTagsForNode(nodeName);
        Debug.Log(nodeTags);

        // if there are no tags, just run it
        if (!nodeTags.Contains("1") && !nodeTags.Contains("2") && !nodeTags.Contains("3")){
            Debug.Log("Starting interrupt: " + nodeName);
            StartCoroutine(InterruptSequence(nodeName));
        }
        // otherwise...
        else if(nodeTags.Contains("1") && timesRun < 1){
            Debug.Log("Starting interupt: " + nodeName);
            StartCoroutine(InterruptSequence(nodeName));
        }
        else if(nodeTags.Contains("2") && timesRun < 2){
            Debug.Log("Starting interupt: " + nodeName);
            StartCoroutine(InterruptSequence(nodeName));
        }
        else if(nodeTags.Contains("3") && timesRun < 3){
            Debug.Log("Starting interupt: " + nodeName);
            StartCoroutine(InterruptSequence(nodeName));
        } 
        else{
            Debug.Log("Tried to start dialogue with node " + nodeName + " but it hit max number of runs.");
            return;
        }
    }
    public IEnumerator InterruptSequence(string nodeName){
        if(Instance.dialogueRunner.IsDialogueRunning){
            if(optionListView.optionViews.Count > 0){
                optionListView.DialogueComplete();
                yield return new WaitForSeconds(optionListView.fadeTime);
            }
        }
        Instance.dialogueRunner.Stop();
        yield return new WaitForEndOfFrame();
        Debug.Log("Running interrupt: " + nodeName);
        Instance.dialogueRunner.StartDialogue(nodeName);
    }
    public void UpdateConvoTargets(string characterName){
        lineView.UpdateDialogueTargets(characterName);
        optionListView.UpdateOptionViewTargets(characterName);
    }
// Functions and Commands
    [YarnFunction("get_variable")] 
    public static string GetVariable(string variableName)
    {
        string value = "";
        if(ProgressManager.Instance.storedVariables.TryGetValue(variableName, out value)){
            return value;
        }
        else{
            return "null";
        }
    }
    
    [YarnCommand("set_variable")] 
    public static void SetVariable(string variableName, string value)
    {
        if(variableName != null && value != null){
            ProgressManager.Instance.storedVariables[variableName] = value;
            Debug.Log("Set variable " + variableName + " to " + value);
        }
    }
    
    [YarnCommand("resume")]
    public static void Resume()
    {
        InvokeYSEvent("event-resume", null);
    }
    [YarnCommand("resume_stopped_node")] 
    public static void ResumeStoppedNode()
    {
        Debug.Log("Beginning resume: " + Instance.stoppedNode);
        if(Instance.stoppedNode == ""){
            Debug.LogWarning("No node to resume");
            return;
        }
        else if (Instance.dialogueRunner.NodeExists(Instance.stoppedNode)){
            Instance.InterruptAndStartDialogue(Instance.stoppedNode);
        }
        else{
            Debug.LogWarning("Tried to resume but Node " + Instance.stoppedNode + " does not exist."); 
            Instance.dialogueRunner.OnViewRequestedInterrupt();
            Instance.dialogueRunner.Stop();
        }
    }
    
    [YarnCommand("restart_bridge")]
    public static void RestartBridge()
    {
        GameManager.GM.RestartBridge();
    }
    [YarnCommand("restart_game")]
    public static void RestartGameYS()
    {
        GameManager.GM.RestartGame();
    }
    [YarnCommand("quit_game")]
    public static void QuitGameYS()
    {
        GameManager.GM.QuitGame();
    }
    [YarnCommand("start_coda")]
    public static void StartCoda()
    {
        GameManager.GM.StartCoda();
    }
    [YarnCommand("enable_gun")]
    public static void EnableGun()
    {
        GameManager.GM.EnableGun();
    }
    [YarnCommand("take_bullet")]
    public void TakeBullet(){
        AkSoundEngine.PostEvent("Play_Exchange_Bullet", gameObject);
        PlayerManagerComponent playerMan = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManagerComponent>();
        playerMan.bulletCount -= 1;
        StartCoroutine(GameObject.Find("Bullets").GetComponent<BulletUIManagerComponent>().ShowRemainingBullet(0.25f));
        GameObject.Find("Bullets").GetComponent<BulletUIManagerComponent>().BulletCheck();
        
    }
    
/*
        // Player draws the gun
        //     Check if we're in conversation
        //         If not, do nothing
        //     If so, check void or bridge
        //         Void
        //             Look for gun_draw_event + Active NPC
        //         Bridge
        //             Run gun_draw_bridge_event
        
        // Player shoots
        //     See what they hit
        //     Check void or bridge?
        //     Void
        //         In conversation?
        //             Yes
        //                 Search for a node named: gun_shoot_event + NPC name
        //                     Run it
        //                 If we don't find one, log a warning cause there should be one for each character
        //             No
        //                 Did we hit a door?
        //                     Yes
        //                         Get the owner of the door, set them active speaker
        //                         Search for a node named: gun_shoot_event_door + NPC Name
        //                         Run it
        //                     No
        //                         Set active speaker to Kase
        //                         run node "gun_shoot_event_kase"
        //                         This should target the 2D version of the dialogue presentation
        //                         This will be a random set of things Kase will say to himself.
        //     Bridge
        //         Should always be in conversation.
        //         Run node gun_draw_bridge-event
    */ 
    
}
