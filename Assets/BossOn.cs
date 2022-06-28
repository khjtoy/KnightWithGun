using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossOn : MonoBehaviour
{
    [SerializeField]
    private MeshCollider gate;

    [SerializeField]
    private BossCtrl bossCtrl;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PLAYER"))
        {
            gate.enabled = true;
            bossCtrl.enabled = true;
        }    
    }
}
