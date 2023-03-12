//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DefineCS;
using UnityEngine.AI;
using UnityEngine.UI;

public class MonsterCtrl : Monster
{
    // �ݶ��̴�
    private SphereCollider[] spheres;
    public SphereCollider[] Spheres => spheres;

    // �ִϸ�����
    private Animator anim;
    public Animator Anim => anim;

    // ������Ʈ ĳ��
    private Transform monsterTransform;
    private Transform targetTransform;
    public Transform MonsterTransform => monsterTransform;
    public Transform TargetTransform => targetTransform;

    // �ٰŸ� ���� �����Ÿ�
    [SerializeField]
    private float attackDist = 2.0f;
    public float AttackDist => attackDist;
    // ���Ÿ� ���� �����Ÿ�
    [SerializeField]
    private float longDistnaceAttackDist = 10.0f;
    public float LongDistnaceAttackDist => longDistnaceAttackDist;

    [SerializeField]
    private HealthBarUI healthBarUI;
    [SerializeField]
    private OpaqueItem opaqueItem;
    public HealthBarUI HealthBarUI => healthBarUI;
    public OpaqueItem OpaqueItem => opaqueItem;

    // ���� ��� ����
    private bool isDie = false;
    public bool IsDie
    {
        get => isDie;
    }

    private Rigidbody rigidbody;
    public Rigidbody Rigid => rigidbody;

    [SerializeField]
    private Transform firePos;

    // �ؽ� ���̺� �� ��������
    private readonly int hashHit = Animator.StringToHash("Hit");

    [SerializeField]
    private GameObject thorn;

    // ������ ���� �ʱⰪ
    private readonly int iniHp = 100;
    [SerializeField]
    private int currHp;

    private MonsterStateMachine monsterStateMachine;


    private void Awake()
    {
        currHp = iniHp;
        monsterTransform = GetComponent<Transform>();
        targetTransform = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();

        anim = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();

        monsterStateMachine = new MonsterStateMachine(this);
    }

    private void Start()
    {
        currHp = iniHp;
        isDie = false;

        GetComponent<CapsuleCollider>().enabled = true;
        spheres = GetComponentsInChildren<SphereCollider>();

        monsterStateMachine.ChangeState(monsterStateMachine.MonsterIdle);
    }

    private void Update()
    {
        monsterStateMachine.Update();

        if (!isDie && !opaqueItem.isOpaque)
            transform.LookAt(targetTransform);
    }

    private void OnTriggerEnter(Collider other)
    {
        monsterStateMachine.OnTriggerEnter(other);
    }

    private void ShootThorn()
    {
        Vector3 moveDir = (targetTransform.position - firePos.position).normalized;

        Quaternion quaternion = Quaternion.LookRotation(moveDir, Vector3.up);

        GameObject thorn = ObjectPoolMgr.Instance.GetPooledObject((int)PooledIndex.THORNS);
        thorn.transform.position = firePos.transform.position;
        thorn.transform.rotation = Quaternion.Euler(0f, quaternion.eulerAngles.y, 0f);
        thorn.SetActive(true);
    }

    public override void MonsterHit(Vector3 bloodPos, Vector3 bloodRot, int damage)
    {
        Debug.Log("��");
        anim.SetTrigger(hashHit);
        currHp -= damage;
        healthBarUI.ChangeHP(currHp, iniHp);

        if (currHp <= 0)
        {
            isDie = true;


            GetComponent<CapsuleCollider>().enabled = false;
            monsterStateMachine.ChangeState(monsterStateMachine.MonsterDie);


            int random = Random.Range(0, 2);

            if (random == 0)
            {
                GameObject bottle = ObjectPoolMgr.Instance.GetPooledObject((int)PooledIndex.WATER_BOTTLE);
                bottle.transform.position = transform.localPosition;
                bottle.SetActive(true);
            }
        }
    }

    /*
private void OnDrawGizmos()
{
    // ���� �����Ÿ�
    if (state == State.SLAM)
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(monsterTransform.position, attackDist);
    }


    // ���� �����Ÿ�
    if (state == State.ATTACK)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(monsterTransform.position, longDistnaceAttackDist);
    }

}
*/
}
