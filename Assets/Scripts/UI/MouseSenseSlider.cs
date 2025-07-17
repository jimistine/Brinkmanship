using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MouseSenseSlider : MonoBehaviour
{
    public Slider slider;
    public GameObject player;
    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        FirstPersonController control = player.GetComponent<FirstPersonController>();
        if(SystemInfo.operatingSystem.Contains("Mac")){
            control.RotationSpeed = 4;
            slider.value = 4;
        }

       if (PlayerPrefs.GetFloat("MouseSensativity", -1) != -1)
       {
            control.RotationSpeed = PlayerPrefs.GetFloat("MouseSensativity", -1);
            slider.value = PlayerPrefs.GetFloat("MouseSensativity", -1);
       }
       else
       {
            control.RotationSpeed = 3;
            slider.value = 3;
       }

    }

    // Update is called once per frame
    void Update()
    {
        FirstPersonController control = player.GetComponent<FirstPersonController>();
        control.RotationSpeed = slider.value;
        PlayerPrefs.SetFloat("MouseSensativity", control.RotationSpeed);
    }
}
