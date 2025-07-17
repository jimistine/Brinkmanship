using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yarn.Unity;

/// <summary>
/// NPCSystem is a static class that manages all the NPCs in the scene. There can only be one instance of this class in the scene.
/// </summary>
public class NPCSystem : MonoBehaviour
{
    public static NPCSystem Instance { get; private set;  }
    
    private List<string> namesOfNpcThatDied = new List<string>();
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("More than one instance of NPCSystem found in scene. This is not allowed. Destroying this (" + gameObject.name + ") instance. Please check your scene.");
            Destroy(this);
            return;
        }
        else
        {
            Instance = this; 
        }
    }
    
    void OnEnable() {
        NPCManagerComponent.OnNPCDeathInSceneEvent += LogNPCDeath;
    }

    void OnDisable() {
        NPCManagerComponent.OnNPCDeathInSceneEvent -= LogNPCDeath;
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void LogNPCDeath(GameObject npc) {
        namesOfNpcThatDied.Add(npc.name);
        Debug.Log("NPC Death in Scene: " + npc.name);
    }
    
    [YarnFunction("is_all_npc_dead")] 
    public static bool IsAllNpcInSceneDead()
    {
        foreach (var npcInScene in Instance.FindNpcsInScene())
        {
            if (!IsNpcDead(npcInScene.name))
            {
                return false;
            }
        }
        return true;
    }
    
    [YarnFunction("is_npc_dead")]
    public static bool IsNpcDead(string npcName) {
        return Instance.namesOfNpcThatDied.Contains(npcName);
    }
    
    List<GameObject> FindNpcsInScene()
    {
        List<GameObject> npcs = new List<GameObject>();
        
        NPCManagerComponent[] managerComponents = FindObjectsOfType(typeof(NPCManagerComponent)) as NPCManagerComponent[];
        foreach (var n in managerComponents)
        {
            npcs.Add(n.gameObject);
            print(n.gameObject.name);
        }
        return npcs;
    }
}
