using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGrenade : MonoBehaviour, Item
{
    [SerializeField]
    private GameObject grenadePrefab;
    [SerializeField]
    private Transform grenadeSpawnPoint;

    [SerializeField]
    private Animator animator;

    private PlayerWeapon playerWeapon;


    private void Start()
    {
        playerWeapon = transform.parent.GetComponent<PlayerWeapon>();
    }
    private void Update()
    {
        ItemAction();
    }
    public void ItemAction()
    {
        if(Input.GetMouseButtonDown(0) && !transform.parent.GetComponent<ZoomAim>().isAim() && playerWeapon.weaponIndex == 1)
        {
            SpawnGrenadeProjectile();
        }
        //StartCoroutine("OnAttack");
    }

    public void Kill()
    {  

    }


    public void SpawnGrenadeProjectile()
    {
        GameObject grenadeClone = Instantiate(grenadePrefab, grenadeSpawnPoint.position, Random.rotation);
        grenadeClone.GetComponent<Grenade>().Setup(10, transform.parent.forward);
        animator.SetTrigger("Bomb");
    }

    private void OnEnable()
    {
        // 탄창 정보 갱신
        // 탄 수 정보 갱신
    }


}
