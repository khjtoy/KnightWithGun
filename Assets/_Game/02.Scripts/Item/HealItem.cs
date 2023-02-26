using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealItem : MonoBehaviour, Item
{

    private PlayerStats playerStats;

    private Animator playerAni;

    private int hashDrink = Animator.StringToHash("Drinking");

    private bool isPlaying = false;
    private void Start()
    {
        playerStats = GameObject.FindWithTag("PLAYER").GetComponent<PlayerStats>();
        playerAni = GameObject.FindWithTag("PLAYER").GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && playerStats.HpCount > 0 && !isPlaying)
        {
            isPlaying = true;
            playerStats.HpCount--;
            playerStats.ChangeItemUI();
            playerAni.SetTrigger(hashDrink);
            Invoke("ItemAction", 4);
            //ItemAction();
        }
    }

    public void ItemAction()
    {
        if (playerStats.CurrentHP < 100)
        {
            playerStats.CurrentHP += 10;
            if (playerStats.CurrentHP > 100) playerStats.CurrentHP = 100;
            playerStats.ChangeUI();
        }
        Kill();
    }

    public void Kill()
    {
        isPlaying = false;
    }
}
