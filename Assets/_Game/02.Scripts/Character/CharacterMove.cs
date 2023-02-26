using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 캐릭터의 움직임을 담당
/// </summary>
public class CharacterMove : Character
{
    //EventParam으로 부터 받을 변수들
    private int inputX;
    private int inputZ;

    Transform cameraObject;
    Vector3 moveDirection;


    [Header("Stats")]
    [SerializeField]
    private float movementSpeed; //이동 스피드
    [SerializeField]
    private float sprintSpeed; //스프린트 스피드
    [SerializeField]
    private Transform playerCamera;
    [SerializeField]
    private float turnSmoothing = 0.06f;

    // 해시 테이블 값 가져오기
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashMoving = Animator.StringToHash("IsMove");

    private PlayerAttack playerAttack;

    private void Start()
    {
        EventManager.StartListening("PLAYER_MOVEMENT", SetMovement);
        cameraObject = Camera.main.transform;
        playerAttack = GetComponent<PlayerAttack>();
    }

    private void Update()
    {
        if (!playerAttack.IsAttack)
            Movement();
        else
            rigid.velocity = Vector3.zero;
    }

    Vector3 normalVector;
    private void SetMovement(EventParam eventParam)
    {
        inputX = (int)eventParam.vectorParam.x;
        inputZ = (int)eventParam.vectorParam.z;
    }

    private void Movement()
    {
        //캐릭터 앞(inputZ = 1) 또는 뒤(inputZ = -1)를 vector에 저장
        moveDirection = cameraObject.forward * inputZ;
        //캐릭터 오른쪽(inputZ = 1) 또는 왼쪽(inputZ = -1)를 vector에 더함
        moveDirection += cameraObject.right * inputX;

        moveDirection.Normalize();

        Rotating(inputX, inputZ);

        if (moveDirection.sqrMagnitude > 0)
        {
            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && !GetComponent<ZoomAim>().isAim() && !GetComponent<PlayerStats>().GetThirst())
            {
                moveDirection *= sprintSpeed;
                ani.SetFloat(hashSpeed, sprintSpeed);
            }
            else
            {
                moveDirection *= movementSpeed;
                ani.SetFloat(hashSpeed, movementSpeed);
            }

            ani.SetBool(hashMoving, true);
        }
        else
            ani.SetBool(hashMoving, false);

        //normalVector의 법선 평면으로부터 플레이어가 움직이려는 방향벡터로 투영
        Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
        //이동
        rigid.velocity = projectedVelocity;
        //PlayerAnim(inputX, inputZ);

        //transform.LookAt(transform.position + moveDirection);

    }

    Vector3 Rotating(float horizontal, float vertical)
    {
        Vector3 forward = playerCamera.TransformDirection(Vector3.forward);

        forward.y = 0.0f;
        forward = forward.normalized;

        Vector3 right = new Vector3(forward.z, 0, -forward.x);
        Vector3 targetDirection;
        targetDirection = forward * vertical + right * horizontal;

        if ((IsMoving() && targetDirection != Vector3.zero))
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            Quaternion newRotation = Quaternion.Slerp(rigid.rotation, targetRotation, turnSmoothing);
            rigid.MoveRotation(newRotation);
            SetLastDirection(targetDirection);
        }

        if (!(Mathf.Abs(horizontal) > 0.9 || Mathf.Abs(vertical) > 0.9))
        {
            Repositioning();
        }

        return targetDirection;
    }

    public bool IsMoving()
    {
        return (inputX != 0) || (inputZ != 0);
    }

    private void SetLastDirection(Vector3 direction)
    {
        lastDirection = direction;
    }

    private void Repositioning()
    {
        if (lastDirection != Vector3.zero)
        {
            lastDirection.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(lastDirection);
            Quaternion newRotation = Quaternion.Slerp(rigid.rotation, targetRotation, turnSmoothing);
            rigid.MoveRotation(newRotation);
        }
    }

    private void OnDestroy()
    {
        EventManager.StopListening("PLAYER_MOVEMENT", SetMovement);
    }

    private void OnApplicationQuit()
    {
        EventManager.StopListening("PLAYER_MOVEMENT", SetMovement);
    }
}
