using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private float horizontal;
    private float vertical;
    // �������� ��
    private float moveAmount;

    Vector3 movementInput;

    EventParam moveParam = new EventParam();

    private void Update()
    {
        MoveInput();
        Reload();
    }

    /// <summary>
    /// �����ӿ� ���� �Է��� ���
    /// </summary>
    private void MoveInput()
    {
        movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        // X�� Z �Է�
        horizontal = movementInput.x;
        vertical = movementInput.z;

        // �������� �� ���
        // Mathf.Clamp01 -> ������ 0���� 1 ������ ��ȯ
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

        // EventManager�� ���� Setting
        moveParam.vectorParam = new Vector3(horizontal, 0, vertical);
        moveParam.intParam = (int)moveAmount;

        EventManager.TriggerEvent("PLAYER_MOVEMENT", moveParam);
    }

    private void Reload()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            EventManager.TriggerEvent("BULLET_RELOAD", new EventParam());
        }
    }
}
