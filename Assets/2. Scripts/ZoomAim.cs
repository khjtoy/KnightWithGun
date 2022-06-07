using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomAim : Character
{
	public Texture2D crosshair;                                           // 십사형 포인터 택스쳐
	public float aimTurnSmoothing = 0.15f;                                // 카메라의 방향과 일치하기 위해 조준 할 때 회전 반응 속도
	public Vector3 aimPivotOffset = new Vector3(0.5f, 1.2f, 0f);         // 조준 시 카메라 Pivot 설정
	public Vector3 aimCamOffset = new Vector3(0f, 0.4f, -0.7f);         // 조준 시 카메라의 Offset 설정

	private int aimBool;
	private int hashSpeed;
	private bool aim;
	Transform cameraObject;

	void Start()
	{
		aimBool = Animator.StringToHash("Aim");
		hashSpeed = Animator.StringToHash("Speed");
		cameraObject = Camera.main.transform;

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	void Update()
	{
		// 마우스 우클릭을 눌렸는가
		if (Input.GetMouseButton(1) && !aim)
		{
			StartCoroutine(ToggleAimOn());
		}
		// 마우스 우클릭 땠는가
		else if (aim && Input.GetMouseButtonUp(1))
		{
			StartCoroutine(ToggleAimOff());
		}

		// 스프린트 하지 못하게 막기
		canSprint = !aim;

		// 카메라 조준 위치를 왼쪽 또는 오른쪽으로 전환
		if (aim && Input.GetMouseButtonDown(2))
		{
			aimCamOffset.x = aimCamOffset.x * (-1);
			aimPivotOffset.x = aimPivotOffset.x * (-1);
		}

		ani.SetBool(aimBool, aim);
	}

	// 조준 모드를 지연을 통한 처리
	private IEnumerator ToggleAimOn()
	{
		yield return new WaitForSeconds(0.05f);
		// 조준 할 수 없다.
		//if (behaviourManager.GetTempLockStatus(this.behaviourCode) || behaviourManager.IsOverriding(this))
			//yield return false;

		// Start aiming.
		//else
		//{
		aim = true;
		int signal = 1;
		aimCamOffset.x = Mathf.Abs(aimCamOffset.x) * signal;
		aimPivotOffset.x = Mathf.Abs(aimPivotOffset.x) * signal;
		yield return new WaitForSeconds(0.1f);
		ani.SetFloat(hashSpeed, 0);
			//behaviourManager.GetAnim.SetFloat(speedFloat, 0);
			// This state overrides the active one.
		//behaviourManager.OverrideWithBehaviour(this);
		//}
	}

	// 조준 모드를 해제를 위한 지연처리
	private IEnumerator ToggleAimOff()
	{
		aim = false;
		yield return new WaitForSeconds(0.3f);
		cameraObject.GetComponent<CameraFollow>().ResetTargetOffsets();
		cameraObject.GetComponent<CameraFollow>().ResetMaxVerticalAngle();
		yield return new WaitForSeconds(0.05f);
		//behaviourManager.RevokeOverridingBehaviour(this);
	}

	// LocalFixedUpdate overrides the virtual function of the base class.
	public void FixedUpdate()
	{
		// 카메라 위치와 방향을 조준 모드로 설정
		if (aim)
			cameraObject.GetComponent<CameraFollow>().SetTargetOffsets(aimPivotOffset, aimCamOffset);
	}

	// 플레이어의 회전 재설정
	public void LateUpdate()
	{
		AimManagement();
	}

	// 조준이 활성 상태일 때 조준 매개변수로 처리
	void AimManagement()
	{
		// 조준할 때 플레이어 방향 고려
		Rotating();
	}

	// Rotate the player to match correct orientation, according to camera.
	void Rotating()
	{
		Vector3 forward = cameraObject.TransformDirection(Vector3.forward);
		// Player is moving on ground, Y component of camera facing is not relevant.
		forward.y = 0.0f;
		forward = forward.normalized;

		// Always rotates the player according to the camera horizontal rotation in aim mode.
		Quaternion targetRotation = Quaternion.Euler(0, cameraObject.GetComponent<CameraFollow>().GetH, 0);

		float minSpeed = Quaternion.Angle(transform.rotation, targetRotation) * aimTurnSmoothing;

		// Rotate entire player to face camera.
		SetLastDirection(forward);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, minSpeed * Time.deltaTime);

	}

	private void SetLastDirection(Vector3 direction)
	{
		lastDirection = direction;
	}

	// Draw the crosshair when aiming.
	void OnGUI()
	{
		if (crosshair)
		{
			float mag = cameraObject.GetComponent<CameraFollow>().GetCurrentPivotMagnitude(aimPivotOffset);
			if (mag < 0.05f)
				GUI.DrawTexture(new Rect(Screen.width / 2 - (crosshair.width * 0.5f),
										 Screen.height / 2 - (crosshair.height * 0.5f),
										 crosshair.width, crosshair.height), crosshair);
		}
	}
}
