using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.Events;
public enum Sequence
{
    TitleScreen,
    MainMenu,
    ContentAdvisory,
    Logos,
    TheDiscovery,
    LockdownEscape,
    FirstSteps,
    Parents,
    FollowingUp,
    TheInvestigationBegins,
    Urnst,
    JonnBrine,
    Tav,
    SupervisorForresterKee,
    Prisoner,
    PrepForTheShowdown,
    TheAirlock,
    TheBridge,
    EndingOne,
    EndingTwo,
    EndingThree,
    WhoWeAre,
    AssetsWeUsed
}
public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }
    // Start is called before the first frame update
    private DialogueRunner dialogueRunner;
    public List<Sequence> visitedSequence = new List<Sequence>();
    public List<string> brNodesVisited = new List<string>();
    public UserTesting testingInfo;
    
    public IDictionary<string, string> storedVariables = new Dictionary<string, string>();
    
    public int numBullets;
    // This system lets us set the state of the players knowledge at will, esp useful for tesing bridge
    [System.Serializable]
    public struct Intel{
        public string name;
        public bool status;
    }
    [Header("Intel Tracking")]
    public bool learnedEverything;
    public List<Intel> intelLearned = new List<Intel>();

    private Sequence _currentSequence;
    public static Sequence currentSequenceYS;
    public Sequence currentSequence
    {
        get
        {
            return _currentSequence;
        }
        set
        {
            Sequence previousSequence = _currentSequence;
            visitedSequence.Add(value);
            OnNewSequenceEvent?.Invoke(previousSequence, value);
            _currentSequence = value;
        }
    }
    
    public delegate void OnNewSequence(Sequence previousSequence, Sequence newSequence);
    public static event OnNewSequence OnNewSequenceEvent;
    
    [YarnCommand("setSequence")]
    public void setSequence(string sequence)
    {
        Instance.currentSequence = StringToSequence(sequence);
        currentSequenceYS = StringToSequence(sequence);
        // invoke event here that the void manager and others can subscribe to
    }
    private Sequence StringToSequence(string sequence)
    {
        return Enum.Parse<Sequence>(sequence);
    } 
    
    public static bool HasVisitedSequence(Sequence sequence)
    {
        return Instance.visitedSequence.Contains(sequence);
    }
    
    [YarnFunction("getSequence")]
    public static string GetSequence()
    {
        return currentSequenceYS.ToString();
    }

    void OnEnable(){
        PlayerManagerComponent.OnBulletCountChangeEvent += UpdateBulletCount;
    }
    void OnDisable(){
        PlayerManagerComponent.OnBulletCountChangeEvent -= UpdateBulletCount;
    }

    private void Awake()
    {
        

        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("More than one instance of ProgressManager found in scene. This is not allowed. Destroying this (" + gameObject.name + ") instance. Please check your scene.");
            Destroy(this);
            return;
        }
        else
        {
            Instance = this; 
        }

        // Add everything that we flagged in the inspector to the actual knowledge base
        foreach (Intel intel in intelLearned){
            if(intel.status == true || learnedEverything){
                storedVariables[intel.name] = "true";
            }
        }
    }

    void UpdateBulletCount(int newNumBullets){
        numBullets = newNumBullets;
    }
    public void LearnAllIntel(){
        foreach (Intel intel in intelLearned){
            storedVariables[intel.name] = "true";
            learnedEverything = true;
        }
    }
    public void Br_NodesVisited(string node){
        brNodesVisited.Add(node);

        testingInfo.AddNodeInfo(node, DateTime.Now);
    }
    void Start(){
    }
    public void SetProgManagerVars(){
        dialogueRunner = FindAnyObjectByType<DialogueRunner>();
        dialogueRunner.onNodeStart.AddListener(Br_NodesVisited);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManagerComponent>().bulletCount = numBullets;
    }
}
