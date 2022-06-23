using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DefineCS;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        SpawnSpider();
    }

    private void SpawnSpider()
    {
        for(int i = 0; i < 50; i++)
        {
            GameObject spider = ObjectPoolMgr.Instance.GetPooledObject((int)PooledIndex.SPIDER);
            spider.GetComponent<SpiderCtrl>().RandomPos();
            spider.SetActive(true);
        }
    }
}
