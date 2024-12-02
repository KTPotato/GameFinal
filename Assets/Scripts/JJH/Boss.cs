using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    public NavMeshAgent bossagent;
    public float Hp;

    private Animator animator;

    private bool isDead = false;

    public List<GameObject> allTargets = new List<GameObject>();

    public float attackRange = 5f;
    public float attackCooldown = 3f;
    private float lastAttackTime = 0f;

    public bool isAttacking = false;
    public bool isTakingHit = false; // GetHit 상태 플래그

    public GameObject rockPrefab;
    public Transform throwPoint;

    private bool canTakeHit = true;
    public float hitCooldown = 2f;

    public void Start()
    {
        bossagent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        Hp = 100;
    }

    public void Update()
    {
        if (isDead || isTakingHit) return; // GetHit 상태일 때 모든 동작 차단

        if (Hp <= 0)
        {
            Die();
            return;
        }

        allTargets.Clear();
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        allTargets.AddRange(player);

        float minmumdistance = float.MaxValue;
        Transform target = null;

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

        if (target != null)
        {
            bossagent.isStopped = false;
            animator.SetBool("Walk", true);
            bossagent.SetDestination(target.position);

            if (minmumdistance <= attackRange && Time.time - lastAttackTime >= attackCooldown)
            {
                bossagent.isStopped = true;
                if (Random.value > 0.5f)
                {
                    Attack1(target);
                }
                else
                {
                    Attack2(target);
                }
            }
        }
        else
        {
            animator.SetBool("Walk", false);
            animator.SetTrigger("Idle");
        }
    }

    private void Attack1(Transform target)
    {
        if (isAttacking) return;

        isAttacking = true;
        animator.SetBool("Walk", false);
        animator.SetTrigger("Throw");
        ThrowRock(target.position);

        lastAttackTime = Time.time;
        StartCoroutine(ResetAttackState());
    }

    private void Attack2(Transform target)
    {
        if (isAttacking) return;

        isAttacking = true;
        animator.SetBool("Walk", false);
        animator.SetTrigger("Punch");

        lastAttackTime = Time.time;
        StartCoroutine(ResetAttackState());
    }

    private void ThrowRock(Vector3 targetPosition)
    {
        StartCoroutine(ThrowRockAfterDelay(targetPosition));
    }

    private IEnumerator ThrowRockAfterDelay(Vector3 targetPosition) // 반환 형식 수정
    {
        yield return new WaitForSeconds(1.5f); // 대기

        if (rockPrefab != null && throwPoint != null)
        {
            // 돌 생성
            GameObject rock = Instantiate(rockPrefab, throwPoint.position, Quaternion.identity);
            Rigidbody rb = rock.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // 반대 방향으로 돌 던지기
                Vector3 direction = (throwPoint.position - targetPosition).normalized;
                rb.AddForce(direction * 15f, ForceMode.Impulse);

                // 회전 효과 추가
                Vector3 randomTorque = new Vector3(
                    Random.Range(-10f, 10f),
                    Random.Range(-10f, 10f),
                    Random.Range(-10f, 10f)
                );
                rb.AddTorque(randomTorque, ForceMode.Impulse);
            }

            // 5초 후에 돌 제거
            Destroy(rock, 5f);
        }
    }


    private IEnumerator ResetAttackState()
    {
        yield return new WaitForSeconds(1f);
        isAttacking = false;
        animator.SetBool("Walk", true);
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
            Debug.Log("앙 맞았띠");
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead || isTakingHit) return;

        Hp -= damage;

        if (canTakeHit)
        {
            canTakeHit = false;
            isTakingHit = true; // GetHit 상태 활성화
            animator.SetBool("Walk", false);
            animator.SetTrigger("GetHit");
            StartCoroutine(ResetHitState());
        }
    }

    private IEnumerator ResetHitState()
    {
        yield return new WaitForSeconds(hitCooldown);
        isTakingHit = false; // GetHit 상태 해제
        canTakeHit = true;
    }
}
