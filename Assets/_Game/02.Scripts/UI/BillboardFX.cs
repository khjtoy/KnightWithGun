using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardFX : MonoBehaviour
{
    private Transform camTransform;

    Quaternion originRotation;

    private void Start()
    {
        originRotation = transform.rotation;
        camTransform = Camera.main.transform;
    }

    private void Update()
    {
        Quaternion q = Quaternion.identity;
        q.y = camTransform.rotation.y;
        transform.rotation = q;
    }
}
