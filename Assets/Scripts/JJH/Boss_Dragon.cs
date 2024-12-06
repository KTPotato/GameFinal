using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

public class Boss_Dragon : MonoBehaviour
{
    public NavMeshAgent bossagent;
    public float Hp;

    private Animator animator;

    private bool isDead = false;
    private bool isAttacking = false;
    private bool isTakingHit = false; // GetHit 상태 플래그
    private bool canTakeHit = true; // 피격 가능한지 여부
    private bool isFlying = false; // Fly 상태
    private bool canDefend = true; // 방어 가능 여부
    private bool isPatternPaused = false; // 패턴이 일시 중단되었는지 여부
    private bool isPatternLocked = false; // 패턴을 다시 끊지 않도록 할 변수
    private float patternLockTime = 10f; // 패턴이 끊기지 않는 시간 (10초)
    private float patternPauseTime = 2f; // 피격 후 패턴 일시 중단 시간 (2초)

    // 공격 관련 변수
    private float attackRange = 9f;
    private float attackCooldown = 3f;
    private float lastAttackTime = 0f;

    public List<GameObject> allTargets = new List<GameObject>();


    // 상태 관리
    public float hitCooldown = 10f;

    private Queue<int> recentPatterns = new Queue<int>(); // 최근 패턴 기록
    private int maxRepeat = 2; // 동일 패턴 최대 반복 횟수

    public GameObject Flame;
    public Transform FlamePoint;

    public GameObject BitePoint;
    public GameObject DrivePoint;
    public GameObject DefendPoint;

    private bool isInitialized = false;

    private void Start()
    {
        bossagent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        Hp = 200;
        StartCoroutine(InitializeAfterDelay());
    }
    private IEnumerator InitializeAfterDelay()
    {
        bossagent.speed = 0;
        yield return new WaitForSeconds(3f); // 3초 대기
        bossagent.speed = 3.5f;
        isInitialized = true;
    }
    public void Update()
    {
        if (isDead || isTakingHit) return;

        if (Hp <= 0)
        {
            Die();
            return;
        }

        allTargets.Clear();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        allTargets.AddRange(players);

        float minimumDistance = float.MaxValue;
        Transform target = null;

        foreach (GameObject obj in allTargets)
        {
            if (obj.GetInstanceID() == gameObject.GetInstanceID()) continue;

            float distance = Vector3.Distance(transform.position, obj.transform.position);

            if (distance < minimumDistance)
            {
                minimumDistance = distance;
                target = obj.transform;
            }
        }

        // 타겟 거리 기반으로 WR_Point 업데이트
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // 초기화가 완료되었을 때만 스피드를 업데이트
            if (isInitialized)
            {
                if (distanceToTarget > 20f)
                {
                    animator.SetFloat("WR_Point", 0.7f);
                    bossagent.speed = 10f;
                }
                else
                {
                    animator.SetFloat("WR_Point", 0.2f);
                    bossagent.speed = 2f;
                }
            }
        }
        else
        {
            animator.SetFloat("WR_Point", 0.2f); // 기본값 설정
        }

        if (target != null)
        {
            if (minimumDistance <= attackRange)
            {
                bossagent.isStopped = true;
                animator.SetBool("Walk", false);

                if (!isAttacking && Time.time - lastAttackTime >= attackCooldown && !isPatternPaused)
                {
                    int attackPattern = GetNextAttackPattern();
                    switch (attackPattern)
                    {
                        case 0:
                            Attack1(target); // Bite 물기
                            break;
                        case 1:
                            Attack2(target); // Driving 박치기
                            break;
                        case 2:
                            Attack3(target); // Flame 불뿜기
                            break;
                    }
                }
            }
            else
            {
                bossagent.isStopped = false;
                animator.SetBool("Walk", true);
                bossagent.SetDestination(target.position);
            }
        }
        else
        {
            animator.SetBool("Walk", false);
            animator.SetTrigger("Idle");
            bossagent.isStopped = true;
            return;
        }

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !isAttacking)
        {
            animator.SetBool("Walk", true);
        }
    }


    private int GetNextAttackPattern()
    {
        int attackPattern;

        do
        {
            attackPattern = Random.Range(0, 3);
        } while (recentPatterns.Count >= maxRepeat && recentPatterns.All(p => p == attackPattern));

        // 패턴 큐에 패턴 추가
        recentPatterns.Enqueue(attackPattern);

        // 큐 크기 초과시 앞의 패턴을 제거
        if (recentPatterns.Count > maxRepeat)
        {
            recentPatterns.Dequeue();
        }

        return attackPattern;
    }

    private void Attack1(Transform target) // 물기
    {
        if (isAttacking || isPatternPaused) return;

        isAttacking = true;
        animator.SetTrigger("Bite");
        Bite(target.position);
        lastAttackTime = Time.time;
        StartCoroutine(ResetAttackState());
    }
    private void Bite(Vector3 targetPosition)
    {
        StartCoroutine(ResetBiteState());
    }
    private IEnumerator ResetBiteState()
    {
        BitePoint.SetActive(true);
        yield return new WaitForSeconds(1.1f);

        if(BitePoint != null)
        {
            BitePoint.SetActive(false);
        }
    }
    private void Attack2(Transform target) // 박치기
    {
        if (isAttacking || isPatternPaused) return;

        isAttacking = true;
        animator.SetTrigger("Driving");
        Drive(target.position);
      
        lastAttackTime = Time.time;
        StartCoroutine(ResetAttackState());
    }
    private void Drive(Vector3 targetPosition)
    {
        StartCoroutine(ResetDriveState());
    }
    private IEnumerator ResetDriveState()
    {
        DrivePoint.SetActive(true);
        yield return new WaitForSeconds(2.3f);

        if (DrivePoint != null)
        {
            DrivePoint.SetActive(false);
        }
    }

    private void Attack3(Transform target) // 불뿜기
    {
        if (isAttacking || isPatternPaused) return;

        isAttacking = true;
        animator.SetTrigger("Flame");
        Under_Flame(target.position);

        lastAttackTime = Time.time;

        StartCoroutine(ResetAttackState());
    }

    private void Under_Flame(Vector3 targetPosition)
    {
        StartCoroutine(ResetUnder_FlameState());
    }

    private IEnumerator ResetUnder_FlameState()
    {
        yield return new WaitForSeconds(0.6f);

        // FlamePoint의 방향을 기준으로 2 units 더 나가게 하기
        Vector3 adjustedPosition = FlamePoint.position + FlamePoint.forward; 
        Quaternion adjustedRotation = FlamePoint.rotation * Quaternion.Euler(15f, 90f, 0f); // 45도 오른쪽 회전 추가

        // 2f 더 나가게 된 위치와 회전값으로 FlameEffect 생성
        GameObject flameEffect = Instantiate(Flame, adjustedPosition, adjustedRotation);

        // FlameEffect가 FlamePoint를 계속 따라가도록 설정 (회전 포함)
        StartCoroutine(MoveFlameToPoint(flameEffect));
    }

    private IEnumerator MoveFlameToPoint(GameObject flameEffect)
    {
        float timeToMove = 1.5f; 
        float elapsedTime = 0f;

        // 지속적으로 FlamePoint를 따라가도록 이동
        while (elapsedTime < timeToMove)
        {
            // FlamePoint의 현재 위치를 추적 (회전은 45도 추가된 값을 사용)
            Vector3 targetPosition = FlamePoint.position + FlamePoint.forward;
            Quaternion targetRotation = FlamePoint.rotation * Quaternion.Euler(15f, -90f, 0f);
            targetPosition.x -= 1.6f;
            targetPosition.y -= 0.4f;
            // Lerp를 사용하여 flameEffect가 FlamePoint로 이동하도록 설정
            flameEffect.transform.position = Vector3.Lerp(flameEffect.transform.position, targetPosition, elapsedTime / timeToMove);
            flameEffect.transform.rotation = Quaternion.Slerp(flameEffect.transform.rotation, targetRotation, elapsedTime / timeToMove);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // FlamePoint 위치와 회전에 정확히 도달
        flameEffect.transform.position = FlamePoint.position + FlamePoint.forward; 
        flameEffect.transform.rotation = FlamePoint.rotation * Quaternion.Euler(15f, -90f, 0f); 

        Destroy(flameEffect, 0.5f);
    }





    private IEnumerator ResetAttackState()
    {
        // 공격 애니메이션이 끝날 때까지 기다리기
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Bite") ||
               animator.GetCurrentAnimatorStateInfo(0).IsName("Driving") ||
               animator.GetCurrentAnimatorStateInfo(0).IsName("Flame"))
        {
            yield return null;
        }

        // 애니메이션이 끝나면 공격이 끝났으므로 상태 리셋
        isAttacking = false;

        // 공격 후 이동 상태로 돌아가기
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            animator.SetBool("Walk", true);
            yield return new WaitForSeconds(0.7f);
            bossagent.speed = 2.5f;
        }
    }


    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        bossagent.isStopped = true;

        GetComponent<Collider>().enabled = false;

        Destroy(gameObject, 2f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bullet"))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead || isTakingHit) return;

        Hp -= damage;
        Debug.Log("맞음");
        if (!isPatternLocked)
        {
            if (canTakeHit)
            {
                canTakeHit = false;
                isTakingHit = true;
                animator.SetBool("Walk", false);
                animator.SetTrigger("GetHit");
                Debug.Log("패턴중지");
                StartCoroutine(LockPatternForDuration(patternLockTime));
                StartCoroutine(TakeDamageCooldown());
            }
        }
    }

    private IEnumerator LockPatternForDuration(float duration)
    {
        isPatternLocked = true;
        yield return new WaitForSeconds(duration);
        isPatternLocked = false;
    }

    private IEnumerator TakeDamageCooldown()
    {
        yield return new WaitForSeconds(patternPauseTime);
        canTakeHit = true;
        isTakingHit = false;
    }
}