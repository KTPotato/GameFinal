using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AlienMonster : MonoBehaviour
{
    public GameObject _target;            // 플레이어의 Transform
    public float attackRange = 10f;     // 공격 범위
    public float fieldOfView = 120f;    // 시야각
    public GameObject projectilePrefab; // 발사체 프리팹
    public Transform firePoint;         // 발사체가 나가는 위치
    public float attackCooldown = 1f;   // 공격 쿨타임
    public float health;        // 몬스터 체력
    

    private NavMeshAgent agent;         // NavMeshAgent 컴포넌트
    private float attackTimer = 0f;     // 공격 타이머
    private Animator _animator;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // NavMeshAgent 초기화
        _target = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (_target == null) return; // 플레이어가 설정되지 않았다면 종료

        // 플레이어를 따라감
        agent.SetDestination(_target.transform.position);

        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, _target.transform.position);

        // 시야 범위 안에 있는지 확인
        Vector3 directionToPlayer = (_target.transform.position - transform.position).normalized;
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
            Vector3 direction = (_target.transform.position - firePoint.position).normalized;
            projectile.transform.forward = direction;

            // Rigidbody를 통해 발사체에 속도 부여
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * 50f; // 발사체 속도 설정
            }

            // 충돌 비활성화
            //Collider projectileCollider = projectile.GetComponent<Collider>();
            //Collider alienCollider = GetComponent<Collider>();
            //if (projectileCollider != null && alienCollider != null)
            //{
            //    Physics.IgnoreCollision(projectileCollider, alienCollider);
            //}

            attackTimer = 0f; // 타이머 초기화
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerBullet")
        {
            TakeDamage(other.GetComponent<BulletCtrl>().Pdmg);
            Destroy(other.gameObject);
        }
    }


    // 몬스터가 데미지를 받을 때 호출되는 메서드
    public void TakeDamage(float damage)
    {
        Debug.Log($"TakeDamage called with damage: {damage}");
        health -= damage; // 체력 감소
        Debug.Log($"Remaining health: {health}");

        if (health <= 0)
        {
            Die(); // 체력이 0 이하일 경우 죽음 처리
        }
    }

    // 몬스터 사망 처리
    private void Die()
    {
        Debug.Log("AlienMonster died!");
        Destroy(gameObject); // 몬스터 오브젝트 파괴
    }
}
