using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EndGameOptions : MonoBehaviour
{

    public GameObject creditsObj;
    public GameObject creditsEndTarget;
    public GameObject endButtons;
    public GameObject postCreditsButtons;
    public float creditsDuration;
    public GameObject codaAudio;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void RestartBridge_End(){
        GameManager.GM.RestartBridge_End();
    }
    public void RestartGmae_End(){
        GameManager.GM.RestartGame();
    }
    public void QuitGame_End(){
        GameManager.GM.QuitGame();
    }
    public void RollCredits(){

        endButtons.GetComponent<CanvasGroup>().DOFade(0, 1.5f).OnComplete(DisableButtons);

    }
    void DisableButtons(){
        endButtons.SetActive(false);
    // Start Roll process
        AkSoundEngine.PostEvent("stop_amb_coda_trado", codaAudio);
        AkSoundEngine.PostEvent("stop_amb_coda_soldacia", codaAudio);
        AkSoundEngine.PostEvent("Play_MM_Music", codaAudio);
        creditsObj.SetActive(true);
        creditsObj.GetComponent<CanvasGroup>().DOFade(1, 4).OnComplete(BeginRoll);
    }
    void BeginRoll(){
        creditsObj.transform.DOMove(creditsEndTarget.transform.position, creditsDuration)
                                    .SetEase(Ease.Linear)
                                    .OnComplete(ShowPostCreditsButtons);
    }
    void ShowPostCreditsButtons(){
        postCreditsButtons.SetActive(true);
        postCreditsButtons.GetComponent<CanvasGroup>().DOFade(1, 3f);
    }
}
