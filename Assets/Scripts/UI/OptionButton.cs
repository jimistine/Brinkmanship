using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionButton : MonoBehaviour
{
    public GameObject HideScreen;
    public GameObject ShowScreen;
   
   public void SwapScreen()
   {
    HideScreen.SetActive(false);
    ShowScreen.SetActive(true);
   }

}
