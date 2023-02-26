using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetItem : MonoBehaviour
{
    // 0 : Hp 1 : Water 2 : Behind
    [SerializeField]
    private int itemNumber;

    public int GetNumber()
    {
        return itemNumber;
    }
}
