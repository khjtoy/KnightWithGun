using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderStateMachine : StateMachine
{
    public SpiderCtrl Spider { get; }

    public SpiderIdle SpiderIdle { get; }
    public SpiderPatrol SpiderPatrol { get; }
    public SpiderTrace SpiderTrace { get; }
    public SpiderAttack SpiderAttack { get; }
    public SpiderDie SpiderDie { get; }

    public SpiderStateMachine(SpiderCtrl spiderCtrl)
    {
        Spider = spiderCtrl;

        SpiderIdle = new SpiderIdle(this);
        SpiderPatrol = new SpiderPatrol(this);
        SpiderTrace = new SpiderTrace(this);
        SpiderAttack = new SpiderAttack(this);
        SpiderDie = new SpiderDie(this);
    }
}
