using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    private NavMeshAgent _monster;
    private GameObject _target;
    private Vector3 _lastKnownPosition;
    private bool _lockOn;

    public float health = 50f;
    public float attackRange = 1.5f; // 공격 범위
    public float stoppingDistance = 0.5f; // 몬스터가 이동을 멈추는 거리

    private int iters = 0;
    private int _updateInterval = 10; // 10 프레임마다 업데이트

    private Animator _animator;

    void Start()
    {
        _monster = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _target = GameObject.FindWithTag("Player");
        _lastKnownPosition = transform.position;
        _lockOn = false;
    }

    void Update()
    {
        // 일정 주기로 시야 검사
        if (iters % _updateInterval == 0)
        {
            CheckVisibility();
        }

        // 플레이어가 시야에 있을 때는 추적
        if (_lockOn)
        {
            _lastKnownPosition = _target.transform.position;
            float distanceToPlayer = Vector3.Distance(transform.position, _lastKnownPosition);

            if (distanceToPlayer > attackRange) // 공격 범위 바깥이면 이동
            {
                _monster.isStopped = false;
                _monster.destination = _lastKnownPosition;
                _animator.SetBool("isWalking", true);
                _animator.SetBool("isAttacking", false);
            }
            else // 공격 범위 안이면 멈추고 공격
            {
                _monster.isStopped = true;
                _animator.SetBool("isWalking", false);
                _animator.SetBool("isAttacking", true);
            }
        }
        else
        {
            // 시야를 잃었고 마지막 위치에 도달했을 경우 멈춤
            if (Vector3.Distance(transform.position, _lastKnownPosition) < stoppingDistance)
            {
                _monster.isStopped = true;
                _animator.SetBool("isWalking", false);
            }
            else
            {
                _monster.isStopped = false;
                _monster.destination = _lastKnownPosition;
                _animator.SetBool("isWalking", true);
            }

            _animator.SetBool("isAttacking", false);
        }

        iters++;
    }

    void CheckVisibility()
    {
        Ray ray = new Ray(transform.position, _target.transform.position - transform.position);
        RaycastHit hit;

        // 플레이어가 시야에 있는지 확인
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.CompareTag("Player"))
            {
                _lockOn = true;
                _lastKnownPosition = _target.transform.position;
            }
            else
            {
                _lockOn = false;
            }
        }
    }
}
