using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DefineCS;

public class FireCtrl : MonoBehaviour
{
    // Bullet Fire Pos
    public Transform firePos;
    [SerializeField]
    private LayerMask aimColiderLayerMask = new LayerMask();
    [SerializeField]
    private Vector3 mouseWorldPos;

    private void Start()
    {
        
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
        // Bullet »ý¼º
        //GameObject bullet = ObjectPoolMgr.Instance.GetPooledObject((int)PooledIndex.BULLET);
        //bullet.transform.position = firePos.position;
      //  bullet.transform.rotation = firePos.rotation;
       // bullet.SetActive(true);

        Vector3 aimDir = (mouseWorldPos - firePos.position).normalized;
        GameObject bullet = ObjectPoolMgr.Instance.GetPooledObject((int)PooledIndex.BULLET);
        bullet.transform.position = firePos.position;
        bullet.transform.rotation = Quaternion.LookRotation(aimDir,Vector3.up);
        bullet.SetActive(true);
    }
}
