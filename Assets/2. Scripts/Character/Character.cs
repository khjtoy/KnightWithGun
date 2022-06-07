using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 공통적인 부분을 추출
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
//[RequireComponent(typeof(Collider))]
public class Character : MonoBehaviour
{
	protected Rigidbody rigid = null;
	protected Animator ani = null;
	protected bool canSprint;
	protected Vector3 lastDirection;

	protected virtual void Awake()
	{
		rigid = GetComponent<Rigidbody>();
		ani = GetComponent<Animator>();
		canSprint = true;
	}
}
