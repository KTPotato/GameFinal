using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using System.Collections;
using System.Linq;

public class Boss_Orc : MonoBehaviour
{
    public NavMeshAgent bossagent;
    public float Hp;

    private Animator animator;

    private bool isDead = false;
    private bool isAttacking = false;
    private bool isTakingHit = false; // GetHit 상태 플래그
    private bool canTakeHit = true; // 피격 가능한지 여부
    private bool isPatternPaused = false; // 패턴이 일시 중단되었는지 여부
    private bool isPatternLocked = false; // 패턴을 다시 끊지 않도록 할 변수
    private float patternLockTime = 10f; // 패턴이 끊기지 않는 시간 (10초)
    private float patternPauseTime = 2f; // 피격 후 패턴 일시 중단 시간 (2초)

    // 공격 관련 변수
    private float attackRange = 3f;
    private float attackCooldown = 3f;
    private float lastAttackTime = 0f;

    public List<GameObject> allTargets = new List<GameObject>();

    // 공격 애니메이션 변수
    public bool isAttackingAnimation = false;
    public GameObject axePrefab; // 도끼 프리팹
    public Transform throwPoint;

    // 상태 관리
    public float hitCooldown = 10f;

    private Queue<int> recentPatterns = new Queue<int>(); // 최근 패턴 기록
    private int maxRepeat = 2; // 동일 패턴 최대 반복 횟수

    // blend tree 써서 멀면 달리고 가까우면 걷기로 해야됨 
    public void Start()
    {
        bossagent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        Hp = 100;
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
                            Attack1(); // Auto 평타
                            break;
                        case 1:
                            Attack2(target); // 도끼 던지기
                            break;
                        case 2:
                            Attack3(); // 도끼 휘두르기
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

        recentPatterns.Enqueue(attackPattern);
        if (recentPatterns.Count > maxRepeat)
        {
            recentPatterns.Dequeue();
        }

        return attackPattern;
    }

    private void Attack1() // Auto 평타
    {
        if (isAttacking || isPatternPaused) return;

        isAttacking = true;
        animator.SetTrigger("Auto");

        lastAttackTime = Time.time;
        StartCoroutine(ResetAttackState());
    }

    private void Attack2(Transform target) // 도끼 던지기
    {
        if (isAttacking || isPatternPaused) return;

        isAttacking = true;
        animator.SetTrigger("Swing");

        ThrowAxe(target.position);

        lastAttackTime = Time.time;
        StartCoroutine(ResetAttackState());
    }

    private void Attack3() // 도끼 휘두르기
    {
        if (isAttacking || isPatternPaused) return;

        isAttacking = true;
        animator.SetTrigger("Swing");

        lastAttackTime = Time.time;
        StartCoroutine(ResetAttackState());
    }

    private void ThrowAxe(Vector3 targetPosition)
    {
        if (axePrefab != null && throwPoint != null)
        {
            StartCoroutine(ThrowAndReturnAxe());
        }
    }

    private IEnumerator ThrowAndReturnAxe()
    {
        yield return new WaitForSeconds(0.8f); 

        GameObject axe = Instantiate(axePrefab, throwPoint.position, Quaternion.identity);

        // Z축으로 90도 회전 설정
        axe.transform.rotation = Quaternion.Euler(axe.transform.rotation.eulerAngles + new Vector3(0, 0, 90));

        Rigidbody rb = axe.GetComponent<Rigidbody>();

        // 충돌 무시
        Collider axeCollider = axe.GetComponent<Collider>();
        Collider throwPointCollider = throwPoint.GetComponent<Collider>();
        if (axeCollider != null && throwPointCollider != null)
        {
            Physics.IgnoreCollision(axeCollider, throwPointCollider);
        }

        if (rb != null)
        {
            Vector3 forwardDirection = throwPoint.forward; // 던지는 방향
            rb.velocity = Vector3.zero; // 초기 속도 설정
            rb.AddForce(forwardDirection * 20f, ForceMode.Impulse); // 앞으로 날리기

            // 날아가는 동안 x축 회전
            StartCoroutine(RotateAxeXAxis(axe));

            // 도끼가 일정 시간 후 돌아오도록 설정
            yield return new WaitForSeconds(1.5f); // 일정 시간 동안 날아감

            rb.velocity = Vector3.zero; // 속도 초기화
            rb.angularVelocity = Vector3.zero; // 회전 초기화

            // 보스의 현재 위치를 목표로 설정
            Vector3 returnPosition = throwPoint.position;

            // 돌아오는 방향 계산
            Vector3 returnDirection = (returnPosition - axe.transform.position).normalized;
            rb.AddForce(returnDirection * 15f, ForceMode.Impulse); // 돌아오는 힘

            // 돌아오는 동안 위치를 지속적으로 체크하여 도착 시 파괴
            StartCoroutine(DestroyAxeOnReturn(axe, returnPosition));
        }
    }

    private IEnumerator RotateAxeXAxis(GameObject axe)
    {
        Rigidbody rb = axe.GetComponent<Rigidbody>();
        while (axe != null)
        {
            if (rb != null)
            {
                rb.angularVelocity = new Vector3(0, 10f, 0); // x축으로만 회전
            }
            yield return null;
        }
    }

    private IEnumerator DestroyAxeOnReturn(GameObject axe, Vector3 targetPosition)
    {
        while (axe != null)
        {
            // 도끼가 목표 위치 근처에 도달했는지 확인
            if (Vector3.Distance(axe.transform.position, targetPosition) < 0.5f)
            {
                Destroy(axe); // 도끼 파괴
                break;
            }
            yield return null;
        }
    }



    private IEnumerator ResetAttackState()
    {
        yield return new WaitForSeconds(1f);

        isAttacking = false;
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
