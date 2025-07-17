using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalDialogueManagerComponent : MonoBehaviour
{
    [SerializeField] private bool _isDialoguable = true;
    [HideInInspector] public bool isDialoguable {
        get {
            return _isDialoguable;
        }
        set {
            _isDialoguable = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartDialogue(string nodeName) {
        Debug.Log("Starting dialogue with " + nodeName);
    }
}
