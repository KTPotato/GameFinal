using System.Collections;
using UnityEngine;

public class BoxMonster : MonoBehaviour
{
    private GameObject _target;
    private Animator _animator;
    private bool _lockOn;
    private bool _canAttack = true; // ���� ���� ����

    public float health = 50f; // ���� ü��
    public float attackRange = 1.5f; // ���� ����
    public float detectionRange = 10f; // �÷��̾ �����ϴ� ����
    public GameObject exp;
    public GameObject heart;
    public float attackDamage = 10f; // �÷��̾�� �� ������
    public float attackCooldown = 1.5f; // ���� ��ٿ� �ð�

    private bool _isPlayerNearby; // �÷��̾� ���� ����
    
    void Start()
    {
        _animator = GetComponent<Animator>();
        _target = GameObject.FindWithTag("Player");

        if (_animator == null || _target == null)
        {
            Debug.LogError("�ʿ��� ������Ʈ�� �������� �ʾҽ��ϴ�!");
        }

        _isPlayerNearby = false;
    }

    void Update()
    {
        if (_target == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _target.transform.position);

        // �÷��̾ ���� ������ ���Դ��� Ȯ��
        if (distanceToPlayer <= detectionRange)
        {
            _isPlayerNearby = true;
        }
        else
        {
            _isPlayerNearby = false;
        }

        // �÷��̾� ��ó�� ���� ���� �ൿ
        if (_isPlayerNearby)
        {
            if (distanceToPlayer > attackRange)
            {
                _animator.SetBool("isWalking", true);
                _animator.SetBool("isAttacking", false);
            }
            else
            {
                _animator.SetBool("isWalking", false);
                _animator.SetBool("isAttacking", true);

                // ���� ���� �߰�
                if (_canAttack)
                {
                    AttackPlayer();
                }
            }
        }
        else
        {
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
            Debug.Log($"BoxMonster attacked! Player HP: {playerCtrl.Hp}");
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

    // ���Ͱ� �������� ���� �� ȣ��Ǵ� �޼���
    public void TakeDamage(float damage)
    {
        health -= damage; // ü�� ����
        Debug.Log($"BoxMonster took {damage} damage. Remaining health: {health}");

        if (health <= 0)
        {
            Die(); // ü���� 0 ������ ��� ���� ó��
        }
    }

    // ���� ��� ó��
    private void Die()
    {
        Debug.Log("BoxMonster died!");
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
