using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterTurtleShell : MonoBehaviour
{
    private NavMeshAgent _monster;
    private GameObject _target;
    private Vector3 _lastKnownPosition;
    private bool _lockOn;

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
            Ray ray = new Ray(transform.position, _target.transform.position - transform.position);
            RaycastHit hit;

            // 플레이어가 시야에 있는지 확인
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    // 플레이어가 시야에 들어오면 추적 시작
                    _lockOn = true;
                    _lastKnownPosition = _target.transform.position;
                    _monster.destination = _lastKnownPosition;
                }
                else
                {
                    // 시야에서 벗어나면 마지막 위치로 이동
                    _lockOn = false;
                    _monster.destination = _lastKnownPosition;
                }
            }
        }

        // 플레이어가 시야에 있을 때는 계속해서 추적
        if (_lockOn)
        {
            _lastKnownPosition = _target.transform.position;
            _monster.destination = _lastKnownPosition;
        }

        // 마지막 위치에 도달 시 멈춤
        if (!_lockOn && Vector3.Distance(transform.position, _lastKnownPosition) < 0.5f)
        {
            _monster.destination = transform.position;
        }

        // 애니메이션 상태 업데이트
        UpdateAnimations();

        iters++;
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
