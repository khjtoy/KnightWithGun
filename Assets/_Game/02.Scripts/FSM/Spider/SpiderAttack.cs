using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderAttack : SpiderBase
{
    public SpiderAttack(SpiderStateMachine spiderStateMachine) : base(spiderStateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.Spider.DamageCol.enabled = true;
        stateMachine.Spider.Anim.SetBool(hashTrace, true);
        stateMachine.Spider.Anim.SetBool(hashAttack, true);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if(stateMachine.Spider.OpaqueItem.isOpaque || distance > stateMachine.Spider.AttackDist)
        {
            stateMachine.ChangeState(stateMachine.SpiderIdle);
        }
    }
}
