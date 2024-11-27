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
    private int _updateInterval = 10; // 10 �����Ӹ��� ������Ʈ

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
        // ���� �ֱ�� �þ� �˻�
        if (iters % _updateInterval == 0)
        {
            Ray ray = new Ray(transform.position, _target.transform.position - transform.position);
            RaycastHit hit;

            // �÷��̾ �þ߿� �ִ��� Ȯ��
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    // �÷��̾ �þ߿� ������ ���� ����
                    _lockOn = true;
                    _lastKnownPosition = _target.transform.position;
                    _monster.destination = _lastKnownPosition;
                }
                else
                {
                    // �þ߿��� ����� ������ ��ġ�� �̵�
                    _lockOn = false;
                    _monster.destination = _lastKnownPosition;
                }
            }
        }

        // �÷��̾ �þ߿� ���� ���� ����ؼ� ����
        if (_lockOn)
        {
            _lastKnownPosition = _target.transform.position;
            _monster.destination = _lastKnownPosition;
        }

        // ������ ��ġ�� ���� �� ����
        if (!_lockOn && Vector3.Distance(transform.position, _lastKnownPosition) < 0.5f)
        {
            _monster.destination = transform.position;
        }

        // �ִϸ��̼� ���� ������Ʈ
        UpdateAnimations();

        iters++;
    }

    void UpdateAnimations()
    {
        // �̵� ���¿� ���� �ִϸ��̼�
        bool isWalking = _monster.velocity.magnitude > 0.1f;
        _animator.SetBool("isWalking", isWalking);

        // ���� ���¸� Ȯ��
        float distanceToTarget = Vector3.Distance(transform.position, _target.transform.position);
        bool isAttacking = _lockOn && distanceToTarget < 2.0f; // ���� ���� ��: 2.0f
        _animator.SetBool("isAttacking", isAttacking);
    }
}
