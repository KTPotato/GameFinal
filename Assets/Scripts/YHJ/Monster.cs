using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    private NavMeshAgent _monster;
    private GameObject _target;
    private Animator _animator;

    public float health = 50f; // ���� ü��
    public float attackRange = 1.5f; // ���� ����
    public float detectionRange = 10f; // �÷��̾ �����ϴ� ����
    public GameObject exp;

    private bool _lockOn;

    void Start()
    {
        _monster = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _target = GameObject.FindWithTag("Player");

        if (_monster == null || _animator == null || _target == null)
        {
            Debug.LogError("�ʿ��� ������Ʈ�� �������� �ʾҽ��ϴ�!");
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

    // ���Ͱ� �������� ���� �� ȣ��Ǵ� �޼���
    public void TakeDamage(float damage)
    {
        health -= damage; // ü�� ����
        Debug.Log($"Monster took {damage} damage. Remaining health: {health}");

        if (health <= 0)
        {
            Die(); // ü���� 0 ������ ��� ���� ó��
        }
    }

    // ���� ��� ó��
    // ���� ��� ó��
    private void Die()
    {
        Debug.Log("Monster died!");
        int rand = Random.Range(5, 8); // �������� ������ ����ġ ���� ���� ����

        for (int i = 0; i < rand; i++)
        {
            // ����ġ ������ ���� �ֺ��� ������ ��ġ�� ����
            Vector3 randomOffset = new Vector3(
                Random.Range(-1f, 1f),  // X�� ���� ��ġ
                Random.Range(0f, 1f),  // Y�� �ణ ���� ����
                Random.Range(-1f, 1f)  // Z�� ���� ��ġ
            );

            Vector3 spawnPosition = transform.position + randomOffset;

            Instantiate(exp, spawnPosition, Quaternion.identity);
        }

        Destroy(gameObject); // ���� ������Ʈ �ı�
    }

}
