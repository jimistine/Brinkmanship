using System.Collections;

using System.Collections.Generic;

using UnityEngine;



public class demo : MonoBehaviour

{

    // Start is called before the first frame update

    void Start()

    {

        AkSoundEngine.PostEvent("play_ambient_temp", WwiseManager.ins);

    }



    // Update is called once per fram

    void Update()

    {

        

    }

}

