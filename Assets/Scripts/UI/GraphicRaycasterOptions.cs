using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Yarn.Unity;

public class GraphicRaycasterOptions : MonoBehaviour
{
    public Color hoverColor;
    public Color startColor;
    private GraphicRaycaster m_Raycaster;
    private PointerEventData m_PointerEventData;
    private EventSystem m_EventSystem;
    private TextMeshProUGUI textMesh;
    private bool hovering;
    private bool hovered;
    // Start is called before the first frame update
    void Start()
    {
        m_Raycaster = GetComponent<GraphicRaycaster>();
        m_EventSystem = GetComponent<EventSystem>();
        textMesh = GetComponent<TextMeshProUGUI>();
        hovering = false;
        hovered = false;
    }

    // Update is called once per frame
    void Update()
    {
        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = new Vector2(0.5f * Screen.width, 0.5f * Screen.height);
        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);
        hovering = false;

        foreach(RaycastResult result in results)
        {
            if(result.gameObject.name == "Br 3D Option View(Clone)")
            {
                //Debug.Log(result.gameObject.name);
                result.gameObject.GetComponent<TextMeshProUGUI>().color = hoverColor;
                if(hovered == false){
                    AkSoundEngine.PostEvent("Play_Ui_2_Hover_b", gameObject);
                    hovered = true;
                }
                hovering = true;
                //result.gameObject.GetComponent<Br_OptionView>();
                if(Input.GetKeyUp(KeyCode.E) || Input.GetMouseButtonUp(0)){
                    result.gameObject.GetComponent<Br_OptionView>().InvokeOptionSelected();
                }
            }
        }
        EndHover();
    }

    void EndHover()
    {
        if (!hovering)
        {
            textMesh.color = startColor;
            hovered = false;
        }
    }
}
