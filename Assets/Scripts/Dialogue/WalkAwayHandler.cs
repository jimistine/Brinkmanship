using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkAwayHandler : MonoBehaviour
{
    public string parentNPC;
    public NPCAudioController npcAC;
    // Start is called before the first frame update
    void Start()
    {
        parentNPC = transform.parent.name;
    }

   
    private void OnTriggerExit(Collider other) {
        //Debug.Log("Exiting, " + other.gameObject);
        if(other.gameObject.tag is "Player"){
            if(DialogueManager.Instance.isDialogueRunning){
                DialogueManager.Instance.dialogueRunner.Stop();
                AkSoundEngine.PostEvent("PanelDisappear", gameObject);
            }
        // When you walk away we hear the NPC get back to what they were doing
            if(parentNPC == "Door_Brine"){
                npcAC.StartTVLayer();
            }
            else if(parentNPC == "Door_Tav"){
                npcAC.StartInteractLayer();
            }
            else if(parentNPC == "Door_Parents"){
                npcAC.StartDomesticLayer();
            }
            else if(parentNPC == "Door_Urnst"){
                npcAC.StartSpritzLayer();
            }
            else if(parentNPC == "Door_Prisoner"){
                npcAC.StartHummingLayer();
            }
        }
    }
}
