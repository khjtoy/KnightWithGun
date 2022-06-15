using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField]
    private Transform player;

    private void Start()
    {
        Debug.Log("11");
    }
    void Update()
    {
        transform.LookAt(player);
    }
}
