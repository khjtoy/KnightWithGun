using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGizmos : MonoBehaviour
{
    public Color gizmoColor = Color.yellow;
    public float radius = 0.1f;


    private void OnDrawGizmos()
    {
        //색상 셋팅
        Gizmos.color = gizmoColor;

        //구체 기즈모 생성
        Gizmos.DrawSphere(transform.position, radius);
    }
}
