using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : Character
{
    [SerializeField]
    private int currentHP = 100;
    [SerializeField]
    private Color flashColor = new Color(1f, 0f, 0f, 0.1f);
    [SerializeField]
    private Image damageImage;
    [SerializeField]
    private Text damageText;
    [SerializeField]
    private float flashSpeed = 5f;
    private bool ChangeClear = false;

    private void Start()
    {
        EventManager.StartListening("PLAYER_DAMAGE", Damage);
    }

    private void Update()
    {
        if(ChangeClear)
        {
            if(damageImage.color == Color.clear)
            {
                ChangeClear = false;
                return;
            }
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
    }

    private void Damage(EventParam eventParam)
    {
        currentHP -= eventParam.intParam;
        damageText.text = string.Format("{0}",currentHP);
        damageImage.color = flashColor;
        ChangeClear = true;
    }

    private void OnDestroy()
    {
        EventManager.StopListening("PLAYER_DAMAGE", Damage);
    }

    private void OnApplicationQuit()
    {
        EventManager.StopListening("PLAYER_DAMAGE", Damage);
    }
}
