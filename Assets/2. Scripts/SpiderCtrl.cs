using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class SpiderCtrl : Monster
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
    public int waypointIndex;
    private Vector3 target;

    // 시야각
    [SerializeField]
    private float viewAngle;
    [SerializeField]
    private float viewDistance;
    [SerializeField]
    private HealthBarUI healthBarUI;
    Vector3 targetDir = Vector3.zero;
    float dotProduct;
    float theta;

    private Rigidbody rigidbody;

    [SerializeField]
    private OpaqueItem opaqueItem;


    [SerializeField]
    private CapsuleCollider damageCol;

    [SerializeField]
    private GameObject floatingTextPrefab;

    // 혈흔 효과 프리팹
    private GameObject bloodEffect;
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
        rigidbody = GetComponent<Rigidbody>();

        // BloodESprayEffect 프리팹 로드
        bloodEffect = Resources.Load<GameObject>("BloodSprayFX");
    }

    private void ShowBloodEffect(Vector3 pos, Quaternion rot)
    {
        // 혈흔 효과 생성
        GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot, monsterTransform);
        Destroy(blood, 1.0f);
    }

    private void OnEnable()
    {
        state = State.IDLE;

        currHp = iniHp;
        isDie = false;

        GetComponent<CapsuleCollider>().enabled = true;

        //몬스터의 상태를 체크하는 코루틴
        StartCoroutine(CheckMonsterState());

        //상태에 따라 몬스터 행동 수행 코루틴
        StartCoroutine(MonsterAction());
    }

    private void Start()
    {

        // Item 지정
        opaqueItem = GameObject.FindGameObjectWithTag("ITEM").GetComponent<OpaqueItem>();

        // Patrol 랜덤 위치 변환
        
        for(int i = 0; i < 100; i++)
        {
            int num1 = Random.Range(0, waypoints.Length);
            int num2 = Random.Range(0, waypoints.Length);

            Transform temp = waypoints[num1];
            waypoints[num1] = waypoints[num2];
            waypoints[num2] = temp;

        }

        //랜덤 위치로 생성
    }

    private void Update()
    {
        
        // 목적지까지 남은 거리로 회전 여부 판단
        if (agent.remainingDistance >= 6f)
        {
            // 에이전트의 회전 값
            Vector3 direction = agent.desiredVelocity;

            // 회전 각도 산출
            Quaternion rotation = Quaternion.LookRotation(direction);

            // 구면 선형보간 함수로 부드러운 회전 처리
            monsterTransform.rotation = Quaternion.Slerp(monsterTransform.rotation, rotation, Time.deltaTime * 10.0f);
        }
        
        // Patrol 지정
        if(Vector3.Distance(monsterTransform.position, target) < 7f && state == State.PATROL)
        {
            Debug.Log("지정");
            IterateWaypointIndex();
            target = waypoints[waypointIndex].position;
        }

        // 시야각
        Vector3 leftBoundary = DirFromAngle(-viewAngle / 2);
        Vector3 rightBoundary = DirFromAngle(viewAngle / 2);
        Debug.DrawLine(transform.position, transform.position + leftBoundary * viewDistance, Color.blue);
        Debug.DrawLine(transform.position, transform.position + rightBoundary * viewDistance, Color.blue);

        targetDir = (targetTransform.position - monsterTransform.position).normalized;
        dotProduct = Vector3.Dot(transform.forward.normalized, targetDir);
        theta = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;
    }

    public Vector3 DirFromAngle(float angleInDegrees)
    {
        angleInDegrees += monsterTransform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
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

            if(opaqueItem.isOpaque)
            {
                state = State.PATROL;
            }
            else if (distance <= attackDist)
            {
                state = State.ATTACK;
            }
            else if(theta <= viewAngle / 2 && distance <= traceDist)
            {
                state = State.TRACE;
            }
            /*
            else if (distance <= traceDist)
            {
                state = State.TRACE;
            }
            */
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
                    damageCol.enabled = false;
                    agent.isStopped = true;
                    anim.SetBool(hashTrace, false);
                    anim.SetBool(hashPatrol, false);
                    break;
                case State.PATROL:
                    damageCol.enabled = false;
                    agent.SetDestination(target);
                    agent.speed = 7f;
                    agent.isStopped = false;
                    anim.SetBool(hashPatrol, true);
                    anim.SetBool(hashTrace, false);
                    anim.SetBool(hashAttack, false);
                    break;
                case State.TRACE:
                    // 추적 대상 좌표로 이동
                    damageCol.enabled = false;
                    agent.SetDestination(targetTransform.position);
                    agent.speed = 10f;
                    agent.isStopped = false;
                    anim.SetBool(hashTrace, true);
                    anim.SetBool(hashAttack, false);
                    anim.SetBool(hashPatrol, false);
                    break;
                case State.ATTACK:
                    damageCol.enabled = true;
                    anim.SetBool(hashAttack, true);
                    break;
                case State.DIE:
                    isDie = true;
                    agent.isStopped = true;
                    anim.SetTrigger(hashDie);

                    //StopAllCoroutines();

                    //GetComponent<CapsuleCollider>().enabled = false;
                    damageCol.enabled = false;
                    healthBarUI.gameObject.SetActive(false);
                    rigidbody.isKinematic = false;
                    break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void RandomPos()
    {
        // wayPoint 지정
        GameObject wayPoint = GameObject.FindGameObjectWithTag("PATROL");

        waypoints = new Transform[20];
        for (int i = 0; i < 20; i++)
        {
            waypoints[i] = wayPoint.transform.GetChild(i);
        }

        int range = Random.Range(0, waypoints.Length);
        transform.position = waypoints[range].position;
    }

    void IterateWaypointIndex()
    {
        waypointIndex++;
        if(waypointIndex == waypoints.Length)
        {
            waypointIndex = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GUN"))
        {
            MonsterHit(monsterTransform.position, monsterTransform.rotation.eulerAngles, 40);
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

    public override void MonsterHit(Vector3 bloodPos, Vector3 bloodRot, int damage)
    {
        if(currHp > 0)
        {
            // 피격 애니메이션 실행
            anim.SetTrigger(hashHit);

            // 몬스터 HP 차감
            currHp -= damage;
            healthBarUI.ChangeHP(currHp, iniHp);

            // 총알의 충졸 지점의 법선 벡터
            Quaternion rot = Quaternion.LookRotation(bloodRot);
            // 혈흔 효과 생상
            ShowBloodEffect(bloodPos, rot);

            if(floatingTextPrefab)
            {
                ShowFloatingText();
            }

            if (currHp <= 0)
            {
                anim.SetBool("IsDie", true);
                anim.SetBool(hashTrace, false);
                anim.SetBool(hashAttack, false);
                anim.SetBool(hashPatrol, false);
                state = State.DIE;
            }
        }
    }

    private void ShowFloatingText()
    {
        var go = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity, transform);
        go.GetComponent<TextMeshPro>().text = currHp.ToString();
    }
}
