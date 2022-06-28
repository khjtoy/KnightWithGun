using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DefineCS;

public class GameManager : MonoBehaviour
{
    int maxTime = 10;
    float timer = 0;
    int MaxCount = 100;
    int count = 0;
    private void Start()
    {
        SpawnSpider();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        
        if(timer >= maxTime)
        {
            count++;
            GameObject spider = ObjectPoolMgr.Instance.GetPooledObject((int)PooledIndex.SPIDER);
            spider.GetComponent<SpiderCtrl>().RandomPos();
            spider.SetActive(true);
            timer = 0;
        }
    }

    private void SpawnSpider()
    {
        for(int i = 0; i < 100; i++)
        {
            GameObject spider = ObjectPoolMgr.Instance.GetPooledObject((int)PooledIndex.SPIDER);
            spider.GetComponent<SpiderCtrl>().RandomPos();
            spider.SetActive(true);
        }
    }
}
