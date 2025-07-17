using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public interface Dialoguable {
    bool isDialoguable {
        get;
        set;
    }
    
    void StartDialogue(string nodeName = null) {
        StartDialogue(nodeName, isThreatened: false);
    }

    void StartDialogue(string nodeName = null, bool isThreatened = false) {
        Debug.LogWarning("StartDialogue() not implemented");
    }
}