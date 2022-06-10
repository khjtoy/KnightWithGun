using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiderCtrl : MonoBehaviour
{

    public enum State
    {
        IDLE,
        PATROL,
        TRACE,
        ATTACK,
        DIE
    }

    // 몬스터의 현재 상태
    public State state = State.IDLE;
    // 추적 사정거리
    public float traceDist = 10.0f;
    // 공격 사정거리
    public float attackDist = 2.0f;
    // 몬스터 사망 여부
    public bool isDie = false;

    // 컴포넌트 캐싱
    private Transform monsterTransform;
    private Transform targetTransform;
    private NavMeshAgent agent;
    private Animator anim;

    // 해시 테이블 값 가져오기
    private readonly int hashTrace = Animator.StringToHash("IsTrace");
    private readonly int hashPatrol = Animator.StringToHash("IsPatrol");
    private readonly int hashAttack = Animator.StringToHash("IsAttack");
    private readonly int hashHit = Animator.StringToHash("Hit");
    private readonly int hashDie = Animator.StringToHash("Die");

    // 몬스터의 생명 초기값
    private readonly int iniHp = 100;
    private int currHp;

    // 순찰
    [SerializeField]
    private Transform[] waypoints;
    private int waypointIndex;
    private Vector3 target;

    // Score 연결
    private void Awake()
    {
        Debug.Log("^^");
        currHp = iniHp;
        monsterTransform = GetComponent<Transform>();
        targetTransform = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();

        //자동회전 기능 비활성화
        agent.updateRotation = false;

        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        state = State.IDLE;

        currHp = iniHp;
        isDie = false;

        GetComponent<CapsuleCollider>().enabled = true;
        SphereCollider[] spheres = GetComponentsInChildren<SphereCollider>();
        foreach (SphereCollider sphere in spheres)
        {
            sphere.enabled = true;
        }

        //몬스터의 상태를 체크하는 코루틴
        StartCoroutine(CheckMonsterState());

        //상태에 따라 몬스터 행동 수행 코루틴
        StartCoroutine(MonsterAction());
    }

    private void Update()
    {
        
        // 목적지까지 남은 거리로 회전 여부 판단
        if (agent.remainingDistance >= 2.0f)
        {
            // 에이전트의 회전 값
            Vector3 direction = agent.desiredVelocity;

            // 회전 각도 산출
            Quaternion rotation = Quaternion.LookRotation(direction);

            // 구면 선형보간 함수로 부드러운 회전 처리
            monsterTransform.rotation = Quaternion.Slerp(monsterTransform.rotation, rotation, Time.deltaTime * 10.0f);
        }
        
        if(Vector3.Distance(monsterTransform.position, target) < 1)
        {
            IterateWaypointIndex();
            target = waypoints[waypointIndex].position;
        }
    }
    private IEnumerator CheckMonsterState()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.3f);

            // 몬스터 죽음 상태시 코루틴 멈춤
            if (state == State.DIE)
            {
                yield break;
            }

            // 몬스터의 캐릭터 사이의 거리 측정
            float distance = Vector3.Distance(monsterTransform.position, targetTransform.position);

            if (distance <= attackDist)
            {
                state = State.ATTACK;
            }
            else if (distance <= traceDist)
            {
                state = State.TRACE;
            }
            else
            {
                //state = State.IDLE;
                state = State.PATROL;
            }
        }
    }
    private IEnumerator MonsterAction()
    {
        while (!isDie)
        {
            switch (state)
            {
                case State.IDLE:
                    // 추적 중지
                    agent.isStopped = true;
                    anim.SetBool(hashTrace, false);
                    anim.SetBool(hashPatrol, false);
                    break;
                case State.PATROL:
                    agent.SetDestination(target);
                    agent.speed = 7f;
                    agent.isStopped = false;
                    anim.SetBool(hashPatrol, true);
                    anim.SetBool(hashTrace, false);
                    anim.SetBool(hashAttack, false);
                    break;
                case State.TRACE:
                    // 추적 대상 좌표로 이동
                    agent.SetDestination(targetTransform.position);
                    agent.speed = 10f;
                    agent.isStopped = false;
                    anim.SetBool(hashTrace, true);
                    anim.SetBool(hashAttack, false);
                    anim.SetBool(hashPatrol, false);
                    break;
                case State.ATTACK:
                    anim.SetBool(hashAttack, true);
                    break;
                case State.DIE:
                    isDie = true;
                    agent.isStopped = true;

                    anim.SetTrigger(hashDie);

                    //StopAllCoroutines();

                    GetComponent<CapsuleCollider>().enabled = false;
                    SphereCollider[] spheres = GetComponentsInChildren<SphereCollider>();
                    foreach (SphereCollider sphere in spheres)
                    {
                        sphere.enabled = false;
                    }
                    break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    void IterateWaypointIndex()
    {
        waypointIndex++;
        if(waypointIndex == waypoints.Length)
        {
            waypointIndex = 0;
        }
    }
    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("BULLET") && currHp > 0)
        {
            // 총알 삭제
            Destroy(collision.gameObject);
            // 피격 애니메이션 실행
            anim.SetTrigger(hashHit);
            // 충돌 지점
            Vector3 pos = collision.GetContact(0).point;
            // 총알의 충졸 지점의 법선 벡터
            Quaternion rot = Quaternion.LookRotation(-collision.GetContact(0).normal);
            // 혈흔 효과 생상
            ShowBloodEffect(pos, rot);

            // 몬스터 HP 차감
            currHp -= 10;
            if (currHp <= 0)
            {
                state = State.DIE;

                GameManager.GetInstance().DisPlayScore(50);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("PUNCH Collision");
    }
    
    private void ShowBloodEffect(Vector3 pos, Quaternion rot)
    {
        // 혈흔 효과 생성
        GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot, monsterTransform);
        Destroy(blood, 1.0f);
    }

    void OnPlayerDie()
    {
        state = State.PLAYERDIE;
    }
    */

    private void OnDrawGizmos()
    {
        // 추적 사정거리
        if (state == State.TRACE)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(monsterTransform.position, traceDist);
        }

        // 공격 사정거리
        if (state == State.ATTACK)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(monsterTransform.position, attackDist);
        }
    }
}
