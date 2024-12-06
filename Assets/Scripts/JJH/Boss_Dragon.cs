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
    private bool isTakingHit = false; // GetHit ���� �÷���
    private bool canTakeHit = true; // �ǰ� �������� ����
    private bool isFlying = false; // Fly ����
    private bool canDefend = true; // ��� ���� ����
    private bool isPatternPaused = false; // ������ �Ͻ� �ߴܵǾ����� ����
    private bool isPatternLocked = false; // ������ �ٽ� ���� �ʵ��� �� ����
    private float patternLockTime = 10f; // ������ ������ �ʴ� �ð� (10��)
    private float patternPauseTime = 2f; // �ǰ� �� ���� �Ͻ� �ߴ� �ð� (2��)

    // ���� ���� ����
    private float attackRange = 9f;
    private float attackCooldown = 3f;
    private float lastAttackTime = 0f;

    public List<GameObject> allTargets = new List<GameObject>();


    // ���� ����
    public float hitCooldown = 10f;

    private Queue<int> recentPatterns = new Queue<int>(); // �ֱ� ���� ���
    private int maxRepeat = 2; // ���� ���� �ִ� �ݺ� Ƚ��

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
        yield return new WaitForSeconds(3f); // 3�� ���
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

        // Ÿ�� �Ÿ� ������� WR_Point ������Ʈ
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // �ʱ�ȭ�� �Ϸ�Ǿ��� ���� ���ǵ带 ������Ʈ
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
            animator.SetFloat("WR_Point", 0.2f); // �⺻�� ����
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
                            Attack1(target); // Bite ����
                            break;
                        case 1:
                            Attack2(target); // Driving ��ġ��
                            break;
                        case 2:
                            Attack3(target); // Flame �һձ�
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

        // ���� ť�� ���� �߰�
        recentPatterns.Enqueue(attackPattern);

        // ť ũ�� �ʰ��� ���� ������ ����
        if (recentPatterns.Count > maxRepeat)
        {
            recentPatterns.Dequeue();
        }

        return attackPattern;
    }

    private void Attack1(Transform target) // ����
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
    private void Attack2(Transform target) // ��ġ��
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

    private void Attack3(Transform target) // �һձ�
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

        // FlamePoint�� ������ �������� 2 units �� ������ �ϱ�
        Vector3 adjustedPosition = FlamePoint.position + FlamePoint.forward; 
        Quaternion adjustedRotation = FlamePoint.rotation * Quaternion.Euler(15f, 90f, 0f); // 45�� ������ ȸ�� �߰�

        // 2f �� ������ �� ��ġ�� ȸ�������� FlameEffect ����
        GameObject flameEffect = Instantiate(Flame, adjustedPosition, adjustedRotation);

        // FlameEffect�� FlamePoint�� ��� ���󰡵��� ���� (ȸ�� ����)
        StartCoroutine(MoveFlameToPoint(flameEffect));
    }

    private IEnumerator MoveFlameToPoint(GameObject flameEffect)
    {
        float timeToMove = 1.5f; 
        float elapsedTime = 0f;

        // ���������� FlamePoint�� ���󰡵��� �̵�
        while (elapsedTime < timeToMove)
        {
            // FlamePoint�� ���� ��ġ�� ���� (ȸ���� 45�� �߰��� ���� ���)
            Vector3 targetPosition = FlamePoint.position + FlamePoint.forward;
            Quaternion targetRotation = FlamePoint.rotation * Quaternion.Euler(15f, -90f, 0f);
            targetPosition.x -= 1.6f;
            targetPosition.y -= 0.4f;
            // Lerp�� ����Ͽ� flameEffect�� FlamePoint�� �̵��ϵ��� ����
            flameEffect.transform.position = Vector3.Lerp(flameEffect.transform.position, targetPosition, elapsedTime / timeToMove);
            flameEffect.transform.rotation = Quaternion.Slerp(flameEffect.transform.rotation, targetRotation, elapsedTime / timeToMove);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // FlamePoint ��ġ�� ȸ���� ��Ȯ�� ����
        flameEffect.transform.position = FlamePoint.position + FlamePoint.forward; 
        flameEffect.transform.rotation = FlamePoint.rotation * Quaternion.Euler(15f, -90f, 0f); 

        Destroy(flameEffect, 0.5f);
    }





    private IEnumerator ResetAttackState()
    {
        // ���� �ִϸ��̼��� ���� ������ ��ٸ���
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Bite") ||
               animator.GetCurrentAnimatorStateInfo(0).IsName("Driving") ||
               animator.GetCurrentAnimatorStateInfo(0).IsName("Flame"))
        {
            yield return null;
        }

        // �ִϸ��̼��� ������ ������ �������Ƿ� ���� ����
        isAttacking = false;

        // ���� �� �̵� ���·� ���ư���
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
        Debug.Log("����");
        if (!isPatternLocked)
        {
            if (canTakeHit)
            {
                canTakeHit = false;
                isTakingHit = true;
                animator.SetBool("Walk", false);
                animator.SetTrigger("GetHit");
                Debug.Log("��������");
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