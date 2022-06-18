using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RayCastWeapon : MonoBehaviour
{
    public bool isFiring = false;
    public float fireRate = 25;
    public ParticleSystem muzzleFlash;
    public ParticleSystem hitEffect;
    public Transform raycastOrigin;
    public Transform raycastDestInation;
    public TrailRenderer tracerEffect;

    // Recoil
    public Transform camRecoil;
    public Vector3 recoilKickback;
    public float recoilAmount;


    Ray ray;
    RaycastHit hitInfo;

    [SerializeField]
    private OpaqueItem opaqueItem;

    [SerializeField]
    private Recoil recoil;

    public void StartFiring()
    {
        isFiring = true;
        FireBullet();
    }


    private void FireBullet()
    {
        if(opaqueItem.isOpaque)
        {
            opaqueItem.Kill();
        }
        muzzleFlash.Emit(30);

        ray.origin = raycastOrigin.position;
        ray.direction = raycastDestInation.position - raycastOrigin.position;


        Debug.Log("zz");
        var tracer = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
        tracer.AddPosition(ray.origin);
        recoil.RecoilFire();

        if (Physics.Raycast(ray, out hitInfo))
        {
            Monster monster = hitInfo.collider.gameObject.GetComponent<Monster>();
            if(monster != null)
            {
                Debug.Log("Á¢±Ù");
                monster.MonsterHit(hitInfo.point, hitInfo.normal, 10);
            }
            Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 1.0f);

            Debug.Log(hitInfo.collider.name);
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(5);

            tracer.transform.position = hitInfo.point;
        }

    }

    public void StopFiring()
    {
        isFiring = false;
    }
}
