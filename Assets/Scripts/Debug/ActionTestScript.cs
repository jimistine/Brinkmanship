using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTestScript : MonoBehaviour
{
    public NPCManagerComponent npcManagerComponent;

    void OnEnable() {
        npcManagerComponent.OnDeathAction += LogNPCDeath;
    }

    void OnDisable() {
        npcManagerComponent.OnDeathAction -= LogNPCDeath;
    }

    // Start is called before the first frame update
    void Start()
    {
           
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LogNPCDeath() {
        Debug.Log("Action is called");
    }
}
