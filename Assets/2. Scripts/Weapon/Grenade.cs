using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField]
    private GameObject explosionPrefab;
    [SerializeField]
    private float explosionRadius = 10.0f;
    [SerializeField]
    private float explosionForce = 500.0f;
    [SerializeField]
    private float throwForce = 1000.0f;

    private int explosionDamage;
    private new Rigidbody rigidbody;

    public void Setup(int damage, Vector3 rot)
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(rot * throwForce);

        explosionDamage = damage;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ÀÌÆåÆ® »ý¼º
        Instantiate(explosionPrefab, transform.position, transform.rotation);

        Destroy(gameObject);
    }
}
