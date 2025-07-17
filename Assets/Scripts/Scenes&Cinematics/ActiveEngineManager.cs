using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using BehaviorDesigner.Runtime.Tasks.Unity.Math;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

public class ActiveEngineManager : MonoBehaviour
{
    public static ActiveEngineManager ins = null;
    private Coroutine CreakGenerator = null;
    private float intensity = 100.0f;
    [SerializeField] private GameObject CreakOneShot;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (ins == null)
        {
            ins = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void StartEngines()
    {
        if(ins != null)
            ins._StartEngines();
    }

    public static void EndEngines()
    {
        if (ins != null)
            ins._EndEngines();
    }

    public static void SetIntensity(float f)
    {
        AkSoundEngine.SetRTPCValue("EngineIntensity", f);
        if(ins != null)
            ins.intensity = f;
    }

    private void _StartEngines()
    {
        AkSoundEngine.PostEvent("ActiveEngine_Start", gameObject);
        if (CreakGenerator == null)
        {
            CreakGenerator = StartCoroutine(CreakRoutine());
        }
    }

    private void _EndEngines()
    {
        AkSoundEngine.PostEvent("ActiveEngine_End", gameObject);
        if (CreakGenerator != null)
        {
            StopCoroutine(CreakGenerator);
            CreakGenerator = null;
        }
    }

    private IEnumerator CreakRoutine()
    {
        while (true)
        {
            float wait = UnityEngine.Random.Range(0.1f, 2.15f - (intensity / 100.0f) * 2.0f);
            yield return new WaitForSeconds(wait);
            //AkSoundEngine.PostEvent("ActiveEngine_Creak", gameObject);
            int i = UnityEngine.Random.Range(0, 4);
            for (int j = 0; j <= i; j++)
            {
                Vector3 Direction = UnityEngine.Random.insideUnitSphere * 20;
                Instantiate(CreakOneShot,transform.position+Direction, Quaternion.identity);
            }
        }
    }
}
