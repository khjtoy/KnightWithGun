using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class DogKnightCtrl : Monster
{
    //몬스터 상태정보

    public enum State
    {
        IDLE,
        TRACE,
        COMBACK,
        ATTACK,
        DIE,
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
    private readonly int hashAttack = Animator.StringToHash("IsAttack");
    private readonly int hashComback = Animator.StringToHash("IsComback");
    private readonly int hashHit = Animator.StringToHash("Hit");
    private readonly int hashDie = Animator.StringToHash("DIE");

    // 몬스터의 생명 초기값
    private readonly int iniHp = 500;
    private int currHp;

    [SerializeField]
    private Transform returnPos;

    [SerializeField]
    private GameObject meleeEffect;

    [SerializeField]
    private HealthBarUI healthBarUI;

    EventParam eventParam;

    private bool isComback = false;

    [SerializeField]
    private BoxCollider attackColider;

    // Score 연결
    private void Awake()
    {
        currHp = iniHp;
        monsterTransform = GetComponent<Transform>();
        targetTransform = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();

        //자동회전 기능 비활성화
        agent.updateRotation = false;

        anim = GetComponent<Animator>();

        EventManager.StartListening("COMEBACK", ChangeComback);

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
        if (agent.remainingDistance >= 7.0f)
        {
            // 에이전트의 회전 값
            Vector3 direction = agent.desiredVelocity;

            // 회전 각도 산출
            Quaternion rotation = Quaternion.LookRotation(direction);

            // 구면 선형보간 함수로 부드러운 회전 처리
            monsterTransform.rotation = Quaternion.Slerp(monsterTransform.rotation, rotation, Time.deltaTime * 10.0f);
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

            if (!isComback)
            {
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
                    state = State.IDLE;
                }
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
                    break;
                case State.TRACE:
                    // 추적 대상 좌표로 이동
                    agent.SetDestination(targetTransform.position);
                    agent.isStopped = false;
                    anim.SetBool(hashTrace, true);
                    anim.SetBool(hashAttack, false);
                    break;
                case State.COMBACK:
                        agent.isStopped = false;
                        anim.SetBool(hashTrace, false);
                        anim.SetBool(hashAttack, false);
                        anim.SetBool(hashComback, true);

                        transform.DOMove(returnPos.position, 3f).OnComplete(() =>
                        {
                            isComback = false;
                            anim.SetBool(hashComback, false);
                        });
                    break;
                case State.ATTACK:
                    if(meleeEffect != null)
                    {
                        meleeEffect.SetActive(true);
                    }
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

    public void OffParticle()
    {
        meleeEffect.SetActive(false);
    }
    private void ChangeComback(EventParam eventParam)
    {
        Debug.Log("제발");
        isComback = true;
        state = State.COMBACK;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("RANGE"))
        {
            Debug.Log("복귀");
            EventManager.TriggerEvent("COMEBACK", eventParam);
        }
    }

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

    private void OnDestroy()
    {
        EventManager.StopListening("COMBACK", ChangeComback);
    }

    private void OnApplicationQuit()
    {
        EventManager.StopListening("COMBACK", ChangeComback);
    }

    public void OnAttackCoilder()
    {
        attackColider.enabled = true;
    }

    public void OffAttackColider()
    {
        attackColider.enabled = false;
    }

    public override void MonsterHit(Vector3 bloodPos, Vector3 bloodRot, int damage)
    {
        if (currHp > 0)
        {
            // 피격 애니메이션 실행
            anim.SetTrigger(hashHit);

            // 몬스터 HP 차감
            currHp -= damage;
            healthBarUI.ChangeHP(currHp, iniHp);

            if (currHp <= 0)
            {
                anim.SetBool(hashDie, true);
                anim.SetBool(hashTrace, false);
                anim.SetBool(hashAttack, false);
                state = State.DIE;
            }
        }
    }
}
