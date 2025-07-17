using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayPing(){
        AkSoundEngine.PostEvent("Play_ailyn_ping", gameObject);
    }
}
