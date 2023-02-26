using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckAttack : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerAttack playerAttack = animator.gameObject.GetComponent<PlayerAttack>();
        playerAttack.IsAttack = false;
        playerAttack.OffPaticle();
        
    }
}
