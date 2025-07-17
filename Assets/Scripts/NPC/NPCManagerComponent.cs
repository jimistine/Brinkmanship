using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

[RequireComponent(typeof(NPCGizmosDebugComponent))]
[RequireComponent(typeof(DialogueHandler))]
public class NPCManagerComponent : MonoBehaviour, Attackable, Interactable, Dialoguable
{
    // Properties
    private DialogueHandler dialogueHandler;
    private bool isStoreNodeEmpty {
        get {
            return storedNodeName == null || storedNodeName == "";
        }
    } 
    private bool isRunnerOccupied {
        get {
            return dialogueHandler?.dialogueRunner.IsDialogueRunning ?? false;
        }
    }

    public string characterName;
    public string storedNodeName = null;
    public bool runStoredDialogueOnStart = false;
    public bool isInteractableWhenNotDialoguable = false;
    private DialogueInputManager dialogueInputManager;
    private Br_3DLineView lineView;
    private Br_OptionsListView optionListView;
    private ScrollViewController scrollViewController;
    //public GameManager GM;

    [HideInInspector]
    public NPCController npcController;

    // Interface properties
    [SerializeField] private bool _isAttackable = false;
    [HideInInspector]
    public bool isAttackable {
        get {
            return _isAttackable;
        }
        set {
            _isAttackable = value;
        }
    }

    [SerializeField] private bool _IsIneractable = false;
    [HideInInspector]
    public bool isInteractable {
        get {
            if (isInteractableWhenNotDialoguable) {
                return _IsIneractable;
            } else {
                return _IsIneractable && isDialoguable;
            }
        }
        set {
            _IsIneractable = value;
        }
    }

    [SerializeField] private bool _isDialoguable = true;
    [HideInInspector] public bool isDialoguable {
        get {
            return _isDialoguable && !isStoreNodeEmpty && !isRunnerOccupied; 
        }
        set {
            _isDialoguable = value;
        }
    }

    // Events

    // Static events
    public delegate void OnNPCDeathInScene(GameObject npc);
    public static event OnNPCDeathInScene OnNPCDeathInSceneEvent;

    // Non-static actions
    public event Action OnDeathAction;
    public event Action OnAttackAction;
    public event Action OnInteractAction;
    public event Action OnAimedAction;
    public event Action OnStaredAction;
    public event Action OnDialogueStartAction;

    // Unity events
    public UnityEvent OnDeath;

    public UnityEvent OnAttack;

    public UnityEvent OnInteract;

    public UnityEvent OnAimed;
    public UnityEvent OnStared;
    public UnityEvent OnDialogueStart;

    // Start is called before the first frame update
    void Start()
    {
        npcController = GetComponent<NPCController>();
        dialogueHandler = GetComponent<DialogueHandler>();
        dialogueInputManager = GameObject.FindWithTag("Player").GetComponent<DialogueInputManager>();
        optionListView = GameObject.FindWithTag("OptionsListView").GetComponent<Br_OptionsListView>();
        scrollViewController = GetComponentInChildren<ScrollViewController>();


        if(GameManager.GetCurrentScene() == "02_Void"){
            lineView = GameObject.FindWithTag("LineView").GetComponent<Br_3DLineView>();
            // set the panel alphas to 0 when we start the game
            CanvasGroup lineViewCanvasGroup = GetComponentsInChildren<CanvasGroup>()[0];
            lineViewCanvasGroup.alpha = 0;
            CanvasGroup optionViewCanvasGroup = GetComponentsInChildren<CanvasGroup>()[2];
            optionViewCanvasGroup.alpha = 0;
            
        }
        else{
            //optionListView.CanvasGroup.alpha = 0f;
        }

        if (runStoredDialogueOnStart) {
            StartStoredDialogue(false );
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Attacked() {
        Debug.Log("Shot " + gameObject.name);
        if (!isAttackable) {
            Debug.Log(gameObject.name + " is set to be non attackable.");
            return;
        } 
        if(GameManager.GetCurrentScene() == "02_Void"){
            lineView.UpdateDialogueTargets(characterName);
            optionListView.UpdateOptionViewTargets(characterName);
            DialogueManager.activeNPC = characterName;
            DialogueManager.InvokeYSEvent("gun_shoot_event", characterName);
        }
        else if(GameManager.GetCurrentScene() == "03_Bridge"){
            if (npcController == null) {
                Debug.LogWarning("NPC is attackable can't find its npcController"); 
                return;
            }
            Kill();
        }

        OnAttack.Invoke();
        OnAttackAction?.Invoke();
    }

    public void Interact(bool isThreatened = false) {
        if (isDialoguable) {
            if(GameManager.GetCurrentScene() == "02_Void"){
                lineView.UpdateDialogueTargets(characterName);
                optionListView.UpdateOptionViewTargets(characterName);
            }
            DialogueManager.activeNPC = characterName;
            StartStoredDialogue(isThreatened);
        }

        OnInteract.Invoke();
        OnInteractAction?.Invoke();
    }

    public void StartStoredDialogue(bool isThreatened = false) {
        StartDialogue(storedNodeName, isThreatened);
    }

    public void StartDialogue(string nodeName = null, bool isThreatened = false) {
        if (isStoreNodeEmpty) {
            Debug.LogWarning("No node name was provided, couldn't start node.");
            return;
        }
        
        DialogueRunner runner;
        if(GetComponentsInChildren<DialogueRunner>() != null){
            runner = GetComponentInChildren<DialogueRunner>();
        } 
        else {
            runner = DialogueManager.Instance.dialogueRunner;
        }
        
        Debug.Log((isThreatened ? "(Threatened) " : "(Not Threatened) ") + "Starting dialogue with node: " + nodeName);
        dialogueHandler.dialogueRunner.StartDialogue(nodeName);
        DialogueManager.SetVariable("dialogueOwner", gameObject.name);
        DialogueManager.InvokeYSEvent("event-npc-talk", this.name);
        
        OnDialogueStart.Invoke();
        OnDialogueStartAction?.Invoke();
        
    }

    public void Aimed() {
        OnAimed.Invoke();
        OnAimedAction?.Invoke();
    }

    public void Stared() {
        OnStared.Invoke();
        OnStaredAction?.Invoke();
    }

    private void Kill()
    {   
        npcController.Kill();
        
        OnDeath.Invoke();
        OnDeathAction?.Invoke();
        OnNPCDeathInSceneEvent?.Invoke(this.gameObject);
    }
}
