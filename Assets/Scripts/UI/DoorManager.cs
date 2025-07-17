using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

[Obsolete("Use NPCManager instead of DoorManager")]
public class DoorManager : MonoBehaviour
{
    private DialogueRunner dialogueRunner;
    public GameObject dialogueSystem3DCanvas;
    public string startNode;

    public bool isInteractable
    {
        get
        {
            return _isInteractable;
        }

        set
        {
            _isInteractable = value;
        }
           
    }

    bool _isInteractable = true;
    // Start is called before the first frame update
    void Start()
    {
        dialogueRunner = FindAnyObjectByType<DialogueRunner>();
        //dialogueRunner = dialogueSystem3DPrefab.GetComponentInChildren<DialogueRunner>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        //Test 
        Debug.Log("Door " + gameObject.name + " is activated.");
        dialogueSystem3DCanvas.SetActive(true);
        dialogueRunner.StartDialogue(startNode);
    }

    public void Aimed()
    {

    }
}
