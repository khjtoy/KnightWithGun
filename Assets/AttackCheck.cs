using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCheck : StateMachineBehaviour
{

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<DogKnightCtrl>().OffParticle();
    }
}
