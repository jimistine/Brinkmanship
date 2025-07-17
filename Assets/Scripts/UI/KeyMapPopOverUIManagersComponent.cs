using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyMapPopOverUIManagersComponent : MonoBehaviour
{
    public TMP_Text actionPrompt;
    // Start is called before the first frame update
    void Start()
    {
        Hide();
    }

    void OnEnable()
    {
        CameraRayComponent.OnInteractableEvent += OnInteractableEvent;
    }

    void OnDisable()
    {
        CameraRayComponent.OnInteractableEvent -= OnInteractableEvent;
    }

    void OnInteractableEvent(bool isInteractable, string interactableLabel)
    {
        if (isInteractable)
        {
            actionPrompt.text = interactableLabel;
            Show();
        }
        else
        {
            Hide();
        }
    }

    void Hide()
    {
        foreach (Transform child in transform)
        {
            GameObject childObject = child.gameObject;
            childObject.SetActive(false);
        }
    }

    void Show()
    {
        foreach (Transform child in transform)
        {
            GameObject childObject = child.gameObject;
            childObject.SetActive(true);
        }
    }
}
