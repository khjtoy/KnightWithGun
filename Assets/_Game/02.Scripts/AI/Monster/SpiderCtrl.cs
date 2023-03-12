using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class SpiderCtrl : Monster
{
    [SerializeField]
    private CapsuleCollider damageCol;
    public CapsuleCollider DamageCol => damageCol;

    // ������Ʈ ĳ��
    private Transform monsterTransform;
    private Transform targetTransform;
    private NavMeshAgent agent;
    private Animator anim;

    public Transform MonsterTransform => monsterTransform;
    public Transform TargetTransform => targetTransform;
    public NavMeshAgent Agent => agent;
    public Animator Anim => anim;

    // ���� �����Ÿ�
    [SerializeField]
    private float traceDist = 10.0f;
    // ���� �����Ÿ�
    [SerializeField]
    private float attackDist = 2.0f;
    public float TraceDist => traceDist;
    public float AttackDist => attackDist;

    // �þ߰�
    [SerializeField]
    private float viewAngle;
    [SerializeField]
    private float viewDistance;
    Vector3 targetDir = Vector3.zero;
    float dotProduct;
    float theta;

    public float ViewAngle => viewAngle;
    public float Theta => theta;

    // ����
    [SerializeField]
    private Transform[] waypoints;
    [SerializeField]
    private int waypointIndex;
    [SerializeField]
    private Vector3 target;
    public Transform[] Waypoints => waypoints;
    public int WaypointIndex => waypointIndex;
    public Vector3 Target
    {
        get => target;
        set => target = value;
    }

    [SerializeField]
    private OpaqueItem opaqueItem;
    public OpaqueItem OpaqueItem => opaqueItem;

    [SerializeField]
    private HealthBarUI healthBarUI;
    private Rigidbody rigidbody;

    public HealthBarUI HealthBarUI => healthBarUI;
    public Rigidbody Rigidbody => rigidbody;

    // ���� ��� ����
    private bool isDie = false;
    public bool IsDie => isDie;

    // ������ ���� �ʱⰪ
    private readonly int iniHp = 100;
    private int currHp;

    [SerializeField]
    private GameObject floatingTextPrefab;

    // ���� ȿ�� ������
    private GameObject bloodEffect;

    private readonly int hashHit = Animator.StringToHash("Hit");

    private SpiderStateMachine spiderStateMachine;

    private void Awake()
    {
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

        spiderStateMachine = new SpiderStateMachine(this);
    }

    private void ShowBloodEffect(Vector3 pos, Quaternion rot)
    {
        // ���� ȿ�� ����
        GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot, monsterTransform);
        Destroy(blood, 1.0f);
    }

    private void OnEnable()
    {
        currHp = iniHp;
        isDie = false;

        GetComponent<CapsuleCollider>().enabled = true;

        spiderStateMachine.ChangeState(spiderStateMachine.SpiderIdle);
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
    }

    private void Update()
    {
        spiderStateMachine.Update();

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
        

        // �þ߰�
        ViewCheck();
    }

    private void OnTriggerEnter(Collider other)
    {
        spiderStateMachine.OnTriggerEnter(other);
    }

    private void ViewCheck()
    {
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

    public void IterateWaypointIndex()
    {
        waypointIndex++;
        if(waypointIndex == waypoints.Length)
        {
            waypointIndex = 0;
        }
    }

    /*
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
    */

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
                isDie = true;

                spiderStateMachine.ChangeState(spiderStateMachine.SpiderDie);
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
