using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class DogKnightCtrl : Monster
{
    //���� ��������

    public enum State
    {
        IDLE,
        TRACE,
        COMBACK,
        ATTACK,
        DIE,
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
    private readonly int hashAttack = Animator.StringToHash("IsAttack");
    private readonly int hashComback = Animator.StringToHash("IsComback");
    private readonly int hashHit = Animator.StringToHash("Hit");
    private readonly int hashDie = Animator.StringToHash("DIE");

    // ������ ���� �ʱⰪ
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

    // Score ����
    private void Awake()
    {
        currHp = iniHp;
        monsterTransform = GetComponent<Transform>();
        targetTransform = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();

        //�ڵ�ȸ�� ��� ��Ȱ��ȭ
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

        //������ ���¸� üũ�ϴ� �ڷ�ƾ
        StartCoroutine(CheckMonsterState());

        //���¿� ���� ���� �ൿ ���� �ڷ�ƾ
        StartCoroutine(MonsterAction());
    }

    private void Update()
    {
        // ���������� ���� �Ÿ��� ȸ�� ���� �Ǵ�
        if (agent.remainingDistance >= 7.0f)
        {
            // ������Ʈ�� ȸ�� ��
            Vector3 direction = agent.desiredVelocity;

            // ȸ�� ���� ����
            Quaternion rotation = Quaternion.LookRotation(direction);

            // ���� �������� �Լ��� �ε巯�� ȸ�� ó��
            monsterTransform.rotation = Quaternion.Slerp(monsterTransform.rotation, rotation, Time.deltaTime * 10.0f);
        }
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
                    // ���� ����
                    agent.isStopped = true;
                    anim.SetBool(hashTrace, false);
                    break;
                case State.TRACE:
                    // ���� ��� ��ǥ�� �̵�
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
        Debug.Log("����");
        isComback = true;
        state = State.COMBACK;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("RANGE"))
        {
            Debug.Log("����");
            EventManager.TriggerEvent("COMEBACK", eventParam);
        }
    }

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
            // �ǰ� �ִϸ��̼� ����
            anim.SetTrigger(hashHit);

            // ���� HP ����
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
