using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBase : IState
{
    protected MonsterStateMachine stateMachine;

    protected float distance;

    protected readonly int hashAttack = Animator.StringToHash("IsAttack");
    protected readonly int hashShoot = Animator.StringToHash("IsShoot");

    public MonsterBase(MonsterStateMachine monsterStateMachine)
    {
        stateMachine = monsterStateMachine;
    }

    public virtual void Enter()
    {

    }

    public virtual void Exit()
    {

    }

    public virtual void Update()
    {
        if(stateMachine.Monster.IsDie)
        {
            stateMachine.ChangeState(stateMachine.MonsterDie);
        }

        // 몬스터의 캐릭터 사이의 거리 측정
        distance = Vector3.Distance(stateMachine.Monster.MonsterTransform.position, stateMachine.Monster.TargetTransform.position);
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GUN"))
        {
            stateMachine.Monster.MonsterHit(stateMachine.Monster.MonsterTransform.position, stateMachine.Monster.MonsterTransform.rotation.eulerAngles, 40);
        }
    }

    public virtual void OnDrawGizmos()
    {
        
    }
}
