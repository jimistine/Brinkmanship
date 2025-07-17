using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityPhysics;
using UnityEngine;

public class BulletUIManagerComponent : MonoBehaviour
{
    public List<GameObject> bullets = new List<GameObject>();
    public float hideDis = 100f;
    public float animationSpeed = 500f;
    public float animationDelay = 0.3f;
    public GameObject player;
    public PlayerManagerComponent playerManagerComponent;
    private float showYPos;
    private float hideYPos;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Start is called before the first frame update
    void Start()
    {
        playerManagerComponent = player.GetComponent<PlayerManagerComponent>();
        RectTransform firstBulletRT = bullets[0].GetComponent<RectTransform>();
        showYPos = firstBulletRT.localPosition.y;
        hideYPos = showYPos - hideDis;
        foreach (GameObject bullet in bullets) {
            RectTransform rt = bullet.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(rt.localPosition.x, hideYPos, rt.localPosition.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        ShowBulletCheck();
    }

    private void ShowBulletCheck()
    {

        GunState gunState = GameObject.Find("Gun").GetComponent<GunManagerComponent>().gunState;
        bool isGunRaisedOrAimed = gunState == GunState.Raised || gunState == GunState.Aiming || gunState == GunState.Firing;
        Debug.Log("ShowBulletCheck: " + gunState);
        
        if(isGunRaisedOrAimed)
        {
            StartCoroutine(ShowRemainingBullet(animationDelay));
            BulletCheck();
        } else {
            StartCoroutine(HideRemainingBullet(animationDelay));
        }
    } 

    public void BulletCheck()
    {
        Debug.Log("BulletCheck");
        foreach (GameObject bullet in bullets) { bullet.SetActive(false); }
        for (int i = 0; i < playerManagerComponent.bulletCount; i ++) {
            bullets[i].SetActive(true);
        }
    }

    public IEnumerator ShowRemainingBullet(float delay)
    {
        for (int i = 0; i < playerManagerComponent.bulletCount; i ++) {
            ShowBullet(bullets[i]);
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator HideRemainingBullet(float delay)
    {
        for (int i = 0; i < playerManagerComponent.bulletCount; i ++) {
            HideBullet(bullets[i]);
            yield return new WaitForSeconds(delay);
        }
    }

    private void ShowBullet(GameObject bullet)
    {
        float step = animationSpeed * Time.deltaTime;
        RectTransform bulletRT = bullet.GetComponent<RectTransform>();
        Vector3 currentPos = bulletRT.localPosition;
        Vector3 targetPos = new Vector3(currentPos.x, showYPos, currentPos.z);

        bulletRT.localPosition = Vector3.MoveTowards(currentPos, targetPos, step);
    }

    private void HideBullet(GameObject bullet)
    {
        float step = animationSpeed * Time.deltaTime;
        RectTransform bulletRT = bullet.GetComponent<RectTransform>();
        Vector3 currentPos = bulletRT.localPosition;
        Vector3 targetPos = new Vector3(currentPos.x, hideYPos, currentPos.z);

        bulletRT.localPosition = Vector3.MoveTowards(currentPos, targetPos, step);
    }

}
