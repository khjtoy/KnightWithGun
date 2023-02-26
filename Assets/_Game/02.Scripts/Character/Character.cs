using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾��� �������� �κ��� ����
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
