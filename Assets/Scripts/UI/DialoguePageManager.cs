using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialoguePageManager : MonoBehaviour
{

    public GameObject dialoguePanelContainer;
    public GameObject pagePrefab;
    
    
    private TextMeshProUGUI currentPageText;

    // Start is called before the first frame update
    void Start()
    {
        currentPageText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        // this should probably be moved to only run when the dialogue runner is running or something...
        CheckPage();
        //Debug.Log("First overflow character index: " + currentPageText.firstOverflowCharacterIndex);
    }
    void CheckPage(){
        if(currentPageText.firstOverflowCharacterIndex > 1){
            AddPage();
        }
    }
    void AddPage(){
        GameObject newPage = Instantiate(pagePrefab, dialoguePanelContainer.transform);
        currentPageText.linkedTextComponent = newPage.GetComponentInChildren<TextMeshProUGUI>();
        currentPageText = newPage.GetComponentInChildren<TextMeshProUGUI>();
    }
}
