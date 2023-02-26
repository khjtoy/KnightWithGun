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

    // ������ ���� ����
    public State state = State.IDLE;
    // ���� �����Ÿ�
    public float traceDist = 10.0f;
    // ���� �����Ÿ�
    public float attackDist = 2.0f;
    // ���� ��� ����
    public bool isDie = false;

    // ������Ʈ ĳ��
    private Transform monsterTransform;
    private Transform targetTransform;
    private NavMeshAgent agent;
    private Animator anim;

    // �ؽ� ���̺� �� ��������
    private readonly int hashTrace = Animator.StringToHash("IsTrace");
    private readonly int hashPatrol = Animator.StringToHash("IsPatrol");
    private readonly int hashAttack = Animator.StringToHash("IsAttack");
    private readonly int hashHit = Animator.StringToHash("Hit");
    private readonly int hashDie = Animator.StringToHash("Die");

    // ������ ���� �ʱⰪ
    private readonly int iniHp = 100;
    private int currHp;

    // ����
    [SerializeField]
    private Transform[] waypoints;
    public int waypointIndex;
    [SerializeField]
    private Vector3 target;

    // �þ߰�
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

    // ���� ȿ�� ������
    private GameObject bloodEffect;

    private void Awake()
    {
        Debug.Log("^^");
        currHp = iniHp;
        monsterTransform = GetComponent<Transform>();
        targetTransform = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();

        //�ڵ�ȸ�� ��� ��Ȱ��ȭ
        agent.updateRotation = false;

        anim = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();

        // BloodESprayEffect ������ �ε�
        bloodEffect = Resources.Load<GameObject>("BloodSprayFX");
    }

    private void ShowBloodEffect(Vector3 pos, Quaternion rot)
    {
        // ���� ȿ�� ����
        GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot, monsterTransform);
        Destroy(blood, 1.0f);
    }

    private void OnEnable()
    {
        state = State.IDLE;

        currHp = iniHp;
        isDie = false;

        GetComponent<CapsuleCollider>().enabled = true;

        //������ ���¸� üũ�ϴ� �ڷ�ƾ
        StartCoroutine(CheckMonsterState());

        //���¿� ���� ���� �ൿ ���� �ڷ�ƾ
        StartCoroutine(MonsterAction());
    }

    private void Start()
    {

        // Item ����
        opaqueItem = GameObject.FindGameObjectWithTag("ITEM").GetComponent<OpaqueItem>();

        // Patrol ���� ��ġ ��ȯ
        
        for(int i = 0; i < 100; i++)
        {
            int num1 = Random.Range(0, waypoints.Length);
            int num2 = Random.Range(0, waypoints.Length);

            Transform temp = waypoints[num1];
            waypoints[num1] = waypoints[num2];
            waypoints[num2] = temp;

        }

        //���� ��ġ�� ����
    }

    private void Update()
    {
        
        // ���������� ���� �Ÿ��� ȸ�� ���� �Ǵ�
        if (agent.remainingDistance >= 6f)
        {
            // ������Ʈ�� ȸ�� ��
            Vector3 direction = agent.desiredVelocity;

            // ȸ�� ���� ����
            Quaternion rotation = Quaternion.LookRotation(direction);

            // ���� �������� �Լ��� �ε巯�� ȸ�� ó��
            monsterTransform.rotation = Quaternion.Slerp(monsterTransform.rotation, rotation, Time.deltaTime * 10.0f);
        }
        
        // Patrol ����
        if(Vector3.Distance(monsterTransform.position, target) < 7f && state == State.PATROL)
        {
            Debug.Log("����");
            IterateWaypointIndex();
            target = waypoints[waypointIndex].position;
        }

        // �þ߰�
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

            // ���� ���� ���½� �ڷ�ƾ ����
            if (state == State.DIE)
            {
                yield break;
            }

            // ������ ĳ���� ������ �Ÿ� ����
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
                    // ���� ����
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
                    // ���� ��� ��ǥ�� �̵�
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
        // wayPoint ����
        GameObject wayPoint = GameObject.FindGameObjectWithTag("PATROL");

        waypoints = new Transform[16];
        for (int i = 0; i < 16; i++)
        {
            waypoints[i] = wayPoint.transform.GetChild(i);
        }

        int range = Random.Range(0, waypoints.Length);
        transform.position = waypoints[range].position;

        range = Random.Range(0, waypoints.Length);
        target = waypoints[range].position;
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
            // �Ѿ� ����
            Destroy(collision.gameObject);
            // �ǰ� �ִϸ��̼� ����
            anim.SetTrigger(hashHit);
            // �浹 ����
            Vector3 pos = collision.GetContact(0).point;
            // �Ѿ��� ���� ������ ���� ����
            Quaternion rot = Quaternion.LookRotation(-collision.GetContact(0).normal);
            // ���� ȿ�� ����
            ShowBloodEffect(pos, rot);

            // ���� HP ����
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
        // ���� ȿ�� ����
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
        
        // ���� �����Ÿ�
        if (state == State.TRACE)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(monsterTransform.position, traceDist);
        }
        
        // ���� �����Ÿ�
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
            // �ǰ� �ִϸ��̼� ����
            anim.SetTrigger(hashHit);

            // ���� HP ����
            currHp -= damage;
            healthBarUI.ChangeHP(currHp, iniHp);

            if (bloodPos != new Vector3(100, -100, 100))
            {
                // �Ѿ��� ���� ������ ���� ����
                Quaternion rot = Quaternion.LookRotation(bloodRot);
                // ���� ȿ�� ����
                ShowBloodEffect(bloodPos, rot);
            }
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
        Vector3 l_vector = targetTransform.position - transform.position;
        Quaternion temp = Quaternion.LookRotation(-l_vector).normalized;
        var go = Instantiate(floatingTextPrefab, transform.position, temp, transform);

        //go.transform.LookAt(targetTransform);


        //go.transform.rotation = Quaternion.Euler(go.transform.rotation.x, -go.transform.rotation.y, go.transform.rotation.z);

        go.GetComponent<TextMeshPro>().text = currHp.ToString();
    }
}
