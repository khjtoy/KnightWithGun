using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStateMachine : StateMachine
{
    public MonsterCtrl Monster { get; }

    public MonsterIdle MonsterIdle { get; }
    public MonsterSlam MonsterSlam { get; }
    public MonsterAttack MonsterAttack { get; }
    public MonsterDie MonsterDie { get; }

    public MonsterStateMachine(MonsterCtrl monsterCtrl)
    {
        Monster = monsterCtrl;

        MonsterIdle = new MonsterIdle(this);
        MonsterSlam = new MonsterSlam(this);
        MonsterAttack = new MonsterAttack(this);
        MonsterDie = new MonsterDie(this);
    }
}
