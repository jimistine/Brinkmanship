using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    private GameManager GM;

    // Start is called before the first frame update
    void Start()
    {
        GM = FindObjectOfType<GameManager>();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGameButton(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Starting game");
        GM.StartGame();
        AkSoundEngine.PostEvent("OnSelect", WwiseManager.ins);
        AkSoundEngine.PostEvent("Fade_Music", WwiseManager.ins);
    }
    public void QuitGameButton(){
        Application.Quit();
    }
}
