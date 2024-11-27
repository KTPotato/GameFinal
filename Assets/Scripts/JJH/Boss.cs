using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    public NavMeshAgent bossagent;
    public float Hp;

    private Animator animator;

    private bool isDead = false;

    // 타겟 리스트를 매 프레임마다 새로 초기화
    public List<GameObject> allTargets = new List<GameObject>();

    public float attackRange = 10f;
    public float attackCooldown = 3f;
    private float lastAttackTime = 0f;

    public bool isAttacking = false;

    public GameObject rockPrefab;  // 돌의 프리팹
    public Transform throwPoint;  // 돌을 던질 위치

    public void Start()
    {
        bossagent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        Hp = 100;
    }

    public void Update()
    {
        if (isDead) return;

        if (Hp <= 0)
        {
            Die();
            return;
        }

        // 모든 타겟 리스트를 매 프레임마다 새로 초기화
        allTargets.Clear();

        // 플레이어 타겟 찾기
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        allTargets.AddRange(player);

        float minmumdistance = float.MaxValue;
        Transform target = null;

        // 가까운 타겟 찾기
        foreach (GameObject obj in allTargets)
        {
            if (obj.GetInstanceID() == gameObject.GetInstanceID()) continue;

            float distance = Vector3.Distance(transform.position, obj.transform.position);

            if (distance < minmumdistance)
            {
                minmumdistance = distance;
                target = obj.transform;
            }
        }

        // 타겟이 있을 때
        if (target != null)
        {
            animator.SetBool("Walk", true);  // 타겟이 있을 때만 걷기 상태로 전환
            bossagent.SetDestination(target.position);

            // 공격 범위 내에 있을 때만 공격
            if (minmumdistance <= attackRange && Time.time - lastAttackTime >= attackCooldown)
            {
                if (Random.value > 0.5f)
                {
                    Attack1(target); // 돌 던지기
                }
                else
                {
                    Attack2(target); // 펀치
                }
            }
        }
        else
        {
            // 타겟이 없으면 Idle 상태로 전환
            animator.SetBool("Walk", false); // 걷기 상태를 false로 설정
            animator.SetTrigger("Idle");
        }
    }

    private void Attack1(Transform target)
    {
        if (isAttacking) return;

        isAttacking = true;

        if (target.CompareTag("Player"))
        {
            animator.SetBool("Walk", false);
            animator.SetTrigger("Throw");  // 돌 던지기 애니메이션 트리거
            ThrowRock(target.position);
        }

        lastAttackTime = Time.time;
        StartCoroutine(ResetAttackState());
    }

    private void Attack2(Transform target)
    {
        if (isAttacking) return;

        isAttacking = true;

        if (target.CompareTag("Player"))
        {
            animator.SetBool("Walk", false);
            animator.SetTrigger("Punch"); // 펀치 애니메이션 트리거
        }

        lastAttackTime = Time.time;
        StartCoroutine(ResetAttackState());
    }

    private void ThrowRock(Vector3 targetPosition)
    {
        if (rockPrefab != null && throwPoint != null)
        {
            GameObject rock = Instantiate(rockPrefab, throwPoint.position, Quaternion.identity);
            Rigidbody rb = rock.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 direction = (targetPosition - throwPoint.position).normalized;
                rb.AddForce(direction * 10f, ForceMode.Impulse); // 던지는 힘 조절
            }

            Destroy(rock, 5f); // 5초 후 돌 제거
        }
    }

    private IEnumerator ResetAttackState()
    {
        yield return new WaitForSeconds(1f);

        isAttacking = false;
        animator.SetBool("Walk", true); // `Walk` 상태로 복귀
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        bossagent.isStopped = true;

        GetComponent<Collider>().enabled = false;

        Destroy(gameObject, 2f);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        Hp -= damage;
        animator.SetBool("Walk",false);
        animator.SetTrigger("GetHit"); // GetHit 상태로 전환
    }
}
