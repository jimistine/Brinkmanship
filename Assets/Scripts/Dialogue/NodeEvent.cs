using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class NodeEvent : MonoBehaviour
{
    public delegate void OnNodeStart(string s);
    public static event OnNodeStart OnNodeStartEvent;

    private void OnEnable()
    {
        DialogueRunner dialogueRunner = GetComponent<DialogueRunner>();
        dialogueRunner.onNodeStart.AddListener(_OnNodeStart);
    }

    private void _OnNodeStart(string s)
    {
        OnNodeStartEvent.Invoke(s);
        return;
    }
}
