using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpaqueItem : MonoBehaviour, Item
{
    // 0, 1 -> Player 2 -> Gun
    [SerializeField]
    private Renderer[] opaqueObject = new Renderer[3];
    // 0 -> Player 1 -> Gun
    [SerializeField]
    private Material[] opaqueMaterial = new Material[2];
    [SerializeField]
    private Material[] normalMaterial = new Material[2];
    [SerializeField]
    private float timer;

    private PlayerStats playerStats;

    private Animator playerAni;

    public bool isOpaque { get; set; } = false;

    private int hashDrink = Animator.StringToHash("Drinking");

    private bool isPlaying = false;

    private void Start()
    {
        playerStats = GameObject.FindWithTag("PLAYER").GetComponent<PlayerStats>();
        playerAni = GameObject.FindWithTag("PLAYER").GetComponent<Animator>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha3) && playerStats.HideCount > 0 && !isPlaying)
        {
            isPlaying = true;
            playerStats.HideCount--;
            playerStats.ChangeItemUI();
            playerAni.SetTrigger(hashDrink);
            Invoke("ItemAction", 4);
            //ItemAction();
        }
    }

    public void ItemAction()
    {
        Invoke("Kill", timer);
        opaqueObject[0].material = opaqueMaterial[0];
        opaqueObject[1].material = opaqueMaterial[0];
        opaqueObject[2].material = opaqueMaterial[1];
        isOpaque = true;
    }

    public void Kill()
    {
        opaqueObject[0].material = normalMaterial[0];
        opaqueObject[1].material = normalMaterial[0];
        opaqueObject[2].material = normalMaterial[1];
        isOpaque = false;
        isPlaying = false;
    }
}
