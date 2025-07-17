using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using System.Linq;


public class DialogueHandler : MonoBehaviour
{
    [HideInInspector]
    public DialogueRunner dialogueRunner;

    // Start is called before the first frame update
    void Start()
    {
        var runners = FindObjectsOfType<DialogueRunner>();
        if (runners.Length > 1) {
            Debug.LogWarning("There is more than one DialogueRunner in the scene. Only the first one will be used.");
        }
        dialogueRunner = FindObjectsOfType<DialogueRunner>().First();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
