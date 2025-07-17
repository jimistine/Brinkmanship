using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
public class Br_WwiseDialogueView : DialogueViewBase
{
    private Action CompleteDialogue;
    public GameObject CaptainGameObject = null;
    public GameObject PlayerGameObject = null;
    public GameObject PilotGameObject = null;
    public GameObject AilynDoorGameObject = null;
    public GameObject UrnstDoorGameObject = null;
    public GameObject ParentsDoorGameObject = null;
    public GameObject TavDoorGameObject = null;
    public GameObject ForresterDoorGameObject = null;
    public GameObject BrineDoorGameObject = null;
    public GameObject PrisonerDoorGameObject = null;
    public GameObject BridgeDoorGameObject = null;
    private GameObject LastPlayedObject = null;
    
    void OnEnable(){
        NPCManagerComponent.OnNPCDeathInSceneEvent += StopVOOnDeath;
    }
    void OnDisable(){
        NPCManagerComponent.OnNPCDeathInSceneEvent -= StopVOOnDeath;
    }
    
    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        //Debug.Log(gameObject.name);
        // save the dialogue finish action for later
        CompleteDialogue = onDialogueLineFinished;
        
        // list of akevent names
        string[] options = {"Play_1sec","Play_3sec","Play_5sec" };
        
        // set up callback mask for the duration and the end of event
        uint callbackType = (uint)(AkCallbackType.AK_EndOfEvent);
        string lineName = dialogueLine.TextID.Replace(":","");
        string eventName = "Play_" + lineName;
        string[] eventArray = {eventName};
        
        AKRESULT result = AkSoundEngine.PrepareEvent(AkPreparationType.Preparation_Load, eventArray,1);
        // post event with DialogueCallback as the callback function
        if (result == AKRESULT.AK_Success)
        {
            GameObject CurGameObject = WwiseManager.ins;
            if (PlayerGameObject != null)
                CurGameObject = PlayerGameObject;
            switch (dialogueLine.CharacterName)
            {
                case "Captain": 
                    if(CaptainGameObject != null)
                        CurGameObject = CaptainGameObject;
                    break;
                case "Pilot":
                    if(PilotGameObject != null)
                        CurGameObject = PilotGameObject;
                    break;
                case "Mom":
                    if(ParentsDoorGameObject != null){
                        CurGameObject = ParentsDoorGameObject; 
                    }
                    break;
                case "Dad":
                    if(ParentsDoorGameObject != null)
                        CurGameObject = ParentsDoorGameObject;
                    break;
                case "Ailyn":
                    if(AilynDoorGameObject != null)
                        CurGameObject = AilynDoorGameObject;
                    break;
                case "Urnst":
                    if(UrnstDoorGameObject != null)
                        CurGameObject = UrnstDoorGameObject;
                    break;
                case "Kee":
                    if(ForresterDoorGameObject != null)
                        CurGameObject = ForresterDoorGameObject;
                    break;
                case "Tav":
                    if(TavDoorGameObject != null)
                        CurGameObject = TavDoorGameObject;
                    break;
                case "Prisoner":
                    if(PrisonerDoorGameObject != null)
                        CurGameObject = PrisonerDoorGameObject;
                    break;
                case "Brine":
                    if(BrineDoorGameObject != null)
                        CurGameObject = BrineDoorGameObject;
                    break;
            }
            if (LastPlayedObject != null){
                if(LastPlayedObject == CurGameObject){
                    AkSoundEngine.PostEvent("STOPVO", LastPlayedObject);
                }
            }
            AkSoundEngine.PostEvent(eventName,CurGameObject,callbackType,DialogueCallback,null);
            LastPlayedObject = CurGameObject;
            Debug.Log(CurGameObject);
        }
        else
        {
            CompleteDialogue?.Invoke();
        }
        
    }
    public override void InterruptLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        // insert code to stop the wwise event
        if (LastPlayedObject != null)
            AkSoundEngine.PostEvent("STOPVO", LastPlayedObject);
        onDialogueLineFinished?.Invoke();
    }
    public override void DismissLine(Action onDismissalComplete)
    {
        // insert code to stop the wwise event
        if (LastPlayedObject != null)
            AkSoundEngine.PostEvent("STOPVO", LastPlayedObject);
        onDismissalComplete?.Invoke();
    }
    
    public void DialogueCallback(object in_cookie, AkCallbackType in_type, object in_info)
    {
        // if duration callback log the duration
        if (in_type == AkCallbackType.AK_Duration)
        {
            AkEventCallbackInfo info = (AkEventCallbackInfo)in_info; 
            AkDurationCallbackInfo durationInfo = (AkDurationCallbackInfo)info;
            Debug.Log("Dialogue lasts " + durationInfo.fDuration + "ms");
        }
        // if end of event signal to DialogueRunner that line is complete
        if (in_type == AkCallbackType.AK_EndOfEvent)
        {
            //Debug.Log("Dialogue ended");
            CompleteDialogue?.Invoke();
        }
    }

    public void StopVOOnDeath(GameObject deadNPC){
        if (deadNPC != null){
            if (deadNPC.name == "Captain"){
                AkSoundEngine.PostEvent("STOPVO", CaptainGameObject);
            }
            else if (deadNPC.name == "Pilot"){
                AkSoundEngine.PostEvent("STOPVO", PilotGameObject);
            }
        }
    }
}
