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
    private bool isTakingHit = false; // GetHit ���� �÷���
    private bool canTakeHit = true; // �ǰ� �������� ����
    private bool isPatternPaused = false; // ������ �Ͻ� �ߴܵǾ����� ����
    private bool isPatternLocked = false; // ������ �ٽ� ���� �ʵ��� �� ����
    private float patternLockTime = 10f; // ������ ������ �ʴ� �ð� (10��)
    private float patternPauseTime = 2f; // �ǰ� �� ���� �Ͻ� �ߴ� �ð� (2��)

    // ���� ���� ����
    private float attackRange = 3f;
    private float attackCooldown = 3f;
    private float lastAttackTime = 0f;

    public List<GameObject> allTargets = new List<GameObject>();

    // ���� �ִϸ��̼� ����
    public bool isAttackingAnimation = false;
    public GameObject axePrefab; // ���� ������
    public Transform throwPoint;

    // ���� ����
    public float hitCooldown = 10f;

    private Queue<int> recentPatterns = new Queue<int>(); // �ֱ� ���� ���
    private int maxRepeat = 2; // ���� ���� �ִ� �ݺ� Ƚ��

    // blend tree �Ἥ �ָ� �޸��� ������ �ȱ�� �ؾߵ� 
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
                            Attack1(); // Auto ��Ÿ
                            break;
                        case 1:
                            Attack2(target); // ���� ������
                            break;
                        case 2:
                            Attack3(); // ���� �ֵθ���
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

    private void Attack1() // Auto ��Ÿ
    {
        if (isAttacking || isPatternPaused) return;

        isAttacking = true;
        animator.SetTrigger("Auto");

        lastAttackTime = Time.time;
        StartCoroutine(ResetAttackState());
    }

    private void Attack2(Transform target) // ���� ������
    {
        if (isAttacking || isPatternPaused) return;

        isAttacking = true;
        animator.SetTrigger("Swing");

        ThrowAxe(target.position);

        lastAttackTime = Time.time;
        StartCoroutine(ResetAttackState());
    }

    private void Attack3() // ���� �ֵθ���
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

        // Z������ 90�� ȸ�� ����
        axe.transform.rotation = Quaternion.Euler(axe.transform.rotation.eulerAngles + new Vector3(0, 0, 90));

        Rigidbody rb = axe.GetComponent<Rigidbody>();

        // �浹 ����
        Collider axeCollider = axe.GetComponent<Collider>();
        Collider throwPointCollider = throwPoint.GetComponent<Collider>();
        if (axeCollider != null && throwPointCollider != null)
        {
            Physics.IgnoreCollision(axeCollider, throwPointCollider);
        }

        if (rb != null)
        {
            Vector3 forwardDirection = throwPoint.forward; // ������ ����
            rb.velocity = Vector3.zero; // �ʱ� �ӵ� ����
            rb.AddForce(forwardDirection * 20f, ForceMode.Impulse); // ������ ������

            // ���ư��� ���� x�� ȸ��
            StartCoroutine(RotateAxeXAxis(axe));

            // ������ ���� �ð� �� ���ƿ����� ����
            yield return new WaitForSeconds(1.5f); // ���� �ð� ���� ���ư�

            rb.velocity = Vector3.zero; // �ӵ� �ʱ�ȭ
            rb.angularVelocity = Vector3.zero; // ȸ�� �ʱ�ȭ

            // ������ ���� ��ġ�� ��ǥ�� ����
            Vector3 returnPosition = throwPoint.position;

            // ���ƿ��� ���� ���
            Vector3 returnDirection = (returnPosition - axe.transform.position).normalized;
            rb.AddForce(returnDirection * 15f, ForceMode.Impulse); // ���ƿ��� ��

            // ���ƿ��� ���� ��ġ�� ���������� üũ�Ͽ� ���� �� �ı�
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
                rb.angularVelocity = new Vector3(0, 10f, 0); // x�����θ� ȸ��
            }
            yield return null;
        }
    }

    private IEnumerator DestroyAxeOnReturn(GameObject axe, Vector3 targetPosition)
    {
        while (axe != null)
        {
            // ������ ��ǥ ��ġ ��ó�� �����ߴ��� Ȯ��
            if (Vector3.Distance(axe.transform.position, targetPosition) < 0.5f)
            {
                Destroy(axe); // ���� �ı�
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
