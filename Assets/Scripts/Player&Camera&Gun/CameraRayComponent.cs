using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRayComponent : MonoBehaviour
{
    public GameObject reticle_aim;
    public GameObject reticle_interactive;
    public GameObject reticle_normal;
    public PlayerInputs playerInputs;
    public float rayLength;
    private GameObject player;
    private PlayerManagerComponent playerManagerComponent;
    private InputAction fire;
    private InputAction interact;

    /// <summary>
    /// This delegate is triggered when the player start and stopped aiming at an interactable object. 
    /// </summary>
    /// <param name="start"> True if the player started aiming at an interactable object, false if the player stopped aiming at an interactable object. </param>
    public delegate void OnInteractable(bool start, string label);
    public static event OnInteractable OnInteractableEvent;
    private string _interactableLabel = "Interact";

    public string interactableLabel {
        get {
            return _interactableLabel;
        }

        set {
            // Update only if the value is different.
            if (value != _interactableLabel) {
                OnInteractableEvent?.Invoke(isInteractable, value);
                _interactableLabel = value;
            }
        }
    }

    public delegate void OnInteractedWithItem(GameObject item);
    public static event OnInteractedWithItem OnInteractedWithItemEvent;

    private bool _isInteractable = false;

    public bool isInteractable {
        get {
            return _isInteractable;
        }

        set {
            // Update only if the value is different.
            if (value != _isInteractable) {
                OnInteractableEvent?.Invoke(value, interactableLabel);
                _isInteractable = value;
            }
        }
    }
    
    [SerializeField] public bool allowFireButtonToTriggerInteract = true;

    private GunState gunState {
        get {
            return playerManagerComponent.gunManagerComponent.gunState;
        }
    }
    private bool isGunRaisedOrAimed {
        get {
            return gunState == GunState.Raised || gunState == GunState.Aiming;
        }
    }
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerInputs = new PlayerInputs();
    }

    void OnEnable()
    {
        fire = playerInputs.Player.Fire;
        interact = playerInputs.Player.Interact;

        fire.Enable();
        interact.Enable();

        fire.performed += OnFire;
        interact.performed += OnInteract;
    }

    void OnDisable()
    {
        fire.Disable();
        interact.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerManagerComponent = player.GetComponent<PlayerManagerComponent>();
        playerManagerComponent.cameraRayComponent = gameObject.GetComponent<CameraRayComponent>();
        ResetReticle();
    }

    // Update is called once per frame
    void Update()
    {
        ResetReticle();
        DetectIneractable();
        DetectAttackable();
        Debug.DrawRay(transform.position, transform.forward * rayLength, Color.red);
    }

    public void OnFire(InputAction.CallbackContext context)
	{
        if (allowFireButtonToTriggerInteract) {
            Interact(context.action);
        }
	}

    public void OnInteract(InputAction.CallbackContext context)
    {
        Interact(null);
    }

    private void DetectAttackable() {
        Ray ray =  Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hitData;
        if (Physics.Raycast(ray, out hitData)) 
        {
            GameObject hitObject = hitData.transform.gameObject;
            Attackable attackable = hitObject.GetComponent<Attackable>();

            // If the object is not attackable, return.
            if (attackable == null || !attackable.isAttackable) { return; }
            // If gun is not raised or aimed, return.
            if (!isGunRaisedOrAimed) { return; }

            ReticleAttack();

            attackable?.Aimed();
        }
    }

    private void DetectIneractable() {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hitData;
        if (Physics.Raycast(ray, out hitData, rayLength)) 
        {
            GameObject hitObject = hitData.transform.gameObject;
            Interactable interactable = hitObject.GetComponent<Interactable>();
            Attackable attackable = hitObject.GetComponent<Attackable>();
            Dialoguable dialoguable = hitObject.GetComponent<Dialoguable>();

            interactableLabel = "Interact";

            if (hitObject.tag == "UI") {
                Debug.Log("UI");
            }
            // If the object is not interactable, return.
            if (!interactable?.isInteractable ?? true) { 
                isInteractable = false;
                return; 
            }
            // if we want to ignore this object
            else if(hitData.collider.gameObject.tag == "Ignore"){
                interactableLabel = "";
                isInteractable = false;
                return;
            }

            if (dialoguable?.isDialoguable ?? false) {
                interactableLabel = "Talk";
            }
            else if (hitObject.tag == "Collectable") {
                interactableLabel = "Collect";
            }

            // If the gun is raised or aimed, return.
            // if (isGunRaisedOrAimed) { 
            //     // If the interactable object is can talk and player is using arm, show itimaidate
            //     if (attackable?.isAttackable ?? false) {
            //         interactableLabel = "Interact";
            //     }

            //     isInteractable = true;
            //     return; 
            // }

            isInteractable = true;

            ReticleInteractive();
            interactable?.Stared();
        }
        else {
            isInteractable = false;
        }
    }

    private void Interact(InputAction inputAction) {
        Ray ray =  Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hitData;

        if (Physics.Raycast(ray, out hitData, rayLength)) 
        {
            GameObject hitObject = hitData.transform.gameObject;
            Interactable interactable = hitObject.GetComponent<Interactable>();

            // If the object is not interactable, return.
            if (interactable == null || !interactable.isInteractable) { return; }

            // If the gun is raised or aimed and triggered action is fire, return.
            if ((inputAction == playerInputs.Player.Fire) && isGunRaisedOrAimed) { return; }

            // If we want to ignore a child of an interactible object
            if(hitData.collider.gameObject.tag == "Ignore"){ return; }

            if (hitObject.tag == "Collectable") {
                print("collected a new object");
                playerManagerComponent.AddToInventory(hitObject);
            }

            interactable?.Interact(isGunRaisedOrAimed);
            OnInteractedWithItemEvent?.Invoke(hitObject);
        } 
    }

    private void ReticleAttack() {
        reticle_aim.SetActive(true);
        reticle_interactive.SetActive(false);
        reticle_normal.SetActive(false);
    }

    private void ReticleInteractive() {
        reticle_aim.SetActive(false);
        reticle_interactive.SetActive(true);
        reticle_normal.SetActive(false);
    }

    private void ResetReticle() {
        reticle_aim.SetActive(false);
        reticle_interactive.SetActive(false);
        reticle_normal.SetActive(true);
    }
}