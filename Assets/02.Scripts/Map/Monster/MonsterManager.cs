using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements.Experimental;

public enum AIState
{
    Idle,
    Walk,
    Run,
    Attack
}

public class MonsterManager : MonoBehaviour
{
    [Header("Stats")]
    public float walkSpeed;
    public float runSpeed;
    public bool running = false;
    public bool standing = false;
    public bool founding = false;
    private bool isStunned = false;
    private bool isAttackDoor = false;
    private float stunTimer = 0f;

    [Header("AI")]
    private NavMeshAgent agent;
    public float detectDistance;
    public float addedDistance;
    private AIState aiState;
    public System.Func<bool> findPlayer;

    [Header("Wandering")]
    public float minWanderDistance;
    public float maxWanderDistance;
    public float minWanderWaitTime;
    public float maxWanderWaitTime;

    [Header("Combat")]
    MonsterController monster;
    public float playerDistance;
    public float rayLength;


    public float fieldOfView = 120f;

    private SoundSource soundSource;
    private Animator animator;
    private SkinnedMeshRenderer[] meshRenderers;

    private bool isChasing = false;


    // --- 인형 유인 관련 변수 ---
    private bool attracted = false;         // 인형에 유인 중인지 여부
    private float attractTimer = 0f;        // 유인 지속 시간 카운터
    private Vector3 attractTargetPosition;  // 유인 위치 (인형 위치)
    public float attractDuration = 8f;      // 유인 지속 시간 (초)

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        monster = GetComponent<MonsterController>();
        SetState(AIState.Walk);
    }

    void Update()
    {
        // 스턴 상태 처리 먼저
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;

            if (stunTimer <= 0f)
            {
                isStunned = false;
                agent.isStopped = false;
                animator.speed = agent.speed / walkSpeed;  // 원래 애니메이션 속도 복원
            }

            return; // 스턴 중엔 아래 로직 실행하지 않음
        }

        // --- 인형 유인 상태 우선 처리 ---
        if (attracted)
        {
            attractTimer -= Time.deltaTime;

            // 유인 시간이 끝나면 원래 상태로 돌아감
            if (attractTimer <= 0f)
            {
                attracted = false;
                SetState(AIState.Walk); // 유인 종료 후 걷기 상태 복귀
            }
        }

        playerDistance = Vector3.Distance(transform.position, Player.Instance.transform.position);
        if (running)
        {
            animator.SetBool("Run", aiState != AIState.Idle);
            animator.SetBool("Walk", false);
        }
        else
        {
            animator.SetBool("Walk", aiState != AIState.Idle);
            animator.SetBool("Run", aiState != AIState.Idle);
        }

        switch (aiState)
        {
            case AIState.Idle:
                PassiveUpdate();
                if (isChasing)
                {
                    isChasing = false;
                    Player.Instance.condition.chaseCount--;
                }
                monster.IdleSound();

                break;
            case AIState.Walk:
                PassiveUpdate();
                if (isChasing)
                {
                    isChasing = false;
                    Player.Instance.condition.chaseCount--;
                }
                monster.WalkSound();
                break;
            case AIState.Run:
                AttackingUpdate();
                if (!isChasing && !attracted)
                {
                    Player.Instance.condition.chaseCount++;
                    isChasing = true;
                }

                if (founding)
                {
                    FindHide();
                }
                if (!founding && Player.Instance.controller.currentState == PlayerState.Hiding) // 찾는 도중 숨었을 때 처리
                {
                    Debug.Log("들킴ㄷㄷ");
                    founding = true; // 코루틴 반복 호출 방지
                    StartCoroutine(FindClear());
                }
                Collider[] hits = Physics.OverlapSphere(transform.position, 1f, LayerMask.GetMask("Interactable"));

                foreach (var hit in hits)
                {
                    if (hit.CompareTag("Door"))
                    {
                        StartCoroutine(DestroyDoor(hit.gameObject));
                        Debug.Log("문 감지됨");
                    }
                }
                monster.RunSound();
                
                break;
        }
    }

    public void SetState(AIState state)
    {
        aiState = state;

        switch (aiState)
        {
            case AIState.Idle:
                agent.speed = walkSpeed;
                agent.isStopped = true;
                break;
            case AIState.Walk:
                agent.speed = walkSpeed;
                agent.isStopped = false;
                break;
            case AIState.Run:
                agent.speed = runSpeed;
                animator.SetBool("Run", agent.speed == runSpeed);
                agent.isStopped = false;
                break;
            case AIState.Attack:
                Player.Instance.cinemachineBrain.enabled = true;
                agent.speed = walkSpeed;
                animator.SetTrigger("Attack");
                monster.DieAnimation();
                agent.isStopped = true;
                break;
        }

        animator.speed = agent.speed / walkSpeed;
    }

    

    void PassiveUpdate()
    {
        if (aiState == AIState.Walk && agent.remainingDistance < 0.1f)
        {
            SetState(AIState.Idle);
            if (!standing) Invoke("WanderToNewLocation", Random.Range(minWanderWaitTime, maxWanderWaitTime));
        }

        if (findPlayer != null && findPlayer())
        {
            Debug.Log("공격으로 전환");
            SetState(AIState.Run);
        }
    }

    void WanderToNewLocation()
    {
        if (aiState != AIState.Idle) return;

        SetState(AIState.Walk);
        agent.SetDestination(GetWanderLocation());
    }

    Vector3 GetWanderLocation()
    {
        NavMeshHit hit;

        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);

        int i = 0;

        while (Vector3.Distance(transform.position, hit.position) < detectDistance)
        {
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
            if (i == 30) break;
        }

        return hit.position;
    }

    void AttackingUpdate()
    {
        if (attracted)
        {
            agent.SetDestination(attractTargetPosition);  // 인형 위치를 목적지로 설정
        }
        else if (playerDistance < addedDistance && !isAttackDoor)
        {
            agent.isStopped = false;
            NavMeshPath path = new NavMeshPath();
            if (agent.CalculatePath(Player.Instance.transform.position, path))
            {
                agent.SetDestination(Player.Instance.transform.position);
            }
            else
            {
                agent.SetDestination(transform.position);
                agent.isStopped = true;
                SetState(AIState.Walk);
            }
        }
        else if (!isAttackDoor)
        {
            Debug.Log("마지막 추격");
            Transform lastPlayer = Player.Instance.transform;
            SetState(AIState.Walk);
            agent.SetDestination(lastPlayer.position);
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            agent.isStopped = true;
            SetState(AIState.Attack);
        }
    }

    // --- 인형 유인 함수 ---
    /// <summary>
    /// 몬스터를 특정 위치로 유인한다.
    /// 인형에서 호출하며, 인형 위치로 몬스터가 달려가게 한다.
    /// 일정 시간 후 유인 상태 종료
    /// </summary>
    /// <param name="position">유인할 위치 (인형 위치)</param>
    public void AttractTo(Vector3 position, float time)
    {
        attracted = true;
        attractTimer = attractDuration;
        attractTargetPosition = position;

        SetState(AIState.Run);          // 달리기 상태로 전환
        StartCoroutine(AttracClear(time));
    }

    IEnumerator AttracClear(float time)
    {
        yield return new WaitForSeconds(time);
        attracted = false;
    }


    public void Stun(float duration)
    {
        isStunned = true;
        stunTimer = duration;
        agent.isStopped = true;
        animator.speed = 0f; // 애니메이션도 멈춤
    }

    void FindHide()
    {
        Debug.Log("발사");
        Vector3 directionToPlayer = Player.Instance.transform.position - transform.position;

        Ray ray = new Ray(transform.position, directionToPlayer);
        Debug.DrawRay(transform.position, directionToPlayer, Color.green);
        if (Physics.Raycast(ray, out RaycastHit hit, rayLength, LayerMask.GetMask("Player")))
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                Debug.Log("못숨음");
                if (Player.Instance.controller.currentState == PlayerState.Hiding) monster.DieAnimation();
            }
        }
    }

    IEnumerator FindClear()
    {
        yield return new WaitForSeconds(2f);
        founding = false;
        agent.SetDestination(transform.position);
        agent.isStopped = true;
        SetState(AIState.Walk);
        if (isChasing)
        {
            isChasing = false;
            Player.Instance.condition.chaseCount--;
        }

    }

    IEnumerator DestroyDoor(GameObject door)
    {
        if(isAttackDoor) yield break;
        isAttackDoor = true;
        SoundManager.Instance.PlaySFX("strugglingDoor",door.transform.position,transform);
        Transform lastPlayer = Player.Instance.transform;
        agent.isStopped = true;
        yield return new WaitForSeconds(2f);
        SoundManager.Instance.PlaySFX("doorBrake", door.transform.position,transform);
        Destroy(door);
        agent.isStopped = false;
        SetState(AIState.Walk);
        agent.SetDestination(lastPlayer.position);
        isAttackDoor = false;
    }
}
