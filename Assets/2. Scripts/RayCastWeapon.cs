using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    [SerializeField]
    private Animator playerAni;


    Ray ray;
    RaycastHit hitInfo;

    [SerializeField]
    private OpaqueItem opaqueItem;

    [SerializeField]
    private Recoil recoil;

    // 총알 최대 개수
    [SerializeField]
    private int maxBullet = 50;

    // 남은 총알 개수
    private int currentBullet;

    private bool reloding = false;

    [SerializeField]
    private Text bulletText;
    private void Start()
    {
        EventManager.StartListening("BULLET_RELOAD", Reloading);
        EventManager.StartListening("MAX", SetMaxBullet);
        SetMaxBullet(new EventParam());
    }

    public int GetBullet()
    {
        return currentBullet;
    }


    public void StartFiring()
    {
        if (currentBullet <= 0 || reloding) return;

        //playerAni.SetTrigger("Shoot");
        isFiring = true;
        currentBullet--;
        bulletText.text = string.Format("{0}", currentBullet);
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
                Debug.Log("접근");
                monster.MonsterHit(hitInfo.point, hitInfo.normal, 10);
            }
            if(hitInfo.collider.CompareTag("XTARGET"))
            {
                hitInfo.collider.GetComponent<XTarget>().AddCount();
            }
            Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 1.0f);

            Debug.Log(hitInfo.collider.name);
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(5);

            tracer.transform.position = hitInfo.point;
        }

    }

    private void Reloading(EventParam eventParam)
    {
        if (currentBullet == maxBullet) return;
        playerAni.SetTrigger("Reload");
        reloding = true;
    }

    public void SetMaxBullet(EventParam eventParam)
    {
        currentBullet = maxBullet;
        bulletText.text = string.Format("{0}", currentBullet);
        reloding = false;
    }

    public bool GetReloding()
    {
        return reloding;
    }

    private void OnDestroy()
    {
        EventManager.StopListening("BULLET_RELOAD", Reloading);
        EventManager.StopListening("MAX", SetMaxBullet);
    }

    private void OnApplicationQuit()
    {
        EventManager.StopListening("BULLET_RELOAD", Reloading);
        EventManager.StopListening("MAX", SetMaxBullet);
    }

    public void StopFiring()
    {
        isFiring = false;
    }
}
