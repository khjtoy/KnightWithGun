using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XTarget : MonoBehaviour
{
    [SerializeField]
    private int count = 0;

    public void AddCount()
    {
        count++;

        if(count == 5)
        {
            transform.GetComponentInParent<BossCtrl>().DamageTarget();
            count = 0;
            
        }
    }
}
