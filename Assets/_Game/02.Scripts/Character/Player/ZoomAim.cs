using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomAim : Character
{
	[SerializeField]
	private Texture2D crosshair;                                           // 십사형 포인터 택스쳐
	[SerializeField]
	private float aimTurnSmoothing = 0.15f;                                // 카메라의 방향과 일치하기 위해 조준 할 때 회전 반응 속도
	[SerializeField]
	private Vector3 aimPivotOffset = new Vector3(0.5f, 1.2f, 0f);         // 조준 시 카메라 Pivot 설정
	[SerializeField]
	private Vector3 aimCamOffset = new Vector3(0f, 0.4f, -0.7f);         // 조준 시 카메라의 Offset 설정

	private readonly int aimBool = Animator.StringToHash("Aim");
	private readonly int hashSpeed = Animator.StringToHash("Speed");
	private readonly int hashPeach = Animator.StringToHash("Peach");
	private bool aim;
	Transform cameraObject;

	RayCastWeapon weapon;

	PlayerWeapon playerWeapon;

	private PlayerAttack playerAttack;

	[SerializeField]
	private float time = 0.1f;


	private void OnEnable()
	{
		if(aim)
        {
			StartCoroutine(ToggleAimOff());
		}
	}
	void Start()
	{
		cameraObject = Camera.main.transform;

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		weapon = GetComponentInChildren<RayCastWeapon>();
		playerAttack = GetComponent<PlayerAttack>();
		playerWeapon = GetComponent<PlayerWeapon>();
	}

	void Update()
	{
		ani.SetFloat(hashPeach,(cameraObject.GetComponent<CameraFollow>().GetPitch()) / 50.0f);
		// 마우스 우클릭을 눌렸는가
		if (Input.GetMouseButton(1) && !aim && !playerAttack.IsAttack && playerWeapon.weaponIndex == 0)
		{
			StartCoroutine(ToggleAimOn());
		}
		// 마우스 우클릭 땠는가
		else if (aim && Input.GetMouseButtonUp(1))
		{
			StartCoroutine(ToggleAimOff());
		}

		// 스프린트 하지 못하게 막기
		//canSprint = !aim;

		// 카메라 조준 위치를 왼쪽 또는 오른쪽으로 전환
		if (aim && Input.GetKeyDown(KeyCode.G))
		{
			aimCamOffset.x = aimCamOffset.x * (-1);
			aimPivotOffset.x = aimPivotOffset.x * (-1);
		}

		if (aim)
		{
			time -= Time.deltaTime;
			if (Input.GetMouseButton(0) && time <= 0f)
			{
				weapon.StartFiring();
				time = 0.1f;
			}			

			if (Input.GetMouseButtonUp(0))
			{
				weapon.StopFiring();
				time = 0.1f;
			}
		}
		ani.SetBool(aimBool, aim);
	}

	public bool isAim()
    {
		return aim;
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
		//weapon.gameObject.SetActive(true);
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
		yield return new WaitForSeconds(0.05f);
		cameraObject.GetComponent<CameraFollow>().ResetTargetOffsets();
		cameraObject.GetComponent<CameraFollow>().ResetMaxVerticalAngle();
		yield return new WaitForSeconds(0.05f);
	}

	
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

	void Rotating()
	{
		Vector3 forward = cameraObject.TransformDirection(Vector3.forward);
		forward.y = 0.0f;
		forward = forward.normalized;


		Quaternion targetRotation = Quaternion.Euler(0, cameraObject.GetComponent<CameraFollow>().GetH, 0);

		float minSpeed = Quaternion.Angle(transform.rotation, targetRotation) * aimTurnSmoothing;

		
		SetLastDirection(forward);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, minSpeed * Time.deltaTime);

	}

	private void SetLastDirection(Vector3 direction)
	{
		lastDirection = direction;
	}

	// 조준점 그리기
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
