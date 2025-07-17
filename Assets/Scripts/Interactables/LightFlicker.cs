using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{

    Light flickerLight;
    public bool lightFlickering;
    public float maxOn;
    public float maxOff;
    public float maxIntensity;
    public float minIntensity;
    float interval = 1;
    float brightnessInterval = 1;
    float timer;
    float brightnessTimer;
    
    // Start is called before the first frame update
    void Start()
    {
        flickerLight = GetComponent<Light>();
    }


    void Update()
    {
        if(!DialogueManager.Instance.isDialogueRunning){
            brightnessTimer += Time.deltaTime;
            timer += Time.deltaTime;
            if (timer > interval){
                ToggleLight();
            }
            if (brightnessTimer > brightnessInterval){
                flickerLight.GetComponent<HDAdditionalLightData>().intensity = Random.Range(minIntensity, maxIntensity);
                brightnessInterval = Random.Range(0f, 2f);
                brightnessTimer = 0;
            }
        }
        else{
            flickerLight.enabled = true;
        }
    }
    
    void ToggleLight()
    {
        flickerLight.enabled = !flickerLight.enabled;
        if (flickerLight.enabled){
            interval = Random.Range(0, maxOn);
        }
        else {
            interval = Random.Range(0, maxOff);
        }
        timer = 0;
    }
}
