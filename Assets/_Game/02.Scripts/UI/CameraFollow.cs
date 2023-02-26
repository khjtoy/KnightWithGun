using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public Transform player;                                           // Player�� Transform
	public Vector3 pivotOffset = new Vector3(0.0f, 1.7f, 0.0f);        // ī�޶� ����Ű�� ���� Offset
	public Vector3 camOffset = new Vector3(0.0f, 0.0f, -3.0f);		   // �÷��̾��� ��ġ�� ���õ� ī�޶� ���ġ�ϴ� Offset
	public float smooth = 10f;                                         // ī�޶� ������� �ӵ�
	public float horizontalAimingSpeed = 6f;                           // ���� ���� ȸ�� �ӵ�
	public float verticalAimingSpeed = 6f;                             // ���� ���� ȸ�� �ӵ�
	public float maxVerticalAngle = 30f;                               // ���� �ִ� ����
	public float minVerticalAngle = -60f;                              // ���� �ּ� ����

	public float zoomMaxVerAngle;									   //Zoom �� �� �ִ� ����
	public float zoomMinVerAngle;                                      //Zoom �� �� �ּ� ����

	private float angleH = 0;                                          // ���콺 �̵��� ���� ���� ����
	private float angleV = 0;                                          // ���콺 �̵��� ���� ���� ����
	private Transform cam;                                             // �ش� ��ũ��Ʈ�� Transform
	private Vector3 smoothPivotOffset;                                 // ���� �� ���� ī�޶��� Pivot Offset�� ����
	private Vector3 smoothCamOffset;                                   // ���� �� ���� ī�޶��� Offset�� ����
	private Vector3 targetPivotOffset;                                 // Ÿ���� Pivot Offset
	private Vector3 targetCamOffset;                                   // ���� �� ī�޶��� Offset
	private float defaultFOV;                                          // �⺻ ī�޶��� �þ�
	private float targetFOV;                                           // Ÿ�� ī�޶��� �þ�
	private float targetMaxVerticalAngle;                              // ����� ���� �ִ� ���� ī�޶� ����
	private bool isCustomOffset;                                       // ����� ���� ī�޶� ������ ��� ����
	[SerializeField]
	private Transform cameraRot; // ī�޶� ȸ���� �� Transform;
	[SerializeField]
	private ZoomAim zoomAim;

	// ī�޶� ���� ���� ������Ƽ
	public float GetH { get { return angleH; } }
	public LayerMask layer;

	void Awake()
	{
		cam = transform;

		cam.position = player.position + Quaternion.identity * pivotOffset + Quaternion.identity * camOffset;
		cameraRot.rotation = Quaternion.identity;

		smoothPivotOffset = pivotOffset;
		smoothCamOffset = camOffset;
		defaultFOV = cam.GetComponent<Camera>().fieldOfView;
		angleH = player.eulerAngles.y;

		ResetTargetOffsets();
		ResetFOV();
		ResetMaxVerticalAngle();

		if (camOffset.y > 0)
			Debug.LogWarning("Vertical Cam Offset (Y) will be ignored during collisions!\n" +
				"It is recommended to set all vertical offset in Pivot Offset.");
	}


    void Update()
	{
		angleH += Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1) * horizontalAimingSpeed;
		angleV += Mathf.Clamp(Input.GetAxis("Mouse Y"), -1, 1) * verticalAimingSpeed;

		//if (zoomAim.isAim())
			//angleV = Mathf.Clamp(angleV, zoomMinVerAngle, zoomMaxVerAngle);
		//else
		angleV = Mathf.Clamp(angleV, minVerticalAngle, targetMaxVerticalAngle);
		
		Quaternion camYRotation = Quaternion.Euler(0, angleH, 0);
		Quaternion aimRotation = Quaternion.Euler(-angleV, angleH, 0);
		cameraRot.rotation = aimRotation;

		cam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(cam.GetComponent<Camera>().fieldOfView, targetFOV, Time.deltaTime);

		Vector3 baseTempPosition = player.position + camYRotation * targetPivotOffset;
		Vector3 noCollisionOffset = targetCamOffset;
		while (noCollisionOffset.magnitude >= 0.2f)
		{
			if (DoubleViewingPosCheck(baseTempPosition + aimRotation * noCollisionOffset))
				break;
			noCollisionOffset -= noCollisionOffset.normalized * 0.2f;
		}
		if (noCollisionOffset.magnitude < 0.2f)
			noCollisionOffset = Vector3.zero;

		bool customOffsetCollision = isCustomOffset && noCollisionOffset.sqrMagnitude < targetCamOffset.sqrMagnitude;

		smoothPivotOffset = Vector3.Lerp(smoothPivotOffset, customOffsetCollision ? pivotOffset : targetPivotOffset, smooth * Time.deltaTime);
		smoothCamOffset = Vector3.Lerp(smoothCamOffset, customOffsetCollision ? Vector3.zero : noCollisionOffset, smooth * Time.deltaTime);

		cam.position = player.position + camYRotation * smoothPivotOffset + aimRotation * smoothCamOffset;
	}

	public void SetTargetOffsets(Vector3 newPivotOffset, Vector3 newCamOffset)
	{
		targetPivotOffset = newPivotOffset;
		targetCamOffset = newCamOffset;
		isCustomOffset = true;
	}

	public void ResetTargetOffsets()
	{
		targetPivotOffset = pivotOffset;
		targetCamOffset = camOffset;
		isCustomOffset = false;
	}

	public void ResetYCamOffset()
	{
		targetCamOffset.y = camOffset.y;
	}

	public void SetYCamOffset(float y)
	{
		targetCamOffset.y = y;
	}

	public void SetXCamOffset(float x)
	{
		targetCamOffset.x = x;
	}

	public void SetFOV(float customFOV)
	{
		this.targetFOV = customFOV;
	}

	public void ResetFOV()
	{
		this.targetFOV = defaultFOV;
	}

	public float GetPitch()
    {
		return angleV;
    }

	public void SetMaxVerticalAngle(float angle)
	{
		this.targetMaxVerticalAngle = angle;
	}

	public void ResetMaxVerticalAngle()
	{
		this.targetMaxVerticalAngle = maxVerticalAngle;
	}

	bool DoubleViewingPosCheck(Vector3 checkPos)
	{
		return ViewingPosCheck(checkPos) && ReverseViewingPosCheck(checkPos);
	}

	bool ViewingPosCheck(Vector3 checkPos)
	{
		Vector3 target = player.position + pivotOffset;
		Vector3 direction = target - checkPos;
		if (Physics.SphereCast(checkPos, 0.1f, direction, out RaycastHit hit, direction.magnitude))
		{
			if (hit.transform != player && !hit.transform.GetComponent<Collider>().isTrigger)
			{
				return false;
			}
		}
		return true;
	}

	bool ReverseViewingPosCheck(Vector3 checkPos)
	{
		// Cast origin and direction.
		Vector3 origin = player.position + pivotOffset;
		Vector3 direction = checkPos - origin;
		if (Physics.SphereCast(origin, 0.2f, direction, out RaycastHit hit, direction.magnitude, layer))
		{
			if (hit.transform != player && hit.transform != transform && !hit.transform.GetComponent<Collider>().isTrigger)
			{
				return false;
			}
		}
		return true;
	}

	public float GetCurrentPivotMagnitude(Vector3 finalPivotOffset)
	{
		return Mathf.Abs((finalPivotOffset - smoothPivotOffset).magnitude);
	}
}
