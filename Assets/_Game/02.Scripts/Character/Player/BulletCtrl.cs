using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    //ÃÑ¾Ë ¹ß»ç Èû
    public float force = 1500f;

    private Rigidbody bulletRigidbody;
    private Transform bulletTransform = null;

    private void Start()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
        bulletTransform = GetComponent<Transform>();
        bulletRigidbody.AddForce(bulletTransform.forward * force);
        //bulletRigidbody.velocity = transform.forward * force;
        Invoke("Despawn", 3f);
    }

    private void Despawn()
    {
        ObjectPoolMgr.Instance.Despawn(gameObject);
    }
}
