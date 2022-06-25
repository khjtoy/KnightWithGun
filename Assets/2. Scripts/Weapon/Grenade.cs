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
        ParticleSystem particleSystem = Instantiate(explosionPrefab, transform.position, transform.rotation).GetComponent<ParticleSystem>();

        if(collision.collider.CompareTag("Gate"))
        {
            particleSystem.Emit(1000);
            collision.transform.parent.gameObject.SetActive(false);
            collision.transform.parent.parent.GetChild(1).gameObject.SetActive(true);
        }

        Destroy(gameObject);
    }
}
