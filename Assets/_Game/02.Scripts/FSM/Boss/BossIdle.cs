using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIdle : BossBase
{
    public BossIdle(BossStateMachine bossStateMachine) : base(bossStateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        // 추적 중지
        stateMachine.Boss.Agent.isStopped = true;
        stateMachine.Boss.Anim.SetBool(hashTrace, false);
    }

    public override void Update()
    {
        base.Update();

        // 몬스터의 캐릭터 사이의 거리 측정
        float distance = Vector3.Distance(stateMachine.Boss.MonsterTransform.position, stateMachine.Boss.TargetTransform.position);

        if (distance <= stateMachine.Boss.AttackDist)
        {
            if (stateMachine.Boss.Theta <= stateMachine.Boss.ViewAngle / 2)
            {
                stateMachine.ChangeState(stateMachine.BossAttack);
            }
            else
                stateMachine.ChangeState(stateMachine.BossRotation);
        }
        else if (distance >= stateMachine.Boss.LongDist)
        {
            stateMachine.ChangeState(stateMachine.BossRunAttack);
        }
        else
        {
            stateMachine.ChangeState(stateMachine.BossTrace);
        }
    }
}
