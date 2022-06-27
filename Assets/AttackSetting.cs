using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AttackSetting : StateMachineBehaviour
{
    private bool isOpen = false;
    private bool isKick = false;


    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
    {
        if (!animator.GetBool("Long") && !isOpen)
        {
            if(animator.GetFloat("SDIndex") == 0)
            {
                if (stateInfo.normalizedTime > 0.25f)
                {
                    isOpen = true;
                    isKick = true;
                    animator.GetComponentInParent<BossCtrl>().SetSword(true);
                }
            }
            else if (animator.GetFloat("SDIndex") == 1)
            {
                if (stateInfo.normalizedTime > 0.2f)
                {
                    isOpen = true;
                    animator.GetComponentInParent<BossCtrl>().radiusSword(1);
                    animator.GetComponentInParent<BossCtrl>().SetSword(true);
                }
            }
        }

        else if(animator.GetBool("Long") && !isOpen)
        {
            if(stateInfo.normalizedTime > 0.4f)
            {
                isOpen = true;
                animator.GetComponentInParent<BossCtrl>().SetSword(true);
            }
        }

        if(isKick)
        {
            if (stateInfo.normalizedTime > 0.85f)
            {
                isKick = false;
                animator.GetComponentInParent<BossCtrl>().SetKick(true);
            }
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponentInParent<BossCtrl>().Wait();
        animator.GetComponentInParent<BossCtrl>().SetSword(false);
        animator.GetComponentInParent<BossCtrl>().SetKick(false);
        animator.GetComponentInParent<BossCtrl>().radiusSword(0.24f);
        isOpen = false;
    }
}
