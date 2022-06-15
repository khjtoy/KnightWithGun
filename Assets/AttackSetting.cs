using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSetting : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<BossCtrl>().Wait();
    }
}
