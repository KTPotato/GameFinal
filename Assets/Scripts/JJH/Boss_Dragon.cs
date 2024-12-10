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
    private bool isTakingHit = false; // GetHit 상태 플래그
    private bool canTakeHit = true; // 피격 가능한지 여부
    public bool isFlying = false; // Fly 상태
    private bool isPatternPaused = false; // 패턴이 일시 중단되었는지 여부
    private bool isPatternLocked = false; // 패턴을 다시 끊지 않도록 할 변수
    private float patternLockTime = 10f; // 패턴이 끊기지 않는 시간 (10초)
    private float patternPauseTime = 2f; // 피격 후 패턴 일시 중단 시간 (2초)

    // 공격 관련 변수
    private float attackRange = 10f;
    private float attackCooldown = 4f;
    private float lastAttackTime = 0f;

    private float flyFlameCooldown = 10f; // fly 불뿜기 쿨타임
    private float lastFlyFlameTime = 0f;

    public int damageCount = 0;  // 타격을 받은 횟수 추적
    private bool isBlocking = false;  // 막기 상태 추적

    public bool isInFlightAnimation = false;

    public List<GameObject> allTargets = new List<GameObject>();

    private float stateStartTime; // 날기로 전환하기까지의 시간

    // 상태 관리
    public float hitCooldown = 10f;

    private Queue<int> recentPatterns = new Queue<int>(); // 최근 패턴 기록
    private int maxRepeat = 2; // 동일 패턴 최대 반복 횟수

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

        HpCheck();

        if (!animator.GetBool("Fly") && !isFlying)
        {
            // 상태 전환 체크
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

        // 타겟 거리 기반으로 WR_Point 업데이트
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // 초기화가 완료되었을 때만 스피드를 업데이트
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
            animator.SetFloat("WR_Point", 0.2f); // 기본값 설정
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
                else if (!isAttacking && Time.time - lastFlyFlameTime >= flyFlameCooldown && isFlying && !isInFlightAnimation)
                {
                    // 비행 애니메이션이 끝난 후에만 불을 뿜을 수 있도록 조건 추가
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

        // 공격 중이라면 Fly로 전환하지 않음
        if (isAttacking || IsInAttackAnimation())
        {
            return;
        }
        // 스스로의 콜라이더 끄기
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
        // 스스로의 콜라이더 끄기
        isFlying = true; // Fly 상태로 전환
        isInFlightAnimation = true;
        bossagent.isStopped = true;
        animator.SetBool("Fly", true);
        isAttacking = false; // 현재 진행 중인 공격 패턴 중단
        stateStartTime = Time.time; // Fly 상태 시작 시간 초기화
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

        // 먼저 비행 상태를 끝내는 트리거를 설정
        animator.SetTrigger("Land");


        isInFlightAnimation = true;

        // 비행 애니메이션이 끝났을 때 Walk 상태로 전환하도록
        animator.SetBool("Fly", false);

        bossagent.isStopped = true;
        isAttacking = false;
        stateStartTime = Time.time;  // Transition 시간을 설정
        StartCoroutine(FlyToWalk());
    }

    private IEnumerator FlyToWalk()
    {
        yield return new WaitForSeconds(4.5f);  // 5초 후 Walk로 전환
        isFlying = false;  
        isInFlightAnimation = false;
        bossagent.isStopped = false;
        // 스스로의 콜라이더 켜기
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
        }
    }
    private void Attack4(Transform target)
    {
        // 비행 중이거나, 이미 공격 중이거나, 패턴이 일시 정지 상태이면 리턴
        // 또한, 마지막 불 뿜기 이후 10초가 지나지 않았다면 리턴
        if (isAttacking || isPatternPaused || !isFlying || Time.time - lastFlyFlameTime < 10f || isInFlightAnimation) return;

        isAttacking = true;
        animator.SetTrigger("Fly Flame"); // 비행 상태에서 불 뿜기 트리거
        Fly_Flame(target.position); // 불 뿜기 함수 실행

        lastFlyFlameTime = Time.time; // 마지막 불 뿜기 시간 업데이트
        StartCoroutine(ResetAttackState2());
    }


    private void Fly_Flame(Vector3 targetPosition) // 날아서 불뿜기
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
        // 공격 애니메이션이 끝날 때까지 기다리기
        while (IsInAttackAnimation2())
        {
            yield return null;
        }

        // 애니메이션이 끝나면 공격이 끝났으므로 상태 리셋
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
    private void Attack2(Transform target) // 박치기
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

    private void Attack3(Transform target) // 불뿜기
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
        float timeToMove = 2.0f; 
        float elapsedTime = 0f;

        // 지속적으로 FlamePoint를 따라가도록 이동
        while (elapsedTime < timeToMove)
        {
            // FlamePoint의 현재 위치를 추적 (회전은 45도 추가된 값을 사용)
            Vector3 targetPosition = FlamePoint.position;
            Quaternion targetRotation = FlamePoint.rotation * Quaternion.Euler(15f, -90f, 0f);

            targetPosition.x = FlamePoint.position.x;
            targetPosition.y = FlamePoint.position.y;
            targetPosition.z = FlamePoint.position.z;

            // Lerp를 사용하여 flameEffect가 FlamePoint로 이동하도록 설정
            flameEffect.transform.position = Vector3.Lerp(flameEffect.transform.position, targetPosition, elapsedTime / timeToMove);
            flameEffect.transform.rotation = Quaternion.Slerp(flameEffect.transform.rotation, targetRotation, elapsedTime / timeToMove);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        Destroy(flameEffect, 0.5f);
    }


    private IEnumerator ResetAttackState()
    {

        // 공격 애니메이션이 끝날 때까지 기다리기
        while (IsInAttackAnimation())
        {
            yield return null;
        }

        // 애니메이션이 끝나면 공격이 끝났으므로 상태 리셋
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
            // 방어 중이고 DefendPoint에 충돌했을 경우 체력 감소 방지
            Debug.Log("Defend active: No damage taken.");
            return; // 데미지 무효화
        }
    }
    private bool IsDefendPoint(Collider other)
    {
        // DefendPoint와 충돌했는지 확인
        return other.bounds.Intersects(DefendPoint.GetComponent<Collider>().bounds);
    }

    private void OnTriggerExit(Collider other)
    {
        Hp -= other.GetComponent<BulletCtrl>().Pdmg;
    }
        public void TakeDamage()
    {
        if (isDead || isTakingHit) return;

        Debug.Log("맞음");
        damageCount++;  // 타격 횟수 증가

        // 타격 횟수가 8번에 도달하면 막기 시전
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
                Debug.Log("패턴중지");


                StartCoroutine(LockPatternForDuration(patternLockTime));
                StartCoroutine(TakeDamageCooldown());
            }
        }
    }
    private void Defend()
    {
        isBlocking = true;
        animator.SetTrigger("Defend");  // 막기 애니메이션 트리거
        DefendPoint.SetActive(true);
        StartCoroutine(ResetDamageCountAfterBlock());
        //부딫힌 총알 삭제
    }

    private IEnumerator ResetDamageCountAfterBlock()
    {
        // 막기 애니메이션이 끝날 때까지 기다림
        yield return new WaitForSeconds(2f);  // 막기 애니메이션 시간 (2초 예시)
        DefendPoint.SetActive(false);
        // 막기 종료 후 카운트 리셋
        damageCount = 0;
        isBlocking = false;  // 막기 상태 초기화
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