using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioOneShot : MonoBehaviour
{
    public AK.Wwise.Event oneShotEvent;
    
    // Start is called before the first frame update
    void Start()
    {
        uint callbackType = (uint)(AkCallbackType.AK_Duration);
        AkSoundEngine.PostEvent(oneShotEvent.Id,gameObject,callbackType,EndCallback,null);
    }

    // Update is called once per frame
    public void EndCallback(object in_cookie, AkCallbackType in_type, object in_info)
    {
        if (in_type == AkCallbackType.AK_Duration)
        {
            AkDurationCallbackInfo info = (AkDurationCallbackInfo)(in_info);
            Destroy(gameObject,info.fDuration/1000);
        }
    }
}
