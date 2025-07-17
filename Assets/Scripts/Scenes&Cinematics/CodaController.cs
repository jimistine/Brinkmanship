using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Unity.VisualScripting;
using BehaviorDesigner.Runtime.Tasks.Unity.Timeline;

public class CodaController : MonoBehaviour
{
    public GameObject oneYearLater;
    public int codaFrame = 0;
    public TextMeshProUGUI codaText;
    public CanvasGroup codaCG;
    public GameObject endButtons;
    public GameObject skipButton;
    public bool codaSkipped;
    public GameObject codaAudio;
    public float fadeInTime;
    public float fadeOutTime;
    [System.Serializable]
    public struct EndText{
        public string condition;
        public float timeToRead;
        [TextArea(3,10)]
        public string text;
    }
    public List<EndText> endTextsMaster = new List<EndText>();
    public List<EndText> endTextsShown = new List<EndText>();
    [Header("End Conditions")]
    public bool turnedShip;
    private bool uglyEnding;
    public string gameEndingReached;
    public string capStatus;
    public string pilotStatus;
    public string prisonerStatus;
    // Start is called before the first frame update
    void Start()
    {
        codaCG.alpha = 0;
        InitCoda();
    }
    private void InitCoda(){
        codaSkipped = false;
  // Ship Turned
        ProgressManager.Instance.storedVariables.TryGetValue("turnComplete", out string turnStatus);
        ProgressManager.Instance.storedVariables.TryGetValue("gameEnding", out string gameEndingReached);
        if(gameEndingReached == "ugly"){
            Debug.Log("Ugly Ending Coda");
            uglyEnding = true;
            endTextsShown.Add(endTextsMaster[2]);
            StartCoroutine(BeginCoda());
            return;
        }
        else if(turnStatus == "true"){
            turnedShip = true;
            endTextsShown.Add(endTextsMaster[0]);
        }
        else{
            turnedShip = false;
            endTextsShown.Add(endTextsMaster[1]);
        }
    // Captain Status
        ProgressManager.Instance.storedVariables.TryGetValue("captain", out string capLivingStatus);
        ProgressManager.Instance.storedVariables.TryGetValue("capPayout", out string capPayoutStatus);
        ProgressManager.Instance.storedVariables.TryGetValue("capBlackmailSoldacia", out string capBlackmailSoldaciaStatus);
        if(capLivingStatus == "living"){
            // Cap is alive and you turned the ship
            if(turnedShip){
                capStatus = "captain alive soldacia";
            }
            // Cap is alive and you did not turn the ship
            else{
                capStatus = "captain alive trado";
            }
            EndText textToAdd = endTextsMaster.Find( EndText => EndText.condition == capStatus);
            endTextsShown.Add(textToAdd);
            
            // Cap is alive and you got paid to stay quiet while going to trado
            if(capPayoutStatus == "true"){
                capStatus = "captain blackmail payout";
                textToAdd = endTextsMaster.Find( EndText => EndText.condition == capStatus);
                endTextsShown.Add(textToAdd);
            }
            // Cap is alive and you blackmail him into turning the ship
            else if(capBlackmailSoldaciaStatus == "true"){
                capStatus = "captain blackmail soldacia";
                textToAdd = endTextsMaster.Find( EndText => EndText.condition == capStatus);
                endTextsShown.Add(textToAdd);
            }
        }
        else{
            // Cap is dead and you turned the ship
            if(turnedShip){
                capStatus = "captain dead soldacia";
            }
            // Cap is dead and you did not turn the ship
            else{
                capStatus = "captain dead trado";
            }
            EndText textToAdd = endTextsMaster.Find( EndText => EndText.condition == capStatus);
            endTextsShown.Add(textToAdd);
        }

    // Pilot Status
        ProgressManager.Instance.storedVariables.TryGetValue("pilot", out string pilotLivingStatus);
        ProgressManager.Instance.storedVariables.TryGetValue("pilotPayout", out string pilotPayoutStatus);
        ProgressManager.Instance.storedVariables.TryGetValue("pilotBlackmailSoldacia", out string pilotBlackmailSoldaciaStatus);
        if(pilotLivingStatus == "living"){
            // Garm is alive and you turned the ship
            if(turnedShip){
                pilotStatus = "pilot alive soldacia";
            }
            // Garm is alive and you did not turn the ship
            else{
                pilotStatus = "pilot alive trado";
            }
            
            // Garm is alive and you got paid to stay quiet while going to trado
            if(pilotPayoutStatus == "true"){
                pilotStatus = "pilot blackmail payout";
            }
            // Garm is alive and you blackmailed him into turning the ship
            else if(pilotBlackmailSoldaciaStatus == "true"){
                pilotStatus = "pilot blackmail soldacia";
            }

            EndText textToAdd = endTextsMaster.Find( EndText => EndText.condition == pilotStatus);
            endTextsShown.Add(textToAdd);
        }
        else{
            // Garm is dead and you turned the ship
            if(turnedShip){
                pilotStatus = "pilot dead soldacia";
            }
            // Garm is dead and you did not turn the ship
            else{
                pilotStatus = "pilot dead trado";
            }
            EndText textToAdd = endTextsMaster.Find( EndText => EndText.condition == pilotStatus);
            endTextsShown.Add(textToAdd);
        }
    // Prisoner Status
        ProgressManager.Instance.storedVariables.TryGetValue("captain_punish_prisoner_intel", out string prisonerMetStatus);
        if(prisonerMetStatus == "true"){
            if(turnedShip){
                prisonerStatus = "prisoner soldacia";
            }
            else{
                prisonerStatus = "prisoner trado";
            }
            EndText textToAdd = endTextsMaster.Find( EndText => EndText.condition == prisonerStatus);
            endTextsShown.Add(textToAdd);
        }
    // Start the Sequence
        StartCoroutine(BeginCoda());
    }
    public IEnumerator BeginCoda(){
        codaCG.gameObject.SetActive(true);
        if(uglyEnding == true){
            //brtodo: ugly coda audio goes here
            yield return new WaitForSeconds(4.0f);
            StartCoroutine(CodaTextSequence());
            yield break;
        }
        else if(turnedShip){
            AkSoundEngine.PostEvent("play_amb_coda_soldacia", codaAudio);
        }
        else{
            AkSoundEngine.PostEvent("play_amb_coda_trado", codaAudio);
        }
        
        if(gameEndingReached != "ugly"){
            Debug.Log("game ending reached" + gameEndingReached);
            yield return new WaitForSeconds(3f);
            AkSoundEngine.PostEvent("Coda_Hit", codaAudio);

            oneYearLater.SetActive(true);

            yield return new WaitForSeconds(5f);
            oneYearLater.SetActive(false);

            yield return new WaitForSeconds(2.5f);
            StartCoroutine(CodaTextSequence());
            yield return new WaitForSeconds(1.5f);
            skipButton.GetComponent<CanvasGroup>().DOFade(1, 3);
        }
    }
    public void SkipCoda(){
        codaSkipped = true;
        StopCoroutine(CodaTextSequence());
        codaCG.DOFade(0, 1.0f).OnComplete(DisableCodaTextObj);
        endButtons.SetActive(true);
        endButtons.GetComponent<CanvasGroup>().DOFade(1, 4);
        skipButton.GetComponent<CanvasGroup>().DOFade(0, 2);
    }
    public void DisableCodaTextObj(){
        codaCG.gameObject.SetActive(false);
    }
    public IEnumerator CodaTextSequence(){
        // if(Input.GetKeyDown(KeyCode.Space)){
        //     codaFrame++;
        //     StopCoroutine(CodaTextSequence());
        //     StartCoroutine(CodaTextSequence());
        // }
        if(!codaSkipped){
            UpdateCodaText();
            codaCG.DOFade(1, fadeInTime);
            Debug.Log("End Text: " + endTextsShown[codaFrame].condition + "waiting: " + endTextsShown[codaFrame].timeToRead);
            yield return new WaitForSeconds(endTextsShown[codaFrame].timeToRead);
            codaCG.DOFade(0, fadeOutTime);
            yield return new WaitForSeconds(2.5f);
            if(codaFrame == endTextsShown.Count - 1){
                // end the game
                Debug.Log("Ending game for real");
                endButtons.SetActive(true);
                endButtons.GetComponent<CanvasGroup>().DOFade(1, 4);
                skipButton.GetComponent<CanvasGroup>().DOFade(0, 2);
            }
            else{
                codaFrame++;
                StartCoroutine(CodaTextSequence());
            }
        }
    }

    public void UpdateCodaText(){
        codaText.text = endTextsShown[codaFrame].text;
    }
}
