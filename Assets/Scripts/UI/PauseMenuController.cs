using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject HUD;
    public GameObject DialogueDisplay;
    public GameObject PauseScreen;
    public GameObject Player;
    public GameObject Readme;
    public GameObject OptionScreen;
    private bool isPaused = false;
    private PlayerInput playerInput;

    [Header("Audio")]
    public AK.Wwise.State pausedState;
    public AK.Wwise.State unpausedState;
    public GameObject volSliderParent;
    public AK.Wwise.RTPC RTPC_MasterVolume;
    public AK.Wwise.RTPC RTPC_MusicVolume;
    public AK.Wwise.RTPC RTPC_SFXVolume;
    public AK.Wwise.RTPC RTPC_DialogueVolume;

    private Slider masterVolSlider;
    private Slider musicVolSlider;
    private Slider sFXVolSlider;
    private Slider dialogueVolSlider;

    // Update is called once per frame

    PauseAction action;

    private void Awake()
    {
        action = new PauseAction();
    }


    private void OnEnable()
    {
        action.Enable();
        GameManager.OnRestartGameEvent += Resume;
    }

    private void OnDisable()
    {
        action.Disable();
        GameManager.OnRestartGameEvent -= Resume;
    }

    private void Start()
    {

        Player = GameObject.FindGameObjectWithTag("Player");
        HUD = GameObject.Find("HUD Variant");

        action.Pause.PauseGame.performed += _ => DeterminePause();

        unpausedState.SetValue();
        masterVolSlider = volSliderParent.GetComponentsInChildren<Slider>()[0];
        musicVolSlider = volSliderParent.GetComponentsInChildren<Slider>()[1];
        sFXVolSlider = volSliderParent.GetComponentsInChildren<Slider>()[2];
        dialogueVolSlider = volSliderParent.GetComponentsInChildren<Slider>()[3];

        //override volume if already changed in different scene
        if (PlayerPrefs.GetFloat("MasterVol", -1) != -1)
        {
            masterVolSlider.value = PlayerPrefs.GetFloat("MasterVol", -1);
             RTPC_MasterVolume.SetGlobalValue(masterVolSlider.value);
        }

        if (PlayerPrefs.GetFloat("MusicVol", -1) != -1)
        {
            musicVolSlider.value = PlayerPrefs.GetFloat("MusicVol", -1);
            RTPC_MusicVolume.SetGlobalValue(musicVolSlider.value);
        }

        if (PlayerPrefs.GetFloat("SFXVol", -1) != -1)
        {
            sFXVolSlider.value = PlayerPrefs.GetFloat("SFXVol", -1);
            RTPC_SFXVolume.SetGlobalValue(sFXVolSlider.value);
        }

        if (PlayerPrefs.GetFloat("DialogueVol", -1) != -1)
        {
            dialogueVolSlider.value = PlayerPrefs.GetFloat("DialogueVol", -1);
            RTPC_DialogueVolume.SetGlobalValue(dialogueVolSlider.value);
        }
    }

    private void DeterminePause()
    {
        if (isPaused && OptionScreen.activeSelf)
        {
            OptionsChange();
        }
        else if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Pause()
    {
        isPaused = true;
        PauseScreen.SetActive(true);
        pausedState.SetValue();

        //DialogueDisplay.SetActive(false);
        HUD.SetActive(false);
        Time.timeScale = 0;
        Player.GetComponent<PlayerInput>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        isPaused = false;
        PauseScreen.SetActive(false);
        unpausedState.SetValue();

        //DialogueDisplay.SetActive(true);
        HUD.SetActive(true);
        Time.timeScale = 1;
        Player.GetComponent<PlayerInput>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ReadmeChange()
    {
        //Refresh Test
        if(Readme.activeSelf) Readme.SetActive(false);
        else Readme.SetActive(true);
        return;
    }

    public void OptionsChange()
    {
        if(OptionScreen.activeSelf){
            OptionScreen.SetActive(false);
            PauseScreen.SetActive(true);
        }
        else{
            OptionScreen.SetActive(true);
            PauseScreen.SetActive(false);
        } 
        return;
    }

    public bool GetPauseState()
    {
        return isPaused;
    }

    public void QuitGame(){
        Application.Quit();
    }

// Audio Sliderz
    public void UpdateMasterVolume(){
        RTPC_MasterVolume.SetGlobalValue(masterVolSlider.value);
        PlayerPrefs.SetFloat("MasterVol", masterVolSlider.value);
    }
    public void UpdateMusicVolume(){
        RTPC_MusicVolume.SetGlobalValue(musicVolSlider.value);
        PlayerPrefs.SetFloat("MusicVol", musicVolSlider.value);
    }
    public void UpdateSFXVolume(){
        RTPC_SFXVolume.SetGlobalValue(sFXVolSlider.value);
        PlayerPrefs.SetFloat("SFXVol", sFXVolSlider.value);
    }
    public void UpdateDialogueVolume(){
        RTPC_DialogueVolume.SetGlobalValue(dialogueVolSlider.value);
        PlayerPrefs.SetFloat("DialogueVol", dialogueVolSlider.value);
    }
}