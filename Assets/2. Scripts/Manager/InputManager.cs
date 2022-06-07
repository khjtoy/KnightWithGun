using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private float horizontal;
    private float vertical;
    // 움직임의 양
    private float moveAmount;

    Vector3 movementInput;

    EventParam moveParam = new EventParam();

    private void Update()
    {
        MoveInput();
    }

    /// <summary>
    /// 움직임에 대한 입력을 담당
    /// </summary>
    private void MoveInput()
    {
        movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        // X와 Z 입력
        horizontal = movementInput.x;
        vertical = movementInput.z;

        // 움직임의 양 계산
        // Mathf.Clamp01 -> 강제로 0에서 1 범위로 변환
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

        // EventManager를 위한 Setting
        moveParam.vectorParam = new Vector3(horizontal, 0, vertical);
        moveParam.intParam = (int)moveAmount;

        EventManager.TriggerEvent("PLAYER_MOVEMENT", moveParam);
    }
}
