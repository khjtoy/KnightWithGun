using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DefineCS;

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
        GameObject particleSystem = ObjectPoolMgr.Instance.GetPooledObject((int)PooledIndex.EXPLOSION);
        particleSystem.transform.position = transform.position;
        particleSystem.transform.rotation = transform.rotation;
        particleSystem.SetActive(true);
        //ParticleSystem particleSystem = Instantiate(explosionPrefab, transform.position, transform.rotation).GetComponent<ParticleSystem>();

        // ÆøÅº ¹üÀ§¿¡ ÀÖ´ÂÁö
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach(Collider hit in colliders)
        {
            Monster monster =  hit.GetComponent<Monster>();
            if (monster != null)
            {
                Debug.Log("Á¢±Ù");
                monster.MonsterHit(new Vector3(100,-100,100), new Vector3(100, -100, 100),60);
            }

            Rigidbody rigidbody = hit.GetComponent<Rigidbody>();
            if(rigidbody != null)
            {
                rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        if(collision.collider.CompareTag("Gate"))
        {
            collision.transform.parent.gameObject.SetActive(false);
            collision.transform.parent.parent.GetChild(1).gameObject.SetActive(true);
        }

        if(collision.collider.CompareTag("BossGate"))
        {
            collision.transform.parent.gameObject.SetActive(false);
            collision.transform.parent.parent.GetChild(1).gameObject.SetActive(true);
        }
        Destroy(gameObject);
    }
}
