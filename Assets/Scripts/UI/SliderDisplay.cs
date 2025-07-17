using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class SliderDisplay : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI slidertext;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        slidertext.text = slider.value.ToString();
    }
}
