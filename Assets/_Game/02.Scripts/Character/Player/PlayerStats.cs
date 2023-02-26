using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

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


    EventParam damageParam;
    EventParam bossParam;
    EventParam dogParam;

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

    [SerializeField]
    private Transform cameraShake;
    [SerializeField]
    private WeaponGrenade weaponGrenade;

    [SerializeField]
    private Image panel;

    private bool isDied = false;
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

    private readonly int hashDie = Animator.StringToHash("Die");

    public bool GetThirst()
    {
        return changeThirst;
    }


    private void Start()
    {
        EventManager.StartListening("PLAYER_DAMAGE", Damage);
        damageParam.intParam = 10;
        dogParam.intParam = 15;
        bossParam.intParam = 30;
    }


    private void Update()
    {
        // 40% ���� �뽬 �Ұ�  10% ������
        timer += Time.deltaTime;

        if(timer >= decreaseTime)
        {
            if (currentThirst <= 0) return;
            currentThirst -= 2;
            if (currentThirst < 0) currentThirst = 0;
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
        
        if(currentThirst <= 40)
        {
            changeThirst = true;
        }
        else if(currentThirst > 40 && changeThirst)
        {
            changeThirst = false;
        }
    }

    public void ShootGrenada()
    {
        EventManager.TriggerEvent("ShootGrenada", damageParam);
    }

    private void Damage(EventParam eventParam)
    {
        currentHP -= eventParam.intParam;

        if(currentHP <= 0 && !isDied)
        {
            isDied = true;
            ani.SetTrigger(hashDie);
            panel.DOFade(1, 0.5f).OnComplete(() =>
            {
                panel.gameObject.transform.GetChild(0).GetComponent<Text>().DOFade(1, 1f);
                Invoke("LoadScene", 3f);
            });
        }

        cameraShake.DOShakePosition(0.3f, 4);
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

        if(other.CompareTag("BOSSATTACK"))
        {
            Damage(bossParam);
            other.enabled = false;
        }

        if(other.CompareTag("DOGATTACK"))
        {
            Damage(dogParam);
            other.enabled = false;
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

        if(other.CompareTag("PICKUPGRENADE"))
        {
            weaponGrenade.AddGrenada();
            other.gameObject.SetActive(false);
        }
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(1);
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
