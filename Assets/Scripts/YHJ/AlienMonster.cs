using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class AlienMonster : MonoBehaviour
{
    public GameObject player;            // 플레이어의 Transform
    public float attackRange = 10f;     // 공격 범위
    public float fieldOfView = 120f;    // 시야각
    public GameObject projectilePrefab; // 발사체 프리팹
    public Transform firePoint;         // 발사체가 나가는 위치
    public float attackCooldown = 1f;   // 공격 쿨타임
    private bool _lockOn;

    private NavMeshAgent agent;         // NavMeshAgent 컴포넌트
    private float attackTimer = 0f;     // 공격 타이머
    private Animator _animator;
    private NavMeshAgent _monster;
    private GameObject _target;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // NavMeshAgent 초기화
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return; // 플레이어가 설정되지 않았다면 종료

        // 플레이어를 따라감
        agent.SetDestination(player.transform.position);

        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // 시야 범위 안에 있는지 확인
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (distanceToPlayer <= attackRange && angleToPlayer <= fieldOfView / 2)
        {
            // 공격 조건 충족
            AttackPlayer();
        }
    }

    void AttackPlayer()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            // 발사체 생성
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            // 발사체의 방향 설정
            Vector3 direction = (player.transform.position - firePoint.position).normalized;
            projectile.transform.forward = direction;

            // Rigidbody를 통해 발사체에 속도 부여
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * 50f; // 발사체 속도 설정
            }

            attackTimer = 0f; // 타이머 초기화
        }
    }


    void UpdateAnimations()
    {
        // 이동 상태에 따른 애니메이션
        bool isWalking = _monster.velocity.magnitude > 0.1f;
        _animator.SetBool("isWalking", isWalking);

        // 공격 상태를 확인
        float distanceToTarget = Vector3.Distance(transform.position, _target.transform.position);
        bool isAttacking = _lockOn && distanceToTarget < 2.0f; // 공격 범위 예: 2.0f
        _animator.SetBool("isAttacking", isAttacking);
    }
}
