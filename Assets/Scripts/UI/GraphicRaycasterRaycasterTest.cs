using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GraphicRaycasterRaycasterTest : MonoBehaviour
{
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    ScrollViewController scrollViewController;
    GameObject currentGO;
    public float hoverExitWaitTime;
    private bool waiting;
    private bool hovering;
    // Start is called before the first frame update
    void Start()
    {
        m_Raycaster = GetComponent<GraphicRaycaster>();
        m_EventSystem = GetComponent<EventSystem>();
        scrollViewController = GetComponent<ScrollViewController>();
    }

    // Update is called once per frame
    void Update()
    {
        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = new Vector2(0.5f * Screen.width, 0.5f * Screen.height);
        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);

        foreach (RaycastResult result in results) {
            if(result.gameObject.name == "Scroll View" || result.gameObject.name == "Content")
            {
                // if(waiting){
                //     StopCoroutine(HoverExitWait());
                //     waiting = false;
                // }
                //Debug.Log("Hit " + result.gameObject.name);
                scrollViewController.scrollUpAndDown();
                currentGO = result.gameObject;
                hovering = true;
                return;
            }
            else{
                //Debug.Log("Hit:" + result.gameObject.name);
            }
        }
        EndHover();
        //StartCoroutine(HoverExitWait());
    }
    // public IEnumerator HoverExitWait(){
    //     waiting = true;
    //     yield return new WaitForSeconds(hoverExitWaitTime);
    //     waiting = false;
    //     EndHover();
    // }
    void EndHover(){
        if(hovering){
            hovering = false;
            //Debug.Log("Hover ended");
            scrollViewController.EndHover();
        }
        if(currentGO != null){
            //currentGO.Get
        }
    }
}
