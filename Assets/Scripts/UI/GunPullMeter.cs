using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Yarn.Unity;


public class GunPullMeter : MonoBehaviour
{
    private Slider slider;
    private CanvasGroup canvasGroup;
    private Tween fillTween;
    private Tween emptyTween;
    public GunManagerComponent gunManager;
    public GameObject instructionsDraw;
    public GameObject instructionsShoot;
    public Animator gunPullAnim;
    // Start is called before the first frame update
    void Start()
    {
        gunManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<GunManagerComponent>();
        slider = GetComponent<Slider>();
        canvasGroup = GetComponent<CanvasGroup>();
        
    }

    private void OnEnable(){
    }
    private void OnDisable() {
    }

    // Update is called once per frame
    void Update()
    {
        if(gunManager.gunEnabled){
            if(Input.GetKey(KeyCode.R) || Input.GetMouseButton(1)){
                FillMeter();
            }
            else if(slider.value > 0.0001){
                EmptyMeter();
            }
        }
    }
    void FillMeter(){
        if( emptyTween != null && emptyTween.active){emptyTween.Kill();}
        fillTween = DOTween.To(()=> slider.value, x => slider.value = x, 1.0f, 1.0f);
        if(slider.value < 0.975){
            //canvasGroup.alpha = 1;
            canvasGroup.DOFade(1, 0.5f);

        }
        else{
            // Gun is drawn
            gunPullAnim.SetBool("Draw_Reminder", false);
            instructionsDraw.SetActive(false);
            instructionsShoot.SetActive(true);
            //canvasGroup.DOFade(1, 0.5f);
            //canvasGroup.alpha = 0;
        }
    }
    void EmptyMeter(){
        if(fillTween.active){fillTween.Kill();}
        emptyTween = DOTween.To(()=> slider.value, x => slider.value = x, 0.0f, 1.0f);
        if(slider.value > 0.001){
            canvasGroup.alpha = 1;
        }
        else if(slider.value <= 0.001){
            Debug.Log("lowering");
            gunManager.animator.SetBool("raise", false);
            gunManager.viewAnimator.SetBool("GunDrawn", false);
            gunManager.input.raise = false;
            //canvasGroup.alpha = 0;
            instructionsDraw.SetActive(true);
            instructionsShoot.SetActive(false);
            canvasGroup.DOFade(0.5f, 1f);

        }
    }
    [YarnCommand("actiavate_gun_reminder")]
    public void ActivateGunReminder()
    {
        gunPullAnim.SetBool("Draw_Reminder", true);
    }
    [YarnCommand("deactiavate_gun_reminder")]
    public void DeactivateGunReminder()
    {
        gunPullAnim.SetBool("Draw_Reminder", false);
    }

    /*
        - If I hold R, it fills the meter
            Once full, the gun comes out
        - If I let go, the meter goes down
            When it runs out, the gun goes down
    */
}
