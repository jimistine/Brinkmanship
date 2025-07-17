using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableManagerComponent : MonoBehaviour, Interactable
{
    public Animator animator;
    public float animationDismissTime = 1;
    [SerializeField] private bool _IsIneractable = true;
    [HideInInspector]
    public bool isInteractable {
        get {
            return _IsIneractable;
        }
        set {
            _IsIneractable = value;
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

    public void Stared () {
        if (!isInteractable) {
            Debug.Log(gameObject.name + " is set to be non interactable.");
            return;
        }
        animator.SetBool("isAim", true);
        StartCoroutine(PutBack());
    }

    public void Interact() {
        if (!isInteractable) {
            Debug.Log(gameObject.name + " is set to be non interactable.");
            return;
        }
        Debug.Log("Interact with " + gameObject.name);
        gameObject.SetActive(false);
    }

    private IEnumerator PutBack()
    {
        yield return new WaitForSeconds(animationDismissTime);
        animator.SetBool("isAim", false);
    }
}
