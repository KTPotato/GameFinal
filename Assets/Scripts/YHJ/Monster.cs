using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    private NavMeshAgent _monster;
    private GameObject _target;
    private Animator _animator;

    public float health = 50f;
    public float attackRange = 1.5f; // 공격 범위
    public float detectionRange = 10f; // 플레이어를 감지하는 범위

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
            // 플레이어를 감지하고 추적
            _lockOn = true;
        }
        else
        {
            // 플레이어 감지 범위를 벗어나면 추적 중지
            _lockOn = false;
        }

        if (_lockOn)
        {
            _monster.isStopped = false;
            _monster.SetDestination(_target.transform.position);

            if (distanceToPlayer > attackRange)
            {
                // 공격 범위 밖: 이동
                _animator.SetBool("isWalking", true);
                _animator.SetBool("isAttacking", false);
            }
            else
            {
                // 공격 범위 안: 공격
                _monster.isStopped = true; // 멈춤
                _animator.SetBool("isWalking", false);
                _animator.SetBool("isAttacking", true);
            }
        }
        else
        {
            // 플레이어를 감지하지 못하면 멈춤
            _monster.isStopped = true;
            _animator.SetBool("isWalking", false);
            _animator.SetBool("isAttacking", false);
        }
    }
}
