using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;
using UnityEngine.UI;

public class BossCtrl : Monster
{
    public enum State
    {
        IDLE,
        TRACE,
        ATTACK,
        ROTATION,
        RUN_ATTACK,
        DIE
    }

    // ������ ���� ����
    public State state = State.IDLE;
    // ���� �����Ÿ�
    public float traceDist = 10.0f;
    //�� ���� �����Ÿ�
    public float longDist = 20.0f;
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
    private readonly int hashAttack = Animator.StringToHash("Attack");
    private readonly int hashLAttack = Animator.StringToHash("LAttack");
    private readonly int hashSDIndex = Animator.StringToHash("SDIndex");
    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashLong = Animator.StringToHash("Long");
    private readonly int hashHit = Animator.StringToHash("Hit");

    // ������ ���� �ʱⰪ
    private readonly int iniHp = 3000;
    [SerializeField]
    private int currHp;

    private float timer = 0;
    [SerializeField]
    private float setTime = 0.5f;
    private bool isChange = false;
    private bool isAttack = false;

    private int mode = 0;

    private Rigidbody rigidbody;

    [SerializeField]
    private ParticleSystem slashParticle;
    [SerializeField]
    private SphereCollider bossSword;
    [SerializeField]
    private SphereCollider bossKick;
    [SerializeField]
    private HealthBarUI healthBarUI;
    [SerializeField]
    private GameCutScene gameCutScene;

    Quaternion TargetRot;

    // �þ߰�
    [SerializeField]
    private float viewAngle;
    [SerializeField]
    private float viewDistance;
    Vector3 targetDir = Vector3.zero;
    float dotProduct;
    float theta;
    State oldState;
    public bool isRot = false;

    [SerializeField]
    private Vector3[] targetPos;
    private int targetIndex = 0;
    [SerializeField]
    private SpriteRenderer xTarget;

    private int oldIndex = -1;


    private void Awake()
    {
        Debug.Log("^^");
        currHp = iniHp;
        monsterTransform = GetComponent<Transform>();
        targetTransform = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();

        //�ڵ�ȸ�� ��� ��Ȱ��ȭ
        agent.updateRotation = false;

        anim = GetComponentInChildren<Animator>();
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        ShowTarget();
    }
    private void OnEnable()
    {
        state = State.IDLE;

        currHp = iniHp;
        isDie = false;

        
        GetComponent<CapsuleCollider>().enabled = true;
        bossSword.enabled = false;
        /*
        SphereCollider[] spheres = GetComponentsInChildren<SphereCollider>();
        foreach (SphereCollider sphere in spheres)
        {
            sphere.enabled = true;
        }
        */


        ////������ ���¸� üũ�ϴ� �ڷ�ƾ
        StartCoroutine(CheckMonsterState());

        ////���¿� ���� ���� �ൿ ���� �ڷ�ƾ
        StartCoroutine(MonsterAction());
        state = State.IDLE;
        oldState = State.IDLE;
    }


    private void Update()
    {
        /*
        if(oldState != state)
        {
            oldState = state;

            switch (state)
            {
                case State.IDLE:
                    agent.isStopped = true;
                    anim.SetBool(hashTrace, false);
                    break;
                case State.TRACE:
                    isRot = false;
                    agent.speed = 10f;
                    agent.isStopped = false;
                    anim.SetBool(hashTrace, true);
                    break;
                case State.ATTACK:
                    {
                        if (!isAttack)
                        {
                            isRot = false;
                            isAttack = true;
                            agent.isStopped = true;
                            slashParticle.gameObject.SetActive(true);
                            anim.ResetTrigger(hashLAttack);
                            int index = Random.Range(0, 2);
                            anim.SetFloat(hashSDIndex, index);
                            anim.SetTrigger(hashAttack);
                        }
                    }
                    break;
                case State.ROTATION:
                    break;
                case State.RUN_ATTACK:
                    break;
                case State.DIE:
                    break;
                default:
                    break;
            }
        }

        switch (state)
        {
            case State.IDLE:
                {
                    float distance = Vector3.Distance(monsterTransform.position, targetTransform.position);
                    if (distance > attackDist)
                    {
                        state = State.TRACE;
                    }
                    else
                    {
                        state = State.ATTACK;
                    }
                }
                break;
            case State.TRACE:
                {
                    agent.SetDestination(targetTransform.position);
                    float distance = Vector3.Distance(monsterTransform.position, targetTransform.position);
                    if (distance <= attackDist)
                    {
                        state = State.IDLE;
                    }
                }
                break;
            case State.ATTACK:
                // ���� ����
                // �Ϸ� -> ���
                if(!isAttack)
                {
                    state = State.IDLE; 
                }
                break;
            case State.ROTATION:
                break;
            case State.RUN_ATTACK:
                break;
            case State.DIE:
                break;
            default:
                break;
        }
        */


        if (isChange)
        {
        timer += Time.deltaTime;
        if (timer >= setTime)
        {
            isChange = false;
            isAttack = false;
            isRot = false;
            timer = 0;
        }
        }


        // ���������� ���� �Ÿ��� ȸ�� ���� �Ǵ�
        if ((agent.remainingDistance >= 5f || isRot) && !isChange)
        {
            // ������Ʈ�� ȸ�� ��
            //Vector3 direction = agent.desiredVelocity;
            // ȸ�� ���� ����
            //Quaternion rotation = Quaternion.LookRotation(-direction);

            // monsterTransform.rotation = Quaternion.Slerp(monsterTransform.rotation, rotation, Time.deltaTime * 10.0f);

            // ���� �������� �Լ��� �ε巯�� ȸ�� ó��
            Vector3 l_vector = targetTransform.position - monsterTransform.position;

            // ȸ�� ���� ����
            Quaternion rotation = Quaternion.LookRotation(l_vector);

            // ������Ʈ�� ȸ�� ��
            //Vector3 direction = agent.desiredVelocity;

            // ȸ�� ���� ����
            //Quaternion rotation = Quaternion.LookRotation(-direction);

            // ���� �������� �Լ��� �ε巯�� ȸ�� ó��
            monsterTransform.rotation = Quaternion.Slerp(monsterTransform.rotation, rotation, Time.deltaTime * 10.0f);
        }

        Vector3 leftBoundary = DirFromAngle(-viewAngle / 2);
        Vector3 rightBoundary = DirFromAngle(viewAngle / 2);
        Debug.DrawLine(transform.position, transform.position + leftBoundary * viewDistance, Color.blue);
        Debug.DrawLine(transform.position, transform.position + rightBoundary * viewDistance, Color.blue);

        targetDir = (targetTransform.position - monsterTransform.position).normalized;
        dotProduct = Vector3.Dot(transform.forward.normalized, targetDir);
        theta = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;
        //Debug.Log(theta);
        //theta = theta;

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

            if (!isChange)
            {

                
                /*
                switch (state)
                {
                    case State.IDLE:
                        // ���� ã�� ���ݹ��� �ȿ� ������ ���� ����
                        break;
                    case State.TRACE:
                        break;
                    case State.ATTACK:
                        // ���� ����
                        // �Ϸ� -> ���
                        break;
                    case State.ROTATION:
                        break;
                    case State.RUN_ATTACK:
                        break;
                    case State.DIE:
                        break;
                    default:
                        break;
                }
                */
                

                // ������ ĳ���� ������ �Ÿ� ����
                float distance = Vector3.Distance(monsterTransform.position, targetTransform.position);

                if (distance <= attackDist)
                {
                    if (theta <= viewAngle / 2)
                    {
                        state = State.ATTACK;
                    }
                    else
                        state = State.ROTATION;
                }
                else if (distance >= longDist)
                {
                    state = State.RUN_ATTACK;
                }
                else
                {
                    if (state != State.ATTACK && state != State.RUN_ATTACK) 
                        state = State.TRACE;
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
                    //isRot = false;
                    anim.SetBool(hashTrace, false);
                    //anim.SetBool(hashAttack, false);
                    break;
                case State.TRACE:
                    // ���� ��� ��ǥ�� �̵�
                    isRot = false;
                    agent.SetDestination(targetTransform.position);
                    agent.speed = 10f;
                    agent.isStopped = false;
                    anim.SetBool(hashTrace, true);
                    //anim.SetBool(hashAttack, false);
                    break;
                case State.ROTATION:
                    isRot = true;
                    break;
                case State.ATTACK:
                    if (!isAttack)
                    {
                        isRot = false;
                        isAttack = true;
                        agent.isStopped = true;
                        slashParticle.gameObject.SetActive(true);
                        //bossSword.enabled = true;
                        anim.SetBool(hashTrace, false);
                        anim.ResetTrigger(hashLAttack);
                        int index = Random.Range(0, 2);
                        //index = 1;
                        anim.SetBool(hashLong, false);
                        anim.SetFloat(hashSDIndex, index);
                        anim.SetTrigger(hashAttack);
                    }
                    break;
                case State.RUN_ATTACK:
                    if(!isAttack)
                    {
                        isRot = false;
                        isAttack = true;
                        agent.isStopped = true;
                        slashParticle.gameObject.SetActive(true);
                        anim.SetBool(hashLong, true);
                        anim.ResetTrigger(hashAttack);
                        monsterTransform.DOMove(targetTransform.position, 1.5f);
                        anim.SetTrigger(hashLAttack);
                    }
                    break;
                case State.DIE:
                    isDie = true;
                    agent.isStopped = true;
                    isRot = false;
                    anim.SetTrigger(hashDie);
                    rigidbody.isKinematic = false;
                    break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void Wait()
    {
        Debug.Log(transform.name);
        isChange = true;
        isRot = true;
        slashParticle.gameObject.SetActive(false);
        state = State.IDLE;
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

    public void SetSword(bool setBool)
    {
        bossSword.enabled = setBool;
    }

    public void SetKick(bool setBool)
    {
        bossKick.enabled = setBool;
    }

    public void radiusSword(float radius)
    {
        bossSword.radius = radius;
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

        if(state == State.RUN_ATTACK)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(monsterTransform.position, longDist);
        }
    }

    public override void MonsterHit(Vector3 bloodPos, Vector3 bloodRot, int damage)
    {
        /*
        if (mode != 0) return;
        if (currHp > 0)
        {
            // ���� HP ����

            if(currHp - damage <= 500)
            {
                currHp = 500;
                mode = 1;
                ShowTarget();
                return;
            }

            currHp -= damage;
            healthBarUI.ChangeHP(currHp, iniHp);
            /*
            if (currHp <= 0)
            {
                anim.SetBool("IsDie", true);
                anim.SetBool(hashTrace, false);
                anim.SetBool(hashAttack, false);
                anim.SetBool(hashPatrol, false);
                state = State.DIE;
            }
            
        }
        */
    }

    /*
    public override void MonsterHit()
    {
        if (currHp > 0)
        {
            // �ǰ� �ִϸ��̼� ����
            anim.SetTrigger(hashHit);

            // ���� HP ����
            currHp -= 10;
            healthBarUI.ChangeHP(currHp, iniHp);
            if (currHp <= 0)
            {
                anim.SetBool(hashTrace, false);
                anim.SetBool(hashAttack, false);
                anim.SetBool(hashPatrol, false);
                state = State.DIE;
            }
        }
    }
    */
    private void ShowTarget()
    {
        int random = Random.Range(0, 10);
        while(random == oldIndex)
        {
            random = Random.Range(0, 10);
        }

        oldIndex = random;

        if(currHp <= 300)
        {
            random = 9;
        }
        xTarget.transform.localPosition = targetPos[random];
        xTarget.DOFade(1, 0.5f);
        xTarget.GetComponent<SphereCollider>().enabled = true;
    }

    public void DamageTarget()
    {
        xTarget.GetComponent<SphereCollider>().enabled = false;
        currHp -= 300;
        healthBarUI.ChangeHP(currHp, iniHp);
        xTarget.color = new Color(1, 1, 1, 0);

        if(currHp <= 0)
        {
            gameCutScene.OnBossCutScene(this);
            return;
        }

        anim.SetTrigger(hashHit);
        ShowTarget();
    }

    public void ChangeDie()
    {
        state = State.DIE;
    }
}
