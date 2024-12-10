using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    private NavMeshAgent _monster;
    private GameObject _target;
    private Animator _animator;
    public GameObject heart;

    public float health = 50f; // ���� ü��
    public float attackRange = 1.5f; // ���� ����
    public float detectionRange = 20f; // �÷��̾ �����ϴ� ����
    public float attackDamage = 10f; // �÷��̾�� �� ������
    public float attackCooldown = 1.5f; // ���� ��ٿ� �ð�
    public GameObject exp; // ����ġ ������ ������

    private bool _lockOn;
    private bool _canAttack = true; // ���� ���� ����

    void Start()
    {
        _monster = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _target = GameObject.FindWithTag("Player");
        //heart = Resources.Load<GameObject>("Heart"); // Resources �������� Heart ������ �ε�

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

                if (_canAttack)
                {
                    AttackPlayer();
                }
            }
        }
        else
        {
            _monster.isStopped = true;
            _animator.SetBool("isWalking", false);
            _animator.SetBool("isAttacking", false);
        }
    }

    private void AttackPlayer()
    {
        _canAttack = false; // ���� ���� ���� ��Ȱ��ȭ

        Player1Ctrl playerCtrl = _target.GetComponent<Player1Ctrl>();
        if (playerCtrl != null)
        {
            playerCtrl.Hp -= attackDamage; // �÷��̾��� Hp ����
            Debug.Log($"Monster attacked! Player HP: {playerCtrl.Hp}");
        }

        // ���� ��ٿ� ����
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        _canAttack = true; // ��ٿ� �� ���� ���� ���·� ��ȯ
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




    public void TakeDamage(float damage)
    {
        health -= damage; // ü�� ����
        Debug.Log($"Monster took {damage} damage. Remaining health: {health}");

        if (health <= 0)
        {
            Die(); // ü���� 0 ������ ��� ��� ó��
        }
    }

    private void Die()
    {
        Debug.Log("Monster died!");
        int rand = Random.Range(8, 15); // ������ ����ġ ������ ����

        // ���� ��ġ�� ����ġ ������ ����
        for (int i = 0; i < rand; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(0f, 1f),
                Random.Range(-1f, 1f)
            );

            Vector3 spawnPosition = transform.position + randomOffset;
            Instantiate(exp, spawnPosition, Quaternion.identity);
        }

        // 5% Ȯ���� Heart ������ ����
        float dropChance = Random.Range(0f, 100f);
        if (dropChance <= 5f)
        {
            Vector3 heartSpawnPosition = transform.position + Vector3.up; // ���� ��ġ ���� ����
            Instantiate(heart, heartSpawnPosition, Quaternion.identity);
            Debug.Log("Heart dropped!");
        }

        Destroy(gameObject); // ���� ������Ʈ �ı�
    }

}
