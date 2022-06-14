using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField]
    private RectTransform rect;

    [Header("1.ÃÑ, 2.ÆøÅº")]
    [SerializeField]
    private Sprite[] weaponImage;

    public int weaponIndex { get; private set; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            ChangePanel();
    }

    public void ChangePanel()
    {
        rect.DOKill();
        weaponIndex++;
        if (weaponIndex >= 2) weaponIndex = 0;
        rect.anchoredPosition = new Vector3(265, 31, 0);
        rect.transform.GetChild(0).GetComponent<Image>().sprite = weaponImage[weaponIndex];
        rect.DOAnchorPosX(3, 1f);
    }
}
