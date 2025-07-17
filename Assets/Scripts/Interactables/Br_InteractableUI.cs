using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class Br_InteractableUI : Selectable
{

    public bool isButton;
    public bool isScrollRect;
    private Button uiButton;
    private ScrollRect scrollRect;

    [SerializeField] private bool _IsIneractable = true;
    [HideInInspector]
    public bool isInteractable {
        get{
            return _IsIneractable;
        }
        set{
            _IsIneractable = value;
        }
    }
    // void OnEnable(){
    //     CameraRayComponent.OnInteractableEvent += OnInteractableEvent;
    // }
    // void OnDisable(){
    //     CameraRayComponent.OnInteractableEvent -= OnInteractableEvent;
    // }

    // Start is called before the first frame update
    // void Start()
    // {
        
    //     if(GetComponentInParent<Button>() == null){
    //         scrollRect = GetComponentInParent<ScrollRect>();
    //         isScrollRect = true;
    //     }
    //     else{
    //         uiButton = GetComponentInParent<Button>();
    //         isButton = true;
    //     }
    // }

    public void Interact(bool isThreatened = false) {
        Debug.Log("Interacted with: " + gameObject.name);
    }

    public void Stared(){

    }


    void OnInteractableEvent(bool isInteractable, string interactableLabel){
        if (isInteractable){
            // make ui selected
            Debug.Log("UI is selected");
            if(uiButton != null){
                uiButton.Select();
            }
            else{
                //scrollRect.Select();
            }
        }
        else{
            // make ui unselected
            Debug.Log("UI is deselected");
            if(uiButton != null){
                uiButton.Select();
            }
            else{
                //scrollRect.Select();
            }
        }
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
