using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RayCastWeapon : MonoBehaviour
{
    private bool isFiring = false;
    [SerializeField]
    private float fireRate = 25;
    [SerializeField]
    private ParticleSystem muzzleFlash;
    [SerializeField]
    private ParticleSystem hitEffect;
    [SerializeField]
    private Transform raycastOrigin;
    [SerializeField]
    private Transform raycastDestInation;
    [SerializeField]
    private TrailRenderer tracerEffect;

    // Recoil
    [SerializeField]
    private Transform camRecoil;
    [SerializeField]
    private Vector3 recoilKickback;
    [SerializeField]
    private float recoilAmount;

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

    [SerializeField]
    private AudioSource shotAudio;

    private readonly int hashReload = Animator.StringToHash("Reload");
    private void Start()
    {
        shotAudio = GetComponent<AudioSource>();
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
        shotAudio.Play();
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

        var tracer = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
        tracer.AddPosition(ray.origin);
        recoil.RecoilFire();

        if (Physics.Raycast(ray, out hitInfo))
        {
            Monster monster = hitInfo.collider.gameObject.GetComponent<Monster>();
            if(monster != null)
            {
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
        playerAni.SetTrigger(hashReload);
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
