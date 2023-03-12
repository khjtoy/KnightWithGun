using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderTrace : SpiderBase
{
    public SpiderTrace(SpiderStateMachine spiderStateMachine) : base(spiderStateMachine)
    {

    }
    public override void Enter()
    {
        base.Enter();
        // 추적 대상 좌표로 이동
        stateMachine.Spider.Agent.speed = 10f;
        stateMachine.Spider.Agent.isStopped = false;
        stateMachine.Spider.Anim.SetBool(hashTrace, true);
        stateMachine.Spider.Anim.SetBool(hashAttack, false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        stateMachine.Spider.Agent.SetDestination(stateMachine.Spider.TargetTransform.position);

        if (stateMachine.Spider.OpaqueItem.isOpaque || distance <= stateMachine.Spider.AttackDist ||
            stateMachine.Spider.Theta > stateMachine.Spider.ViewAngle / 2 
            || distance > stateMachine.Spider.TraceDist)
        {
            stateMachine.ChangeState(stateMachine.SpiderIdle);
        }
    }
}
