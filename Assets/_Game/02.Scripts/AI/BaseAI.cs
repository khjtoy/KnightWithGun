using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// AI 상태 정의
public enum eAIStateType
{
    AI_STATE_NONE,
    AI_STATE_IDLE,
    AI_STATE_PATROL,
    AI_STATE_TRACE,
    AI_STATE_ATTACK,
    AI_STATE_SLAM,
    AI_STATE_DIE
}

// AI 종류들
public enum eAIType
{
    CACTUS_AI,
    SPIDER_AI,
}

// 바꿀 AI의 값들
public struct stNextAI
{
    public eAIStateType m_StateType;
    public GameObject m_TargetObject;
    public Vector3 m_Position;
}

public class BaseAI : MonoBehaviour
{
    protected List<stNextAI> m_listNextAI = new List<stNextAI>();
    [SerializeField]
    protected eAIStateType m_CurrentAIState = eAIStateType.AI_STATE_IDLE;



}
