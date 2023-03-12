using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateMachine : StateMachine
{
    public BossCtrl Boss { get; }

    public BossIdle BossIdle { get; }
    public BossTrace BossTrace { get; }
    public BossAttack BossAttack { get; }
    public BossRotation BossRotation { get; }
    public BossRunAttack BossRunAttack { get; }
    public BossDie BossDie { get; }

    public BossStateMachine(BossCtrl bossCtrl)
    {
        Boss = bossCtrl;

        BossIdle = new BossIdle(this);
        BossTrace = new BossTrace(this);
        BossAttack = new BossAttack(this);
        BossRotation = new BossRotation(this);
        BossRunAttack = new BossRunAttack(this);
        BossDie = new BossDie(this);
    }
}
