using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    [SerializeField] Camera portalCamera;
    [SerializeField] Material cameraMat;

    private void Start()
    {
        if(portalCamera.targetTexture != null)
        {
            portalCamera.targetTexture.Release();
        }
        portalCamera.targetTexture = new RenderTexture(Screen.width, Screen.height, 18);
        cameraMat.mainTexture = portalCamera.targetTexture;
    }
}
