using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ĳ������ �������� ���
/// </summary>
public class CharacterMove : Character
{
    //EventParam���� ���� ���� ������
    private int inputX;
    private int inputZ;
    private int inputAmount;

    Transform cameraObject;
    Vector3 moveDirection;


    [Header("Stats")]
    [SerializeField]
    private float movementSpeed; //�̵� ���ǵ�
    [SerializeField]
    private float sprintSpeed; //������Ʈ ���ǵ�
    [SerializeField]
    private Transform playerCamera;
    [SerializeField]
    private float turnSmoothing = 0.06f;
    // �ؽ� ���̺� �� ��������
    // private readonly int hashRunL = Animator.StringToHash("RunL");
    // private readonly int hashRunR = Animator.StringToHash("RunR");
    // private readonly int hashRunF = Animator.StringToHash("RunF");
    //private readonly int hashRunB = Animator.StringToHash("RunB");

    private void Start()
    {
        EventManager.StartListening("PLAYER_MOVEMENT", SetMovement);
        cameraObject = Camera.main.transform;
    }

    private void Update()
    {
        Movement();
    }

    Vector3 normalVector;
    private void SetMovement(EventParam eventParam)
    {
        inputX = (int)eventParam.vectorParam.x;
        inputZ = (int)eventParam.vectorParam.z;
        inputAmount = eventParam.intParam;
        //Debug.Log("���̿�");
    }

    private void Movement()
    {
        //ĳ���� ��(inputZ = 1) �Ǵ� ��(inputZ = -1)�� vector�� ����
        moveDirection = cameraObject.forward * inputZ;
        //ĳ���� ������(inputZ = 1) �Ǵ� ����(inputZ = -1)�� vector�� ����
        moveDirection += cameraObject.right * inputX;

        moveDirection.Normalize();

        Rotating(inputX, inputZ);

        if (moveDirection.sqrMagnitude > 0)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) && canSprint)
            {
                moveDirection *= sprintSpeed;
                ani.SetFloat("Speed", sprintSpeed);
            }
            else
            {
                moveDirection *= movementSpeed;
                ani.SetFloat("Speed", movementSpeed);
            }

            ani.SetBool("IsMove", true);
        }
        else
            ani.SetBool("IsMove", false);

        //normalVector�� ���� ������κ��� �÷��̾ �����̷��� ���⺤�ͷ� ����
        Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
        //�̵�
        rigid.velocity = projectedVelocity;
        //PlayerAnim(inputX, inputZ);

        //transform.LookAt(transform.position + moveDirection);

    }

    void PlayerAnim(int h, int v)
    {
        ani.SetFloat("X", h);
        ani.SetFloat("Z", v);
    }

    Vector3 Rotating(float horizontal, float vertical)
    {
        Vector3 forward = playerCamera.TransformDirection(Vector3.forward);

        forward.y = 0.0f;
        forward = forward.normalized;

        Vector3 right = new Vector3(forward.z, 0, -forward.x);
        Vector3 targetDirection;
        targetDirection = forward * vertical + right * horizontal;

        // Lerp current direction to calculated target direction.
        if ((IsMoving() && targetDirection != Vector3.zero))
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            Quaternion newRotation = Quaternion.Slerp(rigid.rotation, targetRotation, turnSmoothing);
            rigid.MoveRotation(newRotation);
            SetLastDirection(targetDirection);
        }
        // If idle, Ignore current camera facing and consider last moving direction.
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