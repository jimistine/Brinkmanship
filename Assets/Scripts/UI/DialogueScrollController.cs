using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class DialogueScrollController : MonoBehaviour
{
    
    private ScrollRect scrollRect;
    public TextMeshProUGUI convoText;
    // public Br_3DLineView lineView;

    // Start is called before the first frame update
    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        convoText = GetComponentInChildren<TextMeshProUGUI>();
    }
    void OnEnable(){
        //Br_3DLineView.onRunLine += ScrollToNewLine;
        //Br_OptionsListView.onSelectionDone += ScrollToNewLine;
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ScrollToNewLine);
    }
    void OnDisable(){
        //Br_3DLineView.onRunLine -= ScrollToNewLine;
        //Br_OptionsListView.onSelectionDone -= ScrollToNewLine;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Q)){
            ScrollToNewLine(null);
        }
    }
    
    public void ScrollToNewLine(Object obj){
        Debug.Log(obj);
        if(obj == convoText){
            scrollRect.verticalNormalizedPosition = 0; // value range (0 to 1)
        }

    }
}
