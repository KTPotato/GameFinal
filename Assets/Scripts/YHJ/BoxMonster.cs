using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BoxMonster : MonoBehaviour
{
    private NavMeshAgent _boxmonster;
    private GameObject _target;
    private Animator _animator;

    public float health = 30.0f;    // ü��
    public float attackRange = 2f;   // ���� ����
    public float detectionRange = 5f;   // �÷��̾ �����ϴ� �Ÿ�

    private bool _lockOn;

    // Start is called before the first frame update
    void Start()
    {
        _boxmonster = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _target = GameObject.FindWithTag("Player");

        if (_boxmonster == null || _animator == null || _target == null)
        {
            Debug.LogError("�ʿ��� ������Ʈ�� �������� �ʾ�����");
        }

        _lockOn = false;
    }

    // Update is called once per frame
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
            _boxmonster.isStopped = true;
            _animator.SetBool("isWalking", false);
            _animator.SetBool("isAttacking", false);
            return;
        }

        if (_lockOn)
        {
            _boxmonster.isStopped = false;
            _boxmonster.SetDestination(_target.transform.position);

            if (distanceToPlayer > attackRange)
            {
                _animator.SetBool("isWalking", true);
                _animator.SetBool("isAttacking", false);
            }
            else
            {
                _animator.SetBool("isWalking", false);
                _animator.SetBool("isAttacking", true);
            }
        }
    }
}
