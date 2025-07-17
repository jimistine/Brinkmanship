using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Yarn.Unity;


public class DeskComputerController : MonoBehaviour
{

    public VideoClip soldaciaClip;
    public VideoClip tradoClip;
    public VideoPlayer player;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [YarnCommand("Play_Soldacia_Clip")]
    public void PlaySoldaciaClip(){
        player.clip = soldaciaClip;
        player.playbackSpeed = 1.0f;
        AkSoundEngine.PostEvent("Play_Beeps_Soldacia", gameObject);
    }
    [YarnCommand("Play_Trado_Clip")]
    public void PlayTradoClip(){
        player.clip = tradoClip;
        player.Play();
        AkSoundEngine.PostEvent("Play_Beeps_Trado", gameObject);
    }
}
