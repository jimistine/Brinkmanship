using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public void PostMusicEvent(string eventName){
        AkSoundEngine.PostEvent(eventName, gameObject);
    }

    void Update(){
        // Music Testing
        
    }
}
