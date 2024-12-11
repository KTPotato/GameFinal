using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AlienMonster : MonoBehaviour
{
    public GameObject _target;            // 플레이어의 Transform
    public float attackRange = 10f;     // 공격 범위
    public float fieldOfView = 120f;    // 시야각
    public GameObject projectilePrefab; // 발사체 프리팹
    public Transform FirePoint;         // 발사체가 나가는 위치
    public float attackCooldown = 1f;   // 공격 쿨타임
    public float health;        // 몬스터 체력
    public GameObject exp;
    public GameObject heart;
    

    private NavMeshAgent agent;         // NavMeshAgent 컴포넌트
    private float attackTimer = 0f;     // 공격 타이머
    private Animator _animator;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // NavMeshAgent 초기화
        _target = GameObject.FindWithTag("Player");
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (_target == null) return; // 플레이어가 설정되지 않았다면 종료

        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, _target.transform.position);

        // NavMeshAgent가 멈추는 거리를 설정
        agent.stoppingDistance = attackRange / 2; // 일정 거리 유지 (예: 공격 범위의 절반)

        // 플레이어를 따라감 (목표 지점 업데이트)
        if (distanceToPlayer > agent.stoppingDistance) // 플레이어와의 거리가 유지 거리보다 멀면 추적
        {
            agent.SetDestination(_target.transform.position);
        }
        else
        {
            agent.ResetPath(); // 너무 가까워지면 멈추도록 경로 초기화
        }

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
            if (projectilePrefab == null)
            {
                //Debug.LogError("Projectile Prefab is not assigned!");
                return;
            }

            if (FirePoint == null)
            {
                //Debug.LogError("FirePoint is not assigned!");
                return;
            }

            // 발사체 생성
            GameObject projectile = Instantiate(projectilePrefab, FirePoint.position, FirePoint.rotation);
            //Debug.Log($"Projectile created at: {FirePoint.position}");
            _animator.Play("Attack01");

            // EBulletCtrl 스크립트를 찾아 초기화
            EBulletCtrl bulletCtrl = projectile.GetComponent<EBulletCtrl>();
            if (bulletCtrl != null)
            {
                bulletCtrl.bulletSpeed = 300f; // 필요에 따라 속도 설정
                bulletCtrl.Edmg = 10f;        // 필요에 따라 데미지 설정
            }
            else
            {
                //Debug.LogError("EBulletCtrl script is missing on the projectile prefab!");
            }

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

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "spinball")
        {
            TakeDamage(other.GetComponent<BulletCtrl>().Pdmg);
        }
    }



    // 몬스터가 데미지를 받을 때 호출되는 메서드
    public void TakeDamage(float damage)
    {
        //Debug.Log($"TakeDamage called with damage: {damage}");
        health -= damage; // 체력 감소
        //Debug.Log($"Remaining health: {health}");

        if (health <= 0)
        {
            Die(); // 체력이 0 이하일 경우 죽음 처리
        }
    }

    
    // 몬스터 사망 처리
    private void Die()
    {
        //Debug.Log("AlienMonster died!");
        int rand = Random.Range(5, 8); // 랜덤으로 생성할 경험치 구슬 개수 설정

        for (int i = 0; i < rand; i++)
        {
            // 경험치 구슬을 몬스터 주변의 랜덤한 위치에 생성
            Vector3 randomOffset = new Vector3(
                Random.Range(-1f, 1f),  // X축 랜덤 위치
                Random.Range(0f, 1f),  // Y축 약간 위로 띄우기
                Random.Range(-1f, 1f)  // Z축 랜덤 위치
            );

            Vector3 spawnPosition = transform.position + randomOffset;

            Instantiate(exp, spawnPosition, Quaternion.identity);
        }

        // 5% 확률로 Heart 프리팹 생성
        float dropChance = Random.Range(0f, 100f);
        if (dropChance <= 5f)
        {
            Vector3 heartSpawnPosition = transform.position + Vector3.up; // 몬스터 위치 위에 생성
            Instantiate(heart, heartSpawnPosition, Quaternion.identity);
            //Debug.Log("Heart dropped!");
        }

        Destroy(gameObject); // 몬스터 오브젝트 파괴
    }

}
