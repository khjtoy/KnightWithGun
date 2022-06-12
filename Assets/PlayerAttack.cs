using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : Character
{
    readonly int m_HashStateTime = Animator.StringToHash("StateTime");
    readonly int m_HashMeelAttack = Animator.StringToHash("MeelAttack");
    private void Update()
    {
        ani.SetFloat(m_HashStateTime, Mathf.Repeat(ani.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f));

        if(Input.GetMouseButtonDown(0) && !GetComponent<ZoomAim>().isAim())
        {
            ani.SetTrigger(m_HashMeelAttack);
        }
    }
}
