using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttack : MonsterBase
{
    public MonsterAttack(MonsterStateMachine monsterStateMachine) : base(monsterStateMachine)
    {

    }

    public override void Enter()
    {
        stateMachine.Monster.Anim.SetBool(hashAttack, false);
        stateMachine.Monster.Anim.SetBool(hashShoot, true);
    }

    public override void Exit()
    {
        base.Exit();
    }
    public override void Update()
    {
        base.Update();
        if (stateMachine.Monster.OpaqueItem.isOpaque || distance > stateMachine.Monster.LongDistnaceAttackDist || distance <= stateMachine.Monster.AttackDist)
        {
            stateMachine.ChangeState(stateMachine.MonsterIdle);
        }
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}
