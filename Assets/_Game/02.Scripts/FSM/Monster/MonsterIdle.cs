using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterIdle : MonsterBase
{
    public MonsterIdle(MonsterStateMachine monsterStateMachine) : base(monsterStateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();

        foreach (SphereCollider sphere in stateMachine.Monster.Spheres)
        {
            sphere.enabled = false;
        }
        stateMachine.Monster.Anim.SetBool(hashAttack, false);
        stateMachine.Monster.Anim.SetBool(hashShoot, false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (!stateMachine.Monster.OpaqueItem.isOpaque)
        {
            if (distance <= stateMachine.Monster.AttackDist)
            {
                stateMachine.ChangeState(stateMachine.MonsterSlam);
            }
            else if (distance <= stateMachine.Monster.LongDistnaceAttackDist)
            {
                stateMachine.ChangeState(stateMachine.MonsterAttack);
            }
        }
    }
}
