using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSlam : MonsterBase
{
    public MonsterSlam(MonsterStateMachine monsterStateMachine) : base(monsterStateMachine)
    {

    }

    public override void Enter()
    {
        Debug.Log("µé¾î¿È");
        foreach (SphereCollider sphere in stateMachine.Monster.Spheres)
        {
            sphere.enabled = true;
        }
        stateMachine.Monster.Anim.SetBool(hashAttack, true);
        stateMachine.Monster.Anim.SetBool(hashShoot, false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        if (stateMachine.Monster.OpaqueItem.isOpaque || distance > stateMachine.Monster.AttackDist)
        {
            stateMachine.ChangeState(stateMachine.MonsterIdle);
        }
    }
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}
