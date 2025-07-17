using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Febucci.UI;


public class PanelSoundManager : MonoBehaviour
{

    public GameObject convoPanel;
    public GameObject bioPanel;
    public GameObject npcAudioObj;
    private ScrollViewController scrollViewController;
    public NPCManagerComponent nPCManagerComponent;
    public TypewriterByCharacter charName;
    public TypewriterByCharacter charTitle;
    public TypewriterByCharacter upperText;
    public TypewriterByCharacter lowerText;
    private bool typeWriterTriggered;

    // Start is called before the first frame update
    void Start()
    {
        nPCManagerComponent = GetComponentInParent<NPCManagerComponent>();
        convoPanel = gameObject.transform.GetChild(0).gameObject;
        bioPanel = gameObject.transform.GetChild(1).gameObject;
        //DialogueManager.Instance.dialogueRunner.onDialogueComplete.AddListener(OnPanelExit());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable(){
        nPCManagerComponent.OnDialogueStartAction += OnPanelEnter;
    }
    void OnDisable(){
        nPCManagerComponent.OnDialogueStartAction -= OnPanelEnter;
    }
    public void OnScrollHover(){
        AkSoundEngine.PostEvent("Hover_1", convoPanel);
    }
    public void OnEndHover(){
        AkSoundEngine.PostEvent("HoverGone_1", convoPanel);
    }
    void OnPanelEnter(){
        AkSoundEngine.PostEvent("PanelAppear", convoPanel);
        AkSoundEngine.PostEvent("PanelAppear", bioPanel);
        
        if(!typeWriterTriggered){
            typeWriterTriggered = true;
            charName.StartShowingText();
            charTitle.StartShowingText();
            upperText.StartShowingText();
            lowerText.StartShowingText();
        }
    }
    // public UnityEngine.Events.UnityAction OnPanelExit(){
    //     AkSoundEngine.PostEvent("PanelDisappear", convoPanel);
    //     return null;
    // }
}
