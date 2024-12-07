using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    private NavMeshAgent _monster;
    private GameObject _target;
    private Animator _animator;

    public float health = 50f; // 몬스터 체력
    public float attackRange = 1.5f; // 공격 범위
    public float detectionRange = 10f; // 플레이어를 감지하는 범위
    public GameObject exp;

    private bool _lockOn;

    void Start()
    {
        _monster = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _target = GameObject.FindWithTag("Player");

        if (_monster == null || _animator == null || _target == null)
        {
            Debug.LogError("필요한 컴포넌트가 설정되지 않았습니다!");
        }

        _lockOn = false;
    }

    void Update()
    {
        if (_target == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _target.transform.position);

        if (distanceToPlayer <= detectionRange)
        {
            _lockOn = true;
        }
        else
        {
            _lockOn = false;
        }

        if (_lockOn)
        {
            _monster.isStopped = false;
            _monster.SetDestination(_target.transform.position);

            if (distanceToPlayer > attackRange)
            {
                _animator.SetBool("isWalking", true);
                _animator.SetBool("isAttacking", false);
            }
            else
            {
                _monster.isStopped = true;
                _animator.SetBool("isWalking", false);
                _animator.SetBool("isAttacking", true);
            }
        }
        else
        {
            _monster.isStopped = true;
            _animator.SetBool("isWalking", false);
            _animator.SetBool("isAttacking", false);
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
        health -= damage; // 체력 감소
        Debug.Log($"Monster took {damage} damage. Remaining health: {health}");

        if (health <= 0)
        {
            Die(); // 체력이 0 이하일 경우 죽음 처리
        }
    }

    // 몬스터 사망 처리
    // 몬스터 사망 처리
    private void Die()
    {
        Debug.Log("Monster died!");
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

        Destroy(gameObject); // 몬스터 오브젝트 파괴
    }

}
