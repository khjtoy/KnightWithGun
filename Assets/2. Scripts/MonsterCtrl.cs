//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MonsterCtrl : MonoBehaviour
{
    //몬스터 상태정보

    public enum State
    {
        IDLE,
        ATTACK,
        SLAM,
        DAMAGE,
        DIE,
    }

    // 몬스터의 현재 상태
    public State state = State.IDLE;
    // 원거리 공격 사정거리
    public float longDistnaceAttackDist = 10.0f;
    // 근거리 공격 사정거리
    public float attackDist = 2.0f;
    // 몬스터 사망 여부
    public bool isDie = false;

    // 컴포넌트 캐싱
    private Transform monsterTransform;
    private Transform targetTransform;
    private Animator anim;

    // 해시 테이블 값 가져오기
    private readonly int hashAttack = Animator.StringToHash("IsAttack");
    private readonly int hashShoot = Animator.StringToHash("IsShoot");
    private readonly int hashHit = Animator.StringToHash("Hit");
    private readonly int hashDie = Animator.StringToHash("Die");
    // 혈흔 효과 프리팹
    private GameObject bloodEffect;

    [SerializeField]
    private GameObject thorn;

    // 몬스터의 생명 초기값
    private readonly int iniHp = 100;
    private int currHp;

    private void Awake()
    {
        currHp = iniHp;
        monsterTransform = GetComponent<Transform>();
        targetTransform = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();

        anim = GetComponent<Animator>();
    }

    private void Start()
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
                state = State.SLAM;
            }
            else if (distance <= longDistnaceAttackDist)
            {
                state = State.ATTACK;
            }
            else
            {
                state = State.IDLE;
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
                    anim.SetBool(hashAttack, false);
                    anim.SetBool(hashShoot, false);
                    break;
                case State.SLAM:
                    anim.SetBool(hashAttack, true);
                    anim.SetBool(hashShoot, false);
                    break;
                case State.ATTACK:
                    anim.SetBool(hashAttack, false);
                    anim.SetBool(hashShoot, true);
                    break;
                case State.DAMAGE:
                    anim.SetTrigger(hashHit);
                    break;
                case State.DIE:
                    isDie = true;
                    anim.SetTrigger(hashDie);
                    GetComponent<CapsuleCollider>().enabled = false;
                    break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void ShootThorn()
    {
        Debug.Log("Shoot");
        Instantiate(thorn, transform);
    }
    
    private void OnDrawGizmos()
    {
        // 추적 사정거리
        if (state == State.SLAM)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(monsterTransform.position, attackDist);
        }

        
        // 공격 사정거리
        if (state == State.ATTACK)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(monsterTransform.position, longDistnaceAttackDist);
        }
        
    }
    
}
