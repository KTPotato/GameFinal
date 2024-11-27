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
    public float attackRange = 1.5f; // ���� ����
    public float stoppingDistance = 0.5f; // ���Ͱ� �̵��� ���ߴ� �Ÿ�

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
            CheckVisibility();
        }

        // �÷��̾ �þ߿� ���� ���� ����
        if (_lockOn)
        {
            _lastKnownPosition = _target.transform.position;
            float distanceToPlayer = Vector3.Distance(transform.position, _lastKnownPosition);

            if (distanceToPlayer > attackRange) // ���� ���� �ٱ��̸� �̵�
            {
                _monster.isStopped = false;
                _monster.destination = _lastKnownPosition;
                _animator.SetBool("isWalking", true);
                _animator.SetBool("isAttacking", false);
            }
            else // ���� ���� ���̸� ���߰� ����
            {
                _monster.isStopped = true;
                _animator.SetBool("isWalking", false);
                _animator.SetBool("isAttacking", true);
            }
        }
        else
        {
            // �þ߸� �Ҿ��� ������ ��ġ�� �������� ��� ����
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

        // �÷��̾ �þ߿� �ִ��� Ȯ��
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
