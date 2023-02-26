using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static DefineCS;

public class FireCtrl : Character
{
    // Bullet Fire Pos
    public Transform firePos;
    [SerializeField]
    private LayerMask aimColiderLayerMask = new LayerMask();
    [SerializeField]
    private Vector3 mouseWorldPos;
    [SerializeField]
    private ParticleSystem muzzleFlashSystem;
    [SerializeField]
    private Text bulletText;

    // 총알 최대 개수
    [SerializeField]
    private int maxBullet = 50;

    // 남은 총알 개수
    private int currentBullet;

    private void Start()
    {
        EventManager.StartListening("BULLET_RELOAD", Reloading);
        SetMaxBullet();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Fire();
        }

        CheckCenterPoint();

    }

    private void CheckCenterPoint()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColiderLayerMask))
        {
            mouseWorldPos = raycastHit.point;
        }
    }

    private void Fire()
    {
        // Bullet 생성
        //GameObject bullet = ObjectPoolMgr.Instance.GetPooledObject((int)PooledIndex.BULLET);
        //bullet.transform.position = firePos.position;
        //  bullet.transform.rotation = firePos.rotation;
        // bullet.SetActive(true);

        if (currentBullet <= 0) return;

        Vector3 aimDir = (mouseWorldPos - firePos.position).normalized;
        GameObject bullet = ObjectPoolMgr.Instance.GetPooledObject((int)PooledIndex.BULLET);
        bullet.transform.position = firePos.position;
        bullet.transform.rotation = Quaternion.LookRotation(aimDir,Vector3.up);
        bullet.SetActive(true);
        currentBullet--;
        bulletText.text = string.Format("{0}", currentBullet);
        StartCoroutine(ShootVFXRoutine());
    }

    private void Reloading(EventParam eventParam)
    {
        if (currentBullet == maxBullet) return;
        ani.SetTrigger("Reload");
    }

    private void SetMaxBullet()
    {
        currentBullet = maxBullet;
        bulletText.text = string.Format("{0}", currentBullet);
    }

    IEnumerator ShootVFXRoutine()
    {
        yield return new WaitForSeconds(0.11f);
        muzzleFlashSystem.Emit(30);
        //muzzleFlashSystem.Play();
        //bulletCasingEjectSystem.Emit(1);
    }
}
