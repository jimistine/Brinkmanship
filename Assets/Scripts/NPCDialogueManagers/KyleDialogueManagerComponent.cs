using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KyleDialogueManagerComponent : MonoBehaviour
{
    bool _isDialoguable = true;
    public bool isDialoguable {
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

    public void StartDialogue(string nodeName) {
        Debug.Log("Starting dialogue with " + nodeName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
