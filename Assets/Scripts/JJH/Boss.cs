﻿using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
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
    public GameObject rockPrefab;
    public Transform throwPoint;
    public Transform throwPoint2;

    // 상태 관리
    public float hitCooldown = 10f;

    public void Start()
    {
        bossagent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        Hp = 100;
    }

    public void Update()
    {
        if (isDead || isTakingHit) return; // 죽거나 피격 중이라면 더 이상 진행하지 않습니다.

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

        // 가장 가까운 타겟 찾기
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

        // 타겟이 있을 경우 행동
        if (target != null)
        {
            if (minimumDistance <= attackRange)
            {
                // 공격 범위 안에 있는 경우
                bossagent.isStopped = true;
                animator.SetBool("Walk", false);
                // 공격 가능 여부 체크
                if (!isAttacking && Time.time - lastAttackTime >= attackCooldown && !isPatternPaused)
                {
                    int attackPattern = Random.Range(0, 3); // 3가지 공격 패턴 중 하나를 랜덤으로 선택
                    if (attackPattern == 0)
                    {
                        Attack1(target);
                    }
                    else if (attackPattern == 1)
                    {
                        Attack2(target);
                    }
                    else
                    {
                        Attack3(target); // 추가된 Punch 패턴
                    }
                }
            }
            else
            {
                // 공격 범위 밖에 있는 경우
                bossagent.isStopped = false;
                animator.SetBool("Walk", true);
                bossagent.SetDestination(target.position);
            }
        }
        else
        {
            // 타겟이 없는 경우
            animator.SetBool("Walk", false);
            animator.SetTrigger("Idle");
            bossagent.isStopped = true;
            return; // 플레이어 죽일시 멈춤
        }

        // 공격 애니메이션이 끝났는지 확인
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !isAttacking)
        {
            animator.SetBool("Walk", true); // 공격 후 Walk로 돌아가기
        }
    }

    private void Attack1(Transform target) // ThrowRock
    {
        if (isAttacking || isPatternPaused) return;

        isAttacking = true;
        animator.SetBool("Walk", false);
        animator.SetTrigger("Throw");
        ThrowRock(target.position);

        lastAttackTime = Time.time;
        bossagent.speed = 0; // 공격 중에 멈추기
        StartCoroutine(ResetAttackState());
    }

    private void Attack2(Transform target) // ThrowRock2 (세 개의 돌 던지기)
    {
        if (isAttacking || isPatternPaused) return;

        isAttacking = true;
        animator.SetBool("Walk", false);
        animator.SetTrigger("Punch");
        Punch2(target.position); // Punch2로 변경

        lastAttackTime = Time.time;
        bossagent.speed = 0; // 공격 중에 멈추기
        StartCoroutine(ResetAttackState());
    }

    private void Attack3(Transform target) // Punch
    {
        if (isAttacking || isPatternPaused) return;

        isAttacking = true;
        animator.SetBool("Walk", false);
        animator.SetTrigger("Punch");

        lastAttackTime = Time.time;
        bossagent.speed = 0; // 공격 중에 멈추기
        StartCoroutine(ResetAttackState());
    }

    private void ThrowRock(Vector3 targetPosition) // 기존 던지기
    {
        StartCoroutine(ThrowRockInStraightLine());
    }

    private IEnumerator ThrowRockInStraightLine()
    {
        yield return new WaitForSeconds(1.5f);

        if (rockPrefab != null && throwPoint != null)
        {
            GameObject rock = Instantiate(rockPrefab, throwPoint.position, Quaternion.identity);
            Rigidbody rb = rock.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 direction = throwPoint.forward;
                rb.AddForce(direction * 15f, ForceMode.Impulse);

                Vector3 randomTorque = new Vector3(
                    Random.Range(-10f, 10f),
                    Random.Range(-10f, 10f),
                    Random.Range(-10f, 10f)
                );
                rb.AddTorque(randomTorque, ForceMode.Impulse);
            }

            Destroy(rock, 5f);
        }
    }

    private void Punch2(Vector3 targetPosition)
    {
        StartCoroutine(Punch2AfterDelay(targetPosition));  // Punch2를 시작하는 메서드
    }

    private IEnumerator Punch2AfterDelay(Vector3 targetPosition)
    {
        yield return new WaitForSeconds(0.3f);  // 약간의 지연 후 공격

        if (rockPrefab != null && throwPoint2 != null)
        {
            // Punch2 패턴으로 돌 던지기 시작
            GameObject rock = Instantiate(rockPrefab, throwPoint2.position, Quaternion.identity);
            Rigidbody rb = rock.GetComponent<Rigidbody>();

            rock.transform.localScale *= 0.5f;

            // 돌의 앞 방향은 항상 일정함
            Vector3 throwDirection = transform.forward;

            // 1초 뒤에 돌을 5개로 쪼개서 날리기
            yield return new WaitForSeconds(0.7f);

            Destroy(rock);  // 원본 돌 파괴

            // 분리된 돌 5개 생성
            for (int i = 0; i < 5; i++)
            {
                GameObject splitRock = Instantiate(rockPrefab, rock.transform.position, Quaternion.identity);
                Rigidbody splitRb = splitRock.GetComponent<Rigidbody>();

                splitRock.transform.localScale *= 0.5f;

                if (splitRb != null)
                {
                    // 돌이 날아가는 방향은 항상 앞방향으로 고정
                    Vector3 splitDirection = throwDirection + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                    splitDirection = splitDirection.normalized;

                    splitRb.AddForce(splitDirection * 15f, ForceMode.Impulse);

                    // 회전하는 효과를 주기 위해 랜덤한 토크 추가
                    Vector3 randomTorque = new Vector3(
                        Random.Range(-10f, 10f),
                        Random.Range(-10f, 10f),
                        Random.Range(-10f, 10f)
                    );
                    splitRb.AddTorque(randomTorque, ForceMode.Impulse);
                }

                Destroy(splitRock, 5f);
            }
        }
    }

    private IEnumerator ResetAttackState()
    {
        yield return new WaitForSeconds(1f);  // 공격 애니메이션이 끝날 때까지 기다립니다.

        isAttacking = false;

        // 공격 애니메이션이 끝났다면 보스를 다시 이동 가능하게 설정
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            animator.SetBool("Walk", true); // 이동 애니메이션 활성화
            yield return new WaitForSeconds(0.7f);
            bossagent.speed = 2.5f; // 다시 이동
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
                isTakingHit = true; // GetHit 상태 활성화
                animator.SetBool("Walk", false);
                animator.SetTrigger("GetHit");
                Debug.Log("패턴중지");
                // 패턴 진행 중 일시적으로 패턴을 멈추고 일정 시간 동안 패턴이 끊어지지 않도록 설정
                StartCoroutine(LockPatternForDuration(patternLockTime));
                StartCoroutine(TakeDamageCooldown());
            }
        }
    }

    private IEnumerator LockPatternForDuration(float duration)
    {
        isPatternLocked = true;
        yield return new WaitForSeconds(duration);
        isPatternLocked = false; // 일정 시간이 지나면 패턴을 다시 끊을 수 있음
    }

    private IEnumerator TakeDamageCooldown()
    {
        yield return new WaitForSeconds(patternPauseTime); // 일정 시간 동안 피격 후 패턴이 끊어지지 않도록 유지
        canTakeHit = true; // 다시 피격 가능하도록 설정
        isTakingHit = false; // GetHit 상태 비활성화
    }
}
