using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using UnityEngine;

[RequireComponent(typeof(BehaviorTree))]
public class NPCController : MonoBehaviour
{
    private Rigidbody[] rigidbodies;
    private Collider[] colliders;
    private Animator animator;
    private BehaviorTree behaviorTree;
    private UnityEngine.AI.NavMeshAgent navMeshAgent;

    public delegate void OnNPCDeath(GameObject npcKilled);
    public static event OnNPCDeath OnNPCDeathEvent;

    // Start is called before the first frame update
    void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
        animator = GetComponent<Animator>();
        behaviorTree = GetComponent<BehaviorTree>();
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        SetRigidState(true);
        SetColliderState(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnimatorVelocity();
    }

    public void Kill()
    {
        animator.enabled = false;
        behaviorTree.enabled = false;

        SetRigidState(false);
        SetColliderState(true);

        OnNPCDeathEvent?.Invoke(gameObject);
    }

    private void SetRigidState(bool state)
    {
        foreach(Rigidbody rigidbody in rigidbodies)
        {
            if (rigidbody.gameObject.tag != "NPC") {
                rigidbody.isKinematic = state;
            }
            else
            {
                rigidbody.isKinematic = !state;
            }
        }

    }

    private void SetColliderState(bool state)
    {
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag != "NPC")
            {
                collider.enabled = state;
            }
            else
            {
                collider.enabled = !state;
            }
        }

    }

    private void UpdateAnimatorVelocity()
    {
        float velocity = navMeshAgent.velocity.magnitude / navMeshAgent.speed;

        //animator.SetFloat("velocity", velocity);

    }
}
