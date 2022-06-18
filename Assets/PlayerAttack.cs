using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : Character
{
    //readonly int m_HashStateTime = Animator.StringToHash("StateTime");
    readonly int m_HashMeelAttack = Animator.StringToHash("MeelAttack");

    private bool isAttack;

    [SerializeField]
    private GameObject atkParicle;

    [SerializeField]
    private Collider gunCol;

    [SerializeField]
    private float delay;

    private float timer;
    public bool IsAttack
    {
        get
        {
            return isAttack;
        }
        set
        {
            isAttack = value;
        }
    }
    private void Update()
    {
        //ani.SetFloat(m_HashStateTime, Mathf.Repeat(ani.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f));


        if (timer <= 0)
        {
            if (Input.GetMouseButtonDown(0) && !GetComponent<ZoomAim>().isAim())
            {
                ani.SetTrigger(m_HashMeelAttack);
                atkParicle.SetActive(true);
                IsAttack = true;
                gunCol.enabled = true;
                timer = delay;
                rigid.velocity = Vector3.zero;
            }
        }
        else
            timer -= Time.deltaTime;
    }

    public void OffPaticle()
    {
        atkParicle.SetActive(false);
        gunCol.enabled = false;
    }
}
