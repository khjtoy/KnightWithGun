using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponGrenade : MonoBehaviour, Item
{
    [SerializeField]
    private GameObject grenadePrefab;
    [SerializeField]
    private Transform grenadeSpawnPoint;

    [SerializeField]
    private Animator animator;

    private PlayerWeapon playerWeapon;


    public int Grenada { get; private set; } = 10;

    [SerializeField]
    private Text countText;

    private readonly int hashBomb = Animator.StringToHash("Bomb");

    private void Start()
    {
        playerWeapon = transform.parent.GetComponent<PlayerWeapon>();
        EventManager.StartListening("ShootGrenada", SpawnGrenadeProjectile);
    }
    private void Update()
    {
        ItemAction();
    }

    public void AddGrenada()
    {
        Grenada++;
        if(playerWeapon.weaponIndex == 1)
            countText.text = string.Format("{0}", Grenada);
    }
    public void ItemAction()
    {
        if (Grenada <= 0) return;
        if(Input.GetMouseButtonDown(0) && !transform.parent.GetComponent<ZoomAim>().isAim() && playerWeapon.weaponIndex == 1)
        {
            animator.SetTrigger(hashBomb);
            Grenada--;
            countText.text = string.Format("{0}", Grenada);
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
}
