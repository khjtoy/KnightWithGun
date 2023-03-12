using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderDie : SpiderBase
{
    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashBoolDie = Animator.StringToHash("IsDie");

    public SpiderDie(SpiderStateMachine spiderStateMachine) : base(spiderStateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        stateMachine.Spider.Anim.SetBool(hashBoolDie, true);
        stateMachine.Spider.Anim.SetBool(hashTrace, false);
        stateMachine.Spider.Anim.SetBool(hashAttack, false);
        stateMachine.Spider.Anim.SetBool(hashPatrol, false);

        stateMachine.Spider.Agent.isStopped = true;
        stateMachine.Spider.Anim.SetTrigger(hashDie);
        stateMachine.Spider.DamageCol.enabled = false;
        stateMachine.Spider.HealthBarUI.gameObject.SetActive(false);
        stateMachine.Spider.Rigidbody.isKinematic = false;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {

    }

    public override void OnTriggerEnter(Collider other)
    {

    }
}
