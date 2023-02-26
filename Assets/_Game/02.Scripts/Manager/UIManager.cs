using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject panel;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(panel.activeSelf)
            {
                ClickContinue();
            }
            else
            {
                panel.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0;
            }
        }
    }

    public void ClickContinue()
    {
        panel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
    }

    public void ClickExit()
    {
        Application.Quit();
    }
}
