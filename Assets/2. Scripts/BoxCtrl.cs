using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DefineCS;

public class BoxCtrl : Monster
{
    private BoxCollider myColider;
    private Animator animator;
    private int hp;
    private readonly int hashCrash = Animator.StringToHash("Crash");

    private void Start()
    {
        myColider = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();
        hp = 30;
    }
    public override void MonsterHit(Vector3 bloodPos, Vector3 bloodRot, int damage)
    {
        if (hp <= 0) return;

        hp -= damage;

        if(hp <= 0)
        {
            animator.SetTrigger(hashCrash);
            myColider.enabled = false;
            // 0 Hp 1 ¹° 2 ¼û±â
            int random = Random.Range(0, 3);
            GameObject bottle = null;
            switch (random)
            {
                case 0:
                    bottle = ObjectPoolMgr.Instance.GetPooledObject((int)PooledIndex.HP_BOTTLE);
                    break;
                case 1:
                    bottle = ObjectPoolMgr.Instance.GetPooledObject((int)PooledIndex.WATER_BOTTLE);
                    break;
                case 2:
                    bottle = ObjectPoolMgr.Instance.GetPooledObject((int)PooledIndex.HIDE_BOTTLE);
                    break;
            }
            bottle.transform.position = transform.localPosition;
            bottle.SetActive(true);
        }
    }
}
