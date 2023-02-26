using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    public abstract void MonsterHit(Vector3 bloodPos, Vector3 bloodRot, int damage);
}
