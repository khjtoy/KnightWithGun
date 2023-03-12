using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderIdle : SpiderBase
{
    public SpiderIdle(SpiderStateMachine spiderStateMachine) : base(spiderStateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        // 추적 중지
        stateMachine.Spider.DamageCol.enabled = false;
        stateMachine.Spider.Agent.isStopped = true;
        stateMachine.Spider.Anim.SetBool(hashTrace, false);
        stateMachine.Spider.Anim.SetBool(hashPatrol, false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if(stateMachine.Spider.OpaqueItem.isOpaque)
        {
            stateMachine.ChangeState(stateMachine.SpiderPatrol);
        }
        else if (distance <= stateMachine.Spider.AttackDist)
        {
            stateMachine.ChangeState(stateMachine.SpiderAttack);
        }
        else if (stateMachine.Spider.Theta <= stateMachine.Spider.ViewAngle / 2 && 
            distance <= stateMachine.Spider.TraceDist)
        {
            stateMachine.ChangeState(stateMachine.SpiderTrace);
        }
        else
        {
            stateMachine.ChangeState(stateMachine.SpiderPatrol);
        }
    }
}
