using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameCutScene : MonoBehaviour
{
    [SerializeField]
    private PlayableDirector bossTimeline;

    [SerializeField]
    private PlayableDirector startTimeline;

    [SerializeField]
    private List<GameObject> bossOn;

    [SerializeField]
    private List<MonoBehaviour> offPlayerScript;

    [SerializeField]
    private GameObject bossObject;


    private Transform mainCamera;

    [SerializeField]
    private Image FadeImage;

    private bool OnBoss = false;

    private BossCtrl useBossCtrl;
    //private Camera

    [SerializeField]
    private Image panel;
    [SerializeField]
    private Text panelText;

    private void Start()
    {
        mainCamera = Camera.main.transform;
        OnStartCutScene();
    }

    private void Update()
    {
        if ((bossTimeline.state != PlayState.Playing) && OnBoss)
        {
            OnBoss = false;
            OffBossCutScene();
            useBossCtrl.ChangeDie();
            panel.DOFade(1, 0.5f).OnComplete(() =>
            {
                panelText.DOText("MISSION COMPLETE!!", 2f).OnComplete(() =>
                {
                    Invoke("LoadMenu", 2f);
                });
            });
        }

        if(startTimeline.state != PlayState.Playing)
        {
            OffStartCutScene();
        }
    }
    
    private void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void OnBossCutScene(BossCtrl bossCtrl)
    {
        useBossCtrl = bossCtrl;

        for(int i = 0; i < bossOn.Count; i++)
        {
            bossOn[i].SetActive(true);
        }

        for (int i = 0; i < offPlayerScript.Count; i++)
        {
            offPlayerScript[i].enabled = false;
        }

        bossObject.SetActive(false);
        mainCamera.transform.localPosition = new Vector3(0, 0, 0);

        FadeImage.DOFade(1, 0f).OnComplete(() =>
        {
            FadeImage.DOFade(0, 1f);
            OnBoss = true;
            bossTimeline.Play();
        });

    }

    public void OffBossCutScene()
    {

        for (int i = 0; i < bossOn.Count; i++)
        {
            bossOn[i].SetActive(false);
        }

        for (int i = 0; i < offPlayerScript.Count; i++)
        {
            offPlayerScript[i].enabled = true;
        }

        bossObject.transform.localPosition = bossOn[1].transform.localPosition;
        bossObject.SetActive(true);

        FadeImage.DOFade(1, 0f).OnComplete(() =>
        {
            FadeImage.DOFade(0, 1f);
        });
    }

    private void OffStartCutScene()
    {
        for (int i = 0; i < offPlayerScript.Count; i++)
        {
            offPlayerScript[i].enabled = true;
        }
    }

    private void OnStartCutScene()
    {
        for (int i = 0; i < offPlayerScript.Count; i++)
        {
            offPlayerScript[i].enabled = false;
        }
        mainCamera.transform.localPosition = new Vector3(0, 0, 0);
        startTimeline.Play();
    }
}
