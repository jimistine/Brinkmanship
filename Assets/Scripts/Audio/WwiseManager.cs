using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class WwiseManager : MonoBehaviour

{
    public static GameObject ins = null;
    void Awake()
    {
        if (ins == null)
        {
            ins = gameObject;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable(){
        GameManager.OnRestartGameEvent += StopAllAudio;
    }
    private void OnDisable(){
        GameManager.OnRestartGameEvent -= StopAllAudio;
    }

    private void StopAllAudio(){
        AkSoundEngine.StopAll();
    }
}

