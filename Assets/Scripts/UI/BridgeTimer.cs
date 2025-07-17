using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BridgeTimer : MonoBehaviour
{
    public static float currentTime;
    public GameObject shipSpeaker;
    private TMP_Text timerText;
    private GameManager GM;
    private bool triggered_2m;
    private bool triggered_3m;
    private bool triggered_turning;
    private bool triggered_failure;
    private bool triggered60s;
    private bool triggered30s;
    private bool triggered10s;
  
    

    // Start is called before the first frame update
    void Start()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        currentTime = GM.timeOnBridge;
        timerText = GetComponent<TMP_Text>();
        shipSpeaker = timerText.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentTime > 0){
            if(currentTime < 180){
                if (!triggered_3m){
                    //timerText.color = Color.yellow;
                    AkSoundEngine.PostEvent("Play_countdown_3m", shipSpeaker);
                    triggered_3m = true;
                }
            }   
            if(currentTime < 120){
                if (!triggered_2m){
                    //timerText.color = Color.yellow;
                    AkSoundEngine.PostEvent("Play_countdown_2m", shipSpeaker);
                    triggered_2m = true;
                }
            }   
            if(currentTime < 60){
                if (!triggered60s){
                    timerText.color = Color.yellow;
                    AkSoundEngine.PostEvent("Play_countdown_60s", shipSpeaker);
                    triggered60s = true;
                }
            }   
            if(currentTime < 30){
                if(!triggered30s){
                    //timerText.color = Color.;
                    AkSoundEngine.PostEvent("Play_countdown_30s", shipSpeaker);
                    triggered30s = true;
                }
            }
            if(currentTime < 13.5f){
                if(!triggered10s){
                    timerText.color = Color.red;
                    AkSoundEngine.PostEvent("Play_countdown_10s", shipSpeaker);
                    triggered10s = true;
                }
            }
            currentTime -= Time.deltaTime;
            TimeSpan time = TimeSpan.FromSeconds(currentTime);
            timerText.text = "<size=6>T-minus</size>\n00:" + time .ToString(@"mm\:ss") + "\n<size=6>to trajectory lock";
        }
        else{
            timerText.text = "Trajectory locked\nGravity assist maneuver in progress";
        }
    }
}
