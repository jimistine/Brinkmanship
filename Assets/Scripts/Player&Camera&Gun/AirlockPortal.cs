using UnityEngine;

public class AirlockPortal : MonoBehaviour
{
    [SerializeField] Transform playerCam;
    [SerializeField] Transform thisPortal;
    [SerializeField] Transform otherPortal;


    private void LateUpdate()
    {

        Vector3 cameraPos = otherPortal.InverseTransformPoint(playerCam.position);
        Vector3 cameraFor = otherPortal.InverseTransformDirection(playerCam.forward);
        cameraPos = thisPortal.TransformPoint(cameraPos);
        cameraFor = thisPortal.TransformDirection(cameraFor);
        this.transform.position = cameraPos;
        this.transform.LookAt(cameraFor + cameraPos);

    }
}
