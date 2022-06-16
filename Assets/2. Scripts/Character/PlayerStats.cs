using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : Character
{
    [SerializeField]
    private int currentHP = 100;
    [SerializeField]
    private int currentThirst = 100;
    [SerializeField]
    private float decreaseTime = 3f;
    private float timer = 0f;
    [SerializeField]
    private Color flashColor = new Color(1f, 0f, 0f, 0.1f);
    [SerializeField]
    private Image damageImage;
    [SerializeField]
    private Text damageText;
    [SerializeField]
    private Text thirstText;
    [SerializeField]
    private float flashSpeed = 5f;
    private bool ChangeClear = false;
    EventParam damageParam;
    private void Start()
    {
        EventManager.StartListening("PLAYER_DAMAGE", Damage);
        damageParam.intParam = 5;    
    }


    private void Update()
    {
        // 40% 이하 속도가 느려짐 20% 이하 스킬 사용 불가 10% 데미지
        timer += Time.deltaTime;

        if(timer >= decreaseTime)
        {
            currentThirst -= 2;
            thirstText.text = string.Format("{0}", currentThirst);
            timer = 0;
        }

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

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("ATTACK"))
        {
            Damage(damageParam);
        }
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
