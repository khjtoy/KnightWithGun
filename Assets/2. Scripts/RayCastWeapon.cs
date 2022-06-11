using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastWeapon : MonoBehaviour
{
    public bool isFiring = false;
    public ParticleSystem muzzleFlash;
    public Transform raycastOrigin;
    public Transform raycastDestInation;

    Ray ray;
    RaycastHit hitInfo;
    public void StartFiring()
    {
        isFiring = true;
        muzzleFlash.Emit(30);

        ray.origin = raycastOrigin.position;
        ray.direction = raycastDestInation.position - raycastOrigin.position;
        if(Physics.Raycast(ray, out hitInfo))
        {
            Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 1.0f);
        }
    }

    public void StopFiring()
    {
        isFiring = false;
    }
}
