using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Yarn.Unity;
using UnityEngine;

public class DoorEnterHandler : MonoBehaviour
{
    private GameManager gameManager;
    private GameObject player;
    private Animator doorsAnim;
    

    public delegate void OnLoadBridge();
    public static event OnLoadBridge onLoadBridge;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        doorsAnim = GetComponent<Animator>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(BeginAirlockSequence());
        }
    }
    public IEnumerator BeginAirlockSequence(){
        //Close doors, once closed, swap scenes
        GameObject parentObj = GameObject.Find("/PlayerNCamera");
        player = parentObj.transform.Find("Player").gameObject;
        doorsAnim.SetBool("Open_BridgeDoors", false);
        yield return new WaitForSeconds(2.0f);
        StartCoroutine(ChangeScene(player));
    }

    

    IEnumerator ChangeScene(GameObject player)
    {
        onLoadBridge?.Invoke();
        SceneManager.LoadScene("03_Bridge", LoadSceneMode.Additive);

        yield return new WaitForSeconds(0.5f);

        GameObject parentDia = GameObject.FindGameObjectWithTag("DialogueSystem");
        if (parentDia == null) { Debug.LogWarning("Cannot find the Dialogue!!!!!!!!!!!"); }
        else
        {
            DialogueInputManager dialogueInputManager = player.GetComponent<DialogueInputManager>();
            dialogueInputManager.optionsListView = parentDia.GetComponentInChildren<Br_OptionsListView>();
            dialogueInputManager.DR = parentDia.GetComponent<DialogueRunner>();
            dialogueInputManager.lineView = parentDia.GetComponentInChildren<Br_2DLineView>();
            dialogueInputManager.voidLineView = null;
        }

        SceneManager.UnloadSceneAsync("02_Void");

        GameManager.GM.DisableGun();

        // yield return new WaitForSeconds(1.5f);
        // DialogueManager.InvokeYSEvent("3-1_Airlock", null);

    }
    

//     private void OpenDoor()
//     {
//         Debug.Log("The Player has entered the door!!!");

//         GameObject anchor = GameObject.FindGameObjectWithTag("Anchor");

//         
//         CharacterController characterController = player.GetComponent<CharacterController>();
//         GameObject mainCamera = parentObj.transform.Find("MainCamera").gameObject;
//         GameObject playerFollowCamera = parentObj.transform.Find("PlayerFollowCamera").gameObject;
        
//         //Rigidbody rb = player.GetComponentInChildren<Rigidbody>();
//         //Vector3 playerV = rb.velocity;
//         Quaternion playerRot = player.transform.localRotation;
//         Quaternion mainCamRot = mainCamera.transform.localRotation;
//         Quaternion followCamRot = playerFollowCamera.transform.localRotation;
        
//         characterController.enabled = false;
        
//         //The teleport
//         player.transform.position = new Vector3(anchor.transform.position.x, anchor.transform.position.y - 1.375f, anchor.transform.position.z);
//         player.transform.localRotation = Quaternion.Euler(playerRot.x, playerRot.y, playerRot.z);

//         mainCamera.transform.position = anchor.transform.position;
//         //mainCamera.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
//         //mainCamera.transform.localRotation = Quaternion.Euler(mainCamRot.x, mainCamRot.y - 90, mainCamRot.z);

//         playerFollowCamera.transform.position = anchor.transform.position;
//         //playerFollowCamera.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
//        // playerFollowCamera.transform.localRotation = Quaternion.Euler(followCamRot.x, followCamRot.y, followCamRot.z);

//         characterController.enabled = true;
//         //rb.velocity = playerV;

//         StartCoroutine(BeginAirlockSequence());

//         //StartCoroutine(ChangeScene(player));

// /*        //gameManager.UpdateScenes("02_Void", "03_Bridge");
//         SceneManager.LoadScene("03_Bridge", LoadSceneMode.Additive);

//         GameObject parentDia = GameObject.FindGameObjectWithTag("DialogueSystem");
//         //GameObject parentDia = GameObject.Find("/Br Dialogue System");
//         if (parentDia == null) { Debug.Log("Cannot find the Dialogue!!!!!!!!!!!"); }
//         else
//         {
//             DialogueInputManager dialogueInputManager = player.GetComponent<DialogueInputManager>();
//             dialogueInputManager.optionsListView = parentDia.GetComponent<Br_OptionsListView>();
//             dialogueInputManager.DR = parentDia.GetComponent<DialogueRunner>();
//             dialogueInputManager.lineView = parentDia.GetComponent<Br_2DLineView>();
//             dialogueInputManager.voidLineView = null;
//         }

//         SceneManager.UnloadSceneAsync("02_Void");*/
//     }

    
}
