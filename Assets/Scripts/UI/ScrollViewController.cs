using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScrollViewController : MonoBehaviour
{
    //Help to checkout
    [SerializeField] private GameObject obj;
    [SerializeField] private float scrollKeyPace = 50.0f;
    [SerializeField] private float scrollMousePace = 50.0f;
    public ScrollRect scrollRect;
    public PanelSoundManager soundManager;
    public bool isHovered;
    public bool effectsPlayed;
    public CanvasGroup canvasGroup;
    [Range(0.0f, 1.0f)]
    public float startingPanelAlpha;
    [Range(0.0f, 1.0f)]
    public float fadeAlpha;
    private Tween fadeIn;
    private float preHeight;

    // Start is called before the first frame update
    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = startingPanelAlpha;
        soundManager = GetComponentInParent<PanelSoundManager>();
        preHeight = obj.GetComponent<RectTransform>().rect.height;
    }

    void OnEnable(){
        //TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ScrollToNewLine);
    }

    // Update is called once per frame
    void Update()
    {
        //scrollRect.verticalNormalizedPosition = 0; // value range (0 to 1)
        
        if(obj.GetComponent<RectTransform>().rect.height != preHeight)
        {
            ScrollToNewLine();
            preHeight = obj.GetComponent<RectTransform>().rect.height;
        }

        if(isHovered && effectsPlayed && !fadeIn.active){
            canvasGroup.alpha = 1;
        }
    }
    public void scrollUpAndDown()
    {
        Vector3 pos = obj.transform.localPosition;
        Vector2 mouseScroll = Input.mouseScrollDelta;
        if (Input.GetKeyDown(KeyCode.I))
            pos.y -= scrollKeyPace;
        if (Input.GetKeyDown(KeyCode.K))
            pos.y += scrollKeyPace;
        if (mouseScroll.y != 0)
            pos.y -= mouseScroll.y * scrollMousePace;
        obj.transform.localPosition = pos;

    // On Hover Effects
        if(!effectsPlayed){
            effectsPlayed = true;    
            soundManager.OnScrollHover();
            fadeIn = DOTween.To(()=> canvasGroup.alpha, x => canvasGroup.alpha = x, 1f,0.25f);
        }
        isHovered = true;
    }
    public void EndHover(){
        if(effectsPlayed){
            effectsPlayed = false;
            isHovered = false;
            soundManager.OnEndHover();
            DOTween.To(()=> canvasGroup.alpha, x => canvasGroup.alpha = x, fadeAlpha, 0.5f);
            Debug.Log("fade updated");
        }
    }

    // private void keepScrollButton()
    // {
    //     if (preDia != textMesh.text)
    //     {
    //         Debug.Log("text is changed!!!");
    //         scrollRect.verticalNormalizedPosition = 0;
    //         preDia = textMesh.text;
    //     }
    // }
    public void ScrollToNewLine(){
        scrollRect.verticalNormalizedPosition = 0; // value range (0 to 1)
        
    }
}
