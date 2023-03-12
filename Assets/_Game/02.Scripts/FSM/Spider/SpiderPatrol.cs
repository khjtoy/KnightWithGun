using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderPatrol : SpiderBase
{
    public SpiderPatrol(SpiderStateMachine spiderStateMachine) : base(spiderStateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.Spider.Agent.speed = 7f;
        stateMachine.Spider.Agent.isStopped = false;
        stateMachine.Spider.Anim.SetBool(hashPatrol, true);
        stateMachine.Spider.Anim.SetBool(hashAttack, false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        // Patrol ÁöÁ¤
        if (Vector3.Distance(stateMachine.Spider.MonsterTransform.position
            , stateMachine.Spider.Target) < 7f)
        {
            stateMachine.Spider.IterateWaypointIndex();
            stateMachine.Spider.Target = stateMachine.Spider.Waypoints[stateMachine.Spider.WaypointIndex].position;
        }

        stateMachine.Spider.Agent.SetDestination(stateMachine.Spider.Target);

        if (!stateMachine.Spider.OpaqueItem.isOpaque)
        {
            if (distance <= stateMachine.Spider.AttackDist ||
                (stateMachine.Spider.Theta <= stateMachine.Spider.ViewAngle / 2
                && distance <= stateMachine.Spider.TraceDist))
            {
                stateMachine.ChangeState(stateMachine.SpiderIdle);
            }
        }
    }
}
