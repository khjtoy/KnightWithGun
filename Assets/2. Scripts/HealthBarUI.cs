using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField]
    private Image helathBar;
   public void ChangeHP(int hp, int maxHp)
    {
        if (!helathBar.transform.parent.gameObject.activeSelf && hp > 0)
            helathBar.transform.parent.gameObject.SetActive(true);

        //Debug.Log((float)hp / maxHp);
        helathBar.fillAmount = (float)hp / maxHp;
    }
}
