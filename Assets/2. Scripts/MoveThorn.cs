using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveThorn : MonoBehaviour
{
    private GameObject player;
    private Vector3 moveDir;

    [SerializeField]
    private float moveSpeed;
    private void Start()
    {
        player = GameObject.FindWithTag("PLAYER");

        //이동할 방향
        moveDir = (player.transform.position - transform.position).normalized;

        //moveDir.y = 0;

        transform.rotation = Quaternion.LookRotation(moveDir);
    }

    private void Update()
    {
        transform.Translate(moveDir * moveSpeed);
    }
}
