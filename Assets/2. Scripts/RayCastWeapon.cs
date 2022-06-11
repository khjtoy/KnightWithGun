using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastWeapon : MonoBehaviour
{
    public bool isFiring = false;
    public int fireRate = 25;
    public ParticleSystem muzzleFlash;
    public ParticleSystem hitEffect;
    public Transform raycastOrigin;
    public Transform raycastDestInation;
    //public TrailRenderer tracerEffect;

    Ray ray;
    RaycastHit hitInfo;
    public void StartFiring()
    {
        isFiring = true;
        FireBullet();
    }

    private void FireBullet()
    {
        muzzleFlash.Emit(30);

        ray.origin = raycastOrigin.position;
        ray.direction = raycastDestInation.position - raycastOrigin.position;

        Debug.Log("zz");
        //var tracer = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
        //tracer.AddPosition(ray.origin);

        if (Physics.Raycast(ray, out hitInfo))
        {
            Monster monster = hitInfo.collider.gameObject.GetComponent<Monster>();
            if(monster != null)
            {
                Debug.Log("Á¢±Ù");
                monster.MonsterHit();
            }
            Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 1.0f);

            Debug.Log(hitInfo.collider.name);
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(5);

            //tracer.transform.position = hitInfo.point;
        }
    }

    public void StopFiring()
    {
        isFiring = false;
    }
}
