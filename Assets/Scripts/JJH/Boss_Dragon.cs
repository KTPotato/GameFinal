using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class Boss_Dragon : MonoBehaviour
{
    public NavMeshAgent bossagent;
    public float maxHp;
    public float Hp;
    public float dmg;

    private Animator animator;

    private bool isDead = false;
    private bool isAttacking = false;
    private bool isTakingHit = false; // GetHit ���� �÷���
    private bool canTakeHit = true; // �ǰ� �������� ����
    public bool isFlying = false; // Fly ����
    private bool isPatternPaused = false; // ������ �Ͻ� �ߴܵǾ����� ����
    private bool isPatternLocked = false; // ������ �ٽ� ���� �ʵ��� �� ����
    private float patternLockTime = 10f; // ������ ������ �ʴ� �ð� (10��)
    private float patternPauseTime = 2f; // �ǰ� �� ���� �Ͻ� �ߴ� �ð� (2��)

    // ���� ���� ����
    private float attackRange = 10f;
    private float attackCooldown = 4f;
    private float lastAttackTime = 0f;

    private float flyFlameCooldown = 10f; // fly �һձ� ��Ÿ��
    private float lastFlyFlameTime = 0f;

    public int damageCount = 0;  // Ÿ���� ���� Ƚ�� ����
    private bool isBlocking = false;  // ���� ���� ����

    public bool isInFlightAnimation = false;

    public List<GameObject> allTargets = new List<GameObject>();

    private float stateStartTime; // ����� ��ȯ�ϱ������ �ð�

    // ���� ����
    public float hitCooldown = 10f;

    private Queue<int> recentPatterns = new Queue<int>(); // �ֱ� ���� ���
    private int maxRepeat = 2; // ���� ���� �ִ� �ݺ� Ƚ��

    public GameObject Flame;
    public Transform FlamePoint;

    public GameObject BitePoint;
    public GameObject DrivePoint;
    public GameObject DefendPoint;

    public bool isInitialized = false;

    [SerializeField] private Image hpImage;
    public GameObject HpBar;

    private void Start()
    {
        maxHp = 5000;
        Hp = 5000;
        bossagent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        StartCoroutine(InitializeAfterDelay());

        HpBar = GameObject.FindGameObjectWithTag("Hp");
        hpImage = HpBar.GetComponent<Image>();
    }
    public void SpawnPlayer()
    {
        allTargets.Clear();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        allTargets.AddRange(players);
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

        HpCheck();

        if (!animator.GetBool("Fly") && !isFlying)
        {
            // ���� ��ȯ üũ
            if (Time.time - stateStartTime >= 60f && !isFlying)
            {
                TransitionToFly();
                return;
            }
        }
            if (animator.GetBool("Fly") && isFlying)
        {
            if (Time.time - stateStartTime >= 30f)
            {
                TransitionToUnder();
                return;
            }
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
                if (distanceToTarget > 30f)
                {
                    animator.SetFloat("WR_Point", 0.7f);
                    animator.SetFloat("FG_Point", 0.7f);
                    bossagent.speed = 7f;
                }
                else
                {
                    animator.SetFloat("WR_Point", 0.2f);
                    animator.SetFloat("FG_Point", 0.2f);
                    bossagent.speed = 3.5f;
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
                animator.SetBool("Walk", false);

                if (!isAttacking && Time.time - lastAttackTime >= attackCooldown && !isPatternPaused && !isFlying)
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
                else if (!isAttacking && Time.time - lastFlyFlameTime >= flyFlameCooldown && isFlying && !isInFlightAnimation)
                {
                    // ���� �ִϸ��̼��� ���� �Ŀ��� ���� ���� �� �ֵ��� ���� �߰�
                    Attack4(target);
                }

            }
            else
            {
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

        if (isFlying)
        {
            animator.SetBool("Walk", false);
        }
        else if (!IsInAttackAnimation2() && !IsInAttackAnimation() && !isAttacking)
        {
            animator.SetBool("Walk", true);
        }
    }
    private void TransitionToFly()
    {

        // ���� ���̶�� Fly�� ��ȯ���� ����
        if (isAttacking || IsInAttackAnimation())
        {
            return;
        }
        // �������� �ݶ��̴� ����
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
        // �������� �ݶ��̴� ����
        isFlying = true; // Fly ���·� ��ȯ
        isInFlightAnimation = true;
        bossagent.isStopped = true;
        animator.SetBool("Fly", true);
        isAttacking = false; // ���� ���� ���� ���� ���� �ߴ�
        stateStartTime = Time.time; // Fly ���� ���� �ð� �ʱ�ȭ
        StartCoroutine(FalseFight());
    }

    private IEnumerator FalseFight()
    {
        yield return new WaitForSeconds(6.5f);
        isInFlightAnimation = false;
        bossagent.isStopped = false;
    }
    private void TransitionToUnder()
    {
        if (isAttacking || IsInAttackAnimation2())
        {
            return;
        }

        // ���� ���� ���¸� ������ Ʈ���Ÿ� ����
        animator.SetTrigger("Land");


        isInFlightAnimation = true;

        // ���� �ִϸ��̼��� ������ �� Walk ���·� ��ȯ�ϵ���
        animator.SetBool("Fly", false);

        bossagent.isStopped = true;
        isAttacking = false;
        stateStartTime = Time.time;  // Transition �ð��� ����
        StartCoroutine(FlyToWalk());
    }

    private IEnumerator FlyToWalk()
    {
        yield return new WaitForSeconds(4.5f);  // 5�� �� Walk�� ��ȯ
        isFlying = false;  
        isInFlightAnimation = false;
        bossagent.isStopped = false;
        // �������� �ݶ��̴� �ѱ�
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
        }
    }
    private void Attack4(Transform target)
    {
        // ���� ���̰ų�, �̹� ���� ���̰ų�, ������ �Ͻ� ���� �����̸� ����
        // ����, ������ �� �ձ� ���� 10�ʰ� ������ �ʾҴٸ� ����
        if (isAttacking || isPatternPaused || !isFlying || Time.time - lastFlyFlameTime < 10f || isInFlightAnimation) return;

        isAttacking = true;
        animator.SetTrigger("Fly Flame"); // ���� ���¿��� �� �ձ� Ʈ����
        Fly_Flame(target.position); // �� �ձ� �Լ� ����

        lastFlyFlameTime = Time.time; // ������ �� �ձ� �ð� ������Ʈ
        StartCoroutine(ResetAttackState2());
    }


    private void Fly_Flame(Vector3 targetPosition) // ���Ƽ� �һձ�
    {
        StartCoroutine(ResetFly_FlameState());
    }

    private IEnumerator ResetFly_FlameState()
    {
        yield return new WaitForSeconds(0.8f);

        Vector3 adjustedPosition = FlamePoint.position + FlamePoint.forward;
        Quaternion adjustedRotation = FlamePoint.rotation * Quaternion.Euler(30f, 90f, 0f);

        GameObject flameEffect = Instantiate(Flame, adjustedPosition, adjustedRotation);

        StartCoroutine(MoveFlameToPoint2(flameEffect));
    }

    private IEnumerator MoveFlameToPoint2(GameObject flameEffect)
    {
        float timeToMove = 2.0f;
        float elapsedTime = 0f;

        while (elapsedTime < timeToMove)
        {
            Vector3 targetPosition = FlamePoint.position;
            Quaternion targetRotation = FlamePoint.rotation * Quaternion.Euler(30f, -90f, 0f);

            targetPosition.x = FlamePoint.position.x;
            targetPosition.y = FlamePoint.position.y;
            targetPosition.z = FlamePoint.position.z;

            flameEffect.transform.position = Vector3.Lerp(flameEffect.transform.position, targetPosition, elapsedTime / timeToMove);
            flameEffect.transform.rotation = Quaternion.Slerp(flameEffect.transform.rotation, targetRotation, elapsedTime / timeToMove);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        Destroy(flameEffect, 0.5f);
    }

    private IEnumerator ResetAttackState2()
    {
        // ���� �ִϸ��̼��� ���� ������ ��ٸ���
        while (IsInAttackAnimation2())
        {
            yield return null;
        }

        // �ִϸ��̼��� ������ ������ �������Ƿ� ���� ����
        isAttacking = false;
        yield return new WaitForSeconds(0.7f);
    }

    private bool IsInAttackAnimation2()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName("Fly Flame");
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
        if (isAttacking || isPatternPaused || isFlying) return;

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
        isInitialized = false;
        bossagent.speed = 0;
        BitePoint.SetActive(true);
        yield return new WaitForSeconds(1.1f);
        bossagent.speed = 3.5f;
        isInitialized = true;
        if(BitePoint != null)
        {
            BitePoint.SetActive(false);
        }
    }
    private void Attack2(Transform target) // ��ġ��
    {
        if (isAttacking || isPatternPaused || isFlying) return;

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
        isInitialized = false;
        bossagent.speed = 0;
        DrivePoint.SetActive(true);
        yield return new WaitForSeconds(2.3f);
        bossagent.speed = 3.5f;
        isInitialized = true;
        if (DrivePoint != null)
        {
            DrivePoint.SetActive(false);
        }
    }

    private void Attack3(Transform target) // �һձ�
    {
        if (isAttacking || isPatternPaused || isFlying) return;

        isAttacking = true;
        animator.SetTrigger("Flame");
        Under_Flame(target.position);

        lastAttackTime = Time.time;
        StartCoroutine(ResetAttackState());
    }

    private void Under_Flame(Vector3 targetPosition)
    {
        StartCoroutine(ResetUnder_FlameState());
        StartCoroutine(Stop());
    }
    private IEnumerator Stop()
    {
        isInitialized = false;
        bossagent.speed = 0;
        yield return new WaitForSeconds(3.0f);
        bossagent.speed = 3.5f;
        isInitialized = true;
    }
    private IEnumerator ResetUnder_FlameState()
    {
        yield return new WaitForSeconds(0.8f);

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
        float timeToMove = 2.0f; 
        float elapsedTime = 0f;

        // ���������� FlamePoint�� ���󰡵��� �̵�
        while (elapsedTime < timeToMove)
        {
            // FlamePoint�� ���� ��ġ�� ���� (ȸ���� 45�� �߰��� ���� ���)
            Vector3 targetPosition = FlamePoint.position;
            Quaternion targetRotation = FlamePoint.rotation * Quaternion.Euler(15f, -90f, 0f);

            targetPosition.x = FlamePoint.position.x;
            targetPosition.y = FlamePoint.position.y;
            targetPosition.z = FlamePoint.position.z;

            // Lerp�� ����Ͽ� flameEffect�� FlamePoint�� �̵��ϵ��� ����
            flameEffect.transform.position = Vector3.Lerp(flameEffect.transform.position, targetPosition, elapsedTime / timeToMove);
            flameEffect.transform.rotation = Quaternion.Slerp(flameEffect.transform.rotation, targetRotation, elapsedTime / timeToMove);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        Destroy(flameEffect, 0.5f);
    }


    private IEnumerator ResetAttackState()
    {

        // ���� �ִϸ��̼��� ���� ������ ��ٸ���
        while (IsInAttackAnimation())
        {
            yield return null;
        }

        // �ִϸ��̼��� ������ ������ �������Ƿ� ���� ����
        isAttacking = false;
        yield return new WaitForSeconds(0.7f);
    }

    private bool IsInAttackAnimation()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName("Bite") || stateInfo.IsName("Driving") || stateInfo.IsName("Flame");
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        bossagent.isStopped = true;

        GetComponent<Collider>().enabled = false;

        Destroy(gameObject, 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerBullet")
        {
            Hp -= other.GetComponent<BulletCtrl>().Pdmg;
            TakeDamage();
            Destroy(other.gameObject);
        }
        if (isBlocking && IsDefendPoint(other))
        {
            // ��� ���̰� DefendPoint�� �浹���� ��� ü�� ���� ����
            Debug.Log("Defend active: No damage taken.");
            return; // ������ ��ȿȭ
        }
    }
    private bool IsDefendPoint(Collider other)
    {
        // DefendPoint�� �浹�ߴ��� Ȯ��
        return other.bounds.Intersects(DefendPoint.GetComponent<Collider>().bounds);
    }

    private void OnTriggerExit(Collider other)
    {
        Hp -= other.GetComponent<BulletCtrl>().Pdmg;
    }
        public void TakeDamage()
    {
        if (isDead || isTakingHit) return;

        Debug.Log("����");
        damageCount++;  // Ÿ�� Ƚ�� ����

        // Ÿ�� Ƚ���� 8���� �����ϸ� ���� ����
        if (damageCount >= 8 && !isBlocking)
        {
            Defend();
        }

        if (!isPatternLocked)
        {
            if (canTakeHit)
            {
                bossagent.speed = 0;
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
    private void Defend()
    {
        isBlocking = true;
        animator.SetTrigger("Defend");  // ���� �ִϸ��̼� Ʈ����
        DefendPoint.SetActive(true);
        StartCoroutine(ResetDamageCountAfterBlock());
        //�΋H�� �Ѿ� ����
    }

    private IEnumerator ResetDamageCountAfterBlock()
    {
        // ���� �ִϸ��̼��� ���� ������ ��ٸ�
        yield return new WaitForSeconds(2f);  // ���� �ִϸ��̼� �ð� (2�� ����)
        DefendPoint.SetActive(false);
        // ���� ���� �� ī��Ʈ ����
        damageCount = 0;
        isBlocking = false;  // ���� ���� �ʱ�ȭ
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
        bossagent.speed = 3.5f;
        canTakeHit = true;
        isTakingHit = false;
    }

    void HpCheck()
    {
        hpImage.fillAmount = Hp / maxHp;
    }
}