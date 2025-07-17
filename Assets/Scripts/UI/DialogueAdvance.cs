using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.UI;

public class DialogueAdvance : MonoBehaviour
{
    public GameObject lineview;
    public bool isBridge = false;
    // Start is called before the first frame update
    public void Advance(Toggle toggle)
    {
        if (isBridge)
        {
            Br_2DLineView lw = lineview.GetComponent<Br_2DLineView>();
            lw.AutoAdvance = toggle.isOn;
        }
        else
        {
            Br_3DLineView lw = lineview.GetComponent<Br_3DLineView>();
            lw.autoAdvance = toggle.isOn;
        }
    }

}
