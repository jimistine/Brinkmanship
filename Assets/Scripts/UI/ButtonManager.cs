using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ButtonManager : MonoBehaviour
{
    private Animator textAnim;
    // Start is called before the first frame update
    void Start()
    {
        textAnim = gameObject.GetComponentInChildren<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnHover(){
        textAnim.SetBool("Hover", true);
        AkSoundEngine.PostEvent("Play_Ui_2_Hover_b", gameObject);
    }
    public void OnHoverExit(){
        textAnim.SetBool("Hover", false);
        Debug.Log("Exit");

    }
}
