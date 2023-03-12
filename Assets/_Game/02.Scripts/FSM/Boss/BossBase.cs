using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBase : IState
{
    protected BossStateMachine stateMachine;

    protected readonly int hashTrace = Animator.StringToHash("IsTrace");


    public BossBase(BossStateMachine bossStateMachine)
    {
        stateMachine = bossStateMachine;
    }

    public virtual void Enter()
    {

    }

    public virtual void Exit()
    {

    }

    public virtual void OnTriggerEnter(Collider other)
    {

    }

    public virtual void Update()
    {

    }
}
