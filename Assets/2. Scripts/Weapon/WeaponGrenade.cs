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
        EventManager.StartListening("ShootGrenada", SpawnGrenadeProjectile);
    }
    private void Update()
    {
        ItemAction();
    }
    public void ItemAction()
    {
        if(Input.GetMouseButtonDown(0) && !transform.parent.GetComponent<ZoomAim>().isAim() && playerWeapon.weaponIndex == 1)
        {
            animator.SetTrigger("Bomb");
        }
        //StartCoroutine("OnAttack");
    }

    public void Kill()
    {  

    }


    public void SpawnGrenadeProjectile(EventParam eventParam)
    {
        GameObject grenadeClone = Instantiate(grenadePrefab, grenadeSpawnPoint.position, Random.rotation);
        grenadeClone.GetComponent<Grenade>().Setup(10, transform.parent.forward);
    }

    private void OnDestroy()
    {
        EventManager.StopListening("ShootGrenada", SpawnGrenadeProjectile);
    }

    private void OnApplicationQuit()
    {
        EventManager.StopListening("ShootGrenada", SpawnGrenadeProjectile);
    }

    private void OnEnable()
    {
        // 탄창 정보 갱신
        // 탄 수 정보 갱신
    }


}
