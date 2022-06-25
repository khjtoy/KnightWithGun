using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : Character
{
    [SerializeField]
    private int currentHP = 100;

    public int CurrentHP
    {
        get
        {
            return currentHP;
        }
        set
        {
            currentHP = value;
        }
    }
    [SerializeField]
    private int currentThirst = 100;
    
    public int CurrentThirst
    {
        get
        {
            return currentThirst;
        }
        set
        {
            currentThirst = value;
        }
    }

    [SerializeField]
    private float decreaseTime = 3f;
    private float timer = 0f;
    [SerializeField]
    private float thirstTimer = 0f;
    [SerializeField]
    private Color flashColor = new Color(1f, 0f, 0f, 0.1f);
    [SerializeField]
    private Color thirstColor = new Color(0.5f, 1f, 1f, 0.1f);
    [SerializeField]
    private Image damageImage;
    [SerializeField]
    private Text damageText;
    [SerializeField]
    private Text thirstText;
    [SerializeField]
    private float flashSpeed = 5f;
    [SerializeField]
    private float thirstSpeed = 5f;
    private bool ChangeClear = false;
    private bool changeThirst = false;
    //private bool tChangeClear = false;
    EventParam damageParam;

    [Header("Bottle")]
    [SerializeField]
    private Text hpText;
    [SerializeField]
    private Text waterText;
    [SerializeField]
    private Text hideText;

    private int hpCount = 0;
    private int waterCount = 0;
    private int hideCount = 0;

    private int thirstValue = 40;
    private int count = 2;
    public int HpCount
    {
        get
        {
            return hpCount;
        }
        set
        {
            hpCount = value;
        }
    }

    public int WaterCount
    {
        get
        {
            return waterCount;
        }
        set
        {
            waterCount = value;
        }
    }


    public int HideCount
    {
        get
        {
            return hideCount;
        }
        set
        {
            hideCount = value;
        }
    }


    private void Start()
    {
        EventManager.StartListening("PLAYER_DAMAGE", Damage);
        damageParam.intParam = 10;    
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
        
        //Show
        if(CurrentThirst <= 40 && !changeThirst)
        {
            damageImage.color = thirstColor;
            changeThirst = true;
        }
    }

    public void ShootGrenada()
    {
        EventManager.TriggerEvent("ShootGrenada", damageParam);
    }

    private void Damage(EventParam eventParam)
    {
        currentHP -= eventParam.intParam;
        damageText.text = string.Format("{0}",currentHP);
        damageImage.color = flashColor;
        ChangeClear = true;
    }

    public void ChangeUI()
    {
        damageText.text = string.Format("{0}", currentHP);
        thirstText.text = string.Format("{0}", currentThirst);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("ATTACK"))
        {
            Damage(damageParam);
        }
        if(other.CompareTag("ITEM"))
        {
            GetItem getItem = other.GetComponent<GetItem>();
            int number = getItem.GetNumber();
            if(number == 0)
            {
                hpCount++;
                ChangeItemUI();
            }
            else if(number == 1)
            {
                waterCount++;
                ChangeItemUI();
            }
            else if(number == 2)
            {
                hideCount++;
                ChangeItemUI();
            }
            other.gameObject.SetActive(false);
        }
    }

    public void ChangeItemUI()
    {
        hpText.text = string.Format("{0}", hpCount);
        waterText.text = string.Format("{0}", waterCount);
        hideText.text = string.Format("{0}", hideCount);
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
