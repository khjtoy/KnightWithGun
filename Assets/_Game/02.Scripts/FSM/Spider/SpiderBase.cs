using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBase : IState
{
    protected SpiderStateMachine stateMachine;

    protected float distance;

    protected readonly int hashTrace = Animator.StringToHash("IsTrace");
    protected readonly int hashPatrol = Animator.StringToHash("IsPatrol");
    protected readonly int hashAttack = Animator.StringToHash("IsAttack");

    public SpiderBase(SpiderStateMachine spiderStateMachine)
    {
        stateMachine = spiderStateMachine;
    }

    public virtual void Enter()
    {

    }

    public virtual void Exit()
    {

    }

    public virtual void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("GUN"))
        {
            stateMachine.Spider.MonsterHit(stateMachine.Spider.MonsterTransform.position, stateMachine.Spider.MonsterTransform.rotation.eulerAngles, 40);
        }
    } 

    public virtual void Update()
    {
        if (stateMachine.Spider.IsDie)
            stateMachine.ChangeState(stateMachine.SpiderDie);

        // 몬스터의 캐릭터 사이의 거리 측정
        distance = Vector3.Distance(stateMachine.Spider.MonsterTransform.position, 
            stateMachine.Spider.TargetTransform.position);
    }
}
