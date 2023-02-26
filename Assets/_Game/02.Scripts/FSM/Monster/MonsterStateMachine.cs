using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStateMachine : StateMachine
{
    public MonsterCtrl Monster { get; }

    public MonsterStateMachine(MonsterCtrl monsterCtrl)
    {
        Monster = monsterCtrl;
    }
}
