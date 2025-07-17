using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;
using UnityEngine.Events;


/// <summary>
/// Manages player dialogue inputs.
/// </summary>

 
public class DialogueInputManager : MonoBehaviour
{
    public Br_OptionsListView optionsListView;
    public DialogueRunner DR;
    public Br_2DLineView lineView;
    public Br_3DLineView voidLineView;
	private PlayerInput _playerInput;
    private GameManager GM;
    public bool hitContinue;

    // Start is called before the first frame update
    void Start()
    {
		_playerInput = GetComponent<PlayerInput>();
        GM = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        if(GameManager.GetCurrentScene() == "02_Void"){
            voidLineView = GameObject.FindWithTag("LineView").GetComponent<Br_3DLineView>();
        }
        
    }


    public void OnSelectOption_1(InputValue inputValue){
        if(optionsListView.optionViews.Count > 0){
            Debug.Log("picked first option");
            //optionsListView.optionViews[0].InvokeOptionSelected();
            optionsListView.availableOptionViews[0].InvokeOptionSelected();
        }
    }
    public void OnSelectOption_2(InputValue inputValue){
        if(optionsListView.optionViews.Count > 1){
           // optionsListView.optionViews[1].InvokeOptionSelected();
            optionsListView.availableOptionViews[1].InvokeOptionSelected();
        }
    }
    public void OnSelectOption_3(InputValue inputValue){
        if(optionsListView.optionViews.Count > 2){
            //optionsListView.optionViews[2].InvokeOptionSelected();
            optionsListView.availableOptionViews[2].InvokeOptionSelected();
        }
    }
    public void OnSelectOption_4(InputValue inputValue){
        if(optionsListView.optionViews.Count > 3){
            //optionsListView.optionViews[3].InvokeOptionSelected();
            optionsListView.availableOptionViews[3].InvokeOptionSelected();
        }
    }
    private void SelectAvailableOption(int n){
        foreach(Br_OptionView option in optionsListView.optionViews){
            if(option.enabled){

            }
        }
    }

    public void OnAdvanceDialogue(InputValue inputValue){
        if(DR.IsDialogueRunning){
            if(inputValue.isPressed && !hitContinue){
                //DR.Dialogue.Continue();
                if(lineView != null){
                    lineView.OnContinueClicked();
                }
                else if(voidLineView != null){
                    voidLineView.OnContinueClicked();
                }
                hitContinue = true;
            }
        }
        if(!inputValue.isPressed){
            hitContinue = false;
        }
        else{
            Debug.Log("Cannot advance as dialogue is not running.");
        }
    }
}

