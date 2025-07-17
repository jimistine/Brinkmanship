using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEditor;



public class NPCGizmosDebugComponent : MonoBehaviour

{

    [SerializeField] private bool showGizmos = true;

    private string text;



    void OnDrawGizmos ()

    {

        if (showGizmos) {

            DrawDebugText();

        }

    }



    void DrawDebugText()

    {

        text = GetName() + GetAttackable() + GetInteractable() + GetDialougable();

        //Handles.Label(transform.position, text);

    }



    string GetName()

    {

        return gameObject.name;

    }



    string GetAttackable()

    {

        Attackable attackable = GetComponent<Attackable>();

        return "\nAttackable: " + (attackable?.isAttackable.ToString() ?? "null");

    }

    

    string GetInteractable()

    {

        Interactable interactable = GetComponent<Interactable>();

        return "\nInteractable: " + (interactable?.isInteractable.ToString() ?? "Null");

    }



    string GetDialougable()

    {

        Dialoguable dialoguable = GetComponent<Dialoguable>();

        return "\nDialoguable: " + (dialoguable?.isDialoguable.ToString() ?? "Null");

    }

}

