using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterItem : MonoBehaviour, Item
{

    private PlayerStats playerStats;

    private Animator playerAni;

    private int hashDrink = Animator.StringToHash("Drinking");

    bool isPlaying = false;
    private void Start()
    {
        playerStats = GameObject.FindWithTag("PLAYER").GetComponent<PlayerStats>();
        playerAni = GameObject.FindWithTag("PLAYER").GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2) && playerStats.WaterCount > 0 && !isPlaying)
        {
            isPlaying = true;
            playerStats.WaterCount--;
            playerStats.ChangeItemUI();
            playerAni.SetTrigger(hashDrink);
            Invoke("ItemAction", 4);
            //ItemAction();
        }
    }

    public void ItemAction()
    {
        if (playerStats.CurrentThirst < 100)
        {
            playerStats.CurrentThirst += 10;
            if (playerStats.CurrentThirst > 100) playerStats.CurrentThirst = 100;
            playerStats.ChangeUI();
        }
        Kill();
    }

    public void Kill()
    {
        isPlaying = false;
    }
}
