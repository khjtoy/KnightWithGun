using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDie : MonsterBase
{
    private readonly int hashBDie = Animator.StringToHash("IsDie");
    private readonly int hashDie = Animator.StringToHash("Die");

    public MonsterDie(MonsterStateMachine monsterStateMachine) : base(monsterStateMachine)
    {

    }

    public override void Enter()
    {
        foreach (SphereCollider sphere in stateMachine.Monster.Spheres)
        {
            sphere.enabled = false;
        }
        stateMachine.Monster.HealthBarUI.gameObject.SetActive(false);
        stateMachine.Monster.Anim.SetBool(hashBDie, stateMachine.Monster.IsDie);
        stateMachine.Monster.Anim.SetTrigger(hashDie);
        stateMachine.Monster.Rigid.isKinematic = false;
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
