using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField]
    private float destoryTime = 3f;
    [SerializeField]
    private Vector3 offset;

    private void Start()
    {
        Destroy(gameObject, destoryTime);

        transform.localPosition += offset;
    }
}
