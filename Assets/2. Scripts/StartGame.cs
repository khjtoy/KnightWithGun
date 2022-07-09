using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    [SerializeField]
    private PlayableDirector cutscene;

    private bool isEnd = false;

    [SerializeField]
    private Image titleImg;
    [SerializeField]
    private Text[] titleText = new Text[2];

    private void Start()
    {
        
        //PlayerPrefs.SetInt("END", 0);
    }

    private void Update()
    {
        if((cutscene.state != PlayState.Playing) && isEnd == false || PlayerPrefs.GetInt("END", 0) == 1)
        {
            isEnd = true;
            PlayerPrefs.SetInt("END", 1);
            ShowTitle();
            EndCutScene();
        }

        if(isEnd)
        {
            if(Input.anyKeyDown)
            {
                SceneManager.LoadScene("InGame");
            }
        }

    }

    [SerializeField]
    private Image panel;

    public void EndCutScene()
    {
        panel.DOFade(1, 2f);
    }
    public void ShowTitle()
    {
        titleImg.DOFade(1, 1f);
        titleText[0].DOFade(1, 1f);
        titleText[1].DOFade(1, 1f);
    }
}
