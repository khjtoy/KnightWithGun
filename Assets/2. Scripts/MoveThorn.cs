using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveThorn : MonoBehaviour
{
    //private GameObject player;
    //private Vector3 moveDir;

    [SerializeField]
    private float moveSpeed;
    private EventParam damageParam;
    private void Start()
    {
        //player = GameObject.FindWithTag("PLAYER");

        //이동할 방향
        //moveDir = (player.transform.position - transform.position).normalized;

        //moveDir.y = 0;

        //transform.rotation = Quaternion.LookRotation(moveDir);
        damageParam.intParam = 5;
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed);
        Invoke("Despawn", 3f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //moveSpeed = 0;
        Debug.Log("들어옴");
        if(collision.collider.CompareTag("PLAYER"))
        {
            EventManager.TriggerEvent("PLAYER_DAMAGE", damageParam);
        }
        Despawn();
    }

    private void Despawn()
    {
        ObjectPoolMgr.Instance.Despawn(gameObject);
    }
}
