using System.Collections;
using UnityEngine;

public class BoxMonster : MonoBehaviour
{
    private GameObject _target;
    private Animator _animator;
    private bool _lockOn = false;
    private bool _canAttack = true; // ���� ���� ����

    public float health = 50f; // ���� ü��
    public float detectionRange = 15f; // �÷��̾ �����ϴ� ����
    public GameObject exp;
    public GameObject heart;
    public GameObject bulletPrefab; // �߻��� �Ѿ� ������
    public Transform firePoint; // �Ѿ� �߻� ��ġ
    public float bulletSpeed = 10f; // �Ѿ� �ӵ�
    public float attackCooldown = 2f; // ���� ��ٿ� �ð�

    private bool _isPlayerNearby = false; // �÷��̾� ���� ����



    void Start()
    {
        _animator = GetComponent<Animator>();
        _target = GameObject.FindWithTag("Player");

        if (_animator == null || _target == null || bulletPrefab == null || firePoint == null)
        {
            //Debug.LogError("�ʿ��� ������Ʈ�� �������� �ʾҽ��ϴ�!");
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
        

        // �÷��̾ ���� �ȿ� ���� �� ����
        if (_isPlayerNearby && _canAttack)
        {
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        _canAttack = false; // ���� ���� ���� ��Ȱ��ȭ

        // ���� �ִϸ��̼� ���
        _animator.Play("Attack02");

        // �Ѿ� �߻� ����
        FireBullets();

        // ���� ��ٿ� ����
        StartCoroutine(AttackCooldown());
    }

    private void FireBullets()
    {
        // �÷��̾� ���� ���
        Vector3 directionToPlayer = (_target.transform.position - transform.position).normalized;

        // ��ä�� ���� ����
        float[] angles = { -15f, 0f, 15f }; // �Ѿ��� �߻�� ������

        foreach (float angle in angles)
        {
            // ������ ȸ�������� ��ȯ
            Quaternion rotation = Quaternion.Euler(0, angle, 0);

            // �߻� ���� ���
            Vector3 fireDirection = rotation * directionToPlayer;

            // �Ѿ� ���� �� �ʱ�ȭ
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            if (bullet == null)
            {
                //Debug.LogError("�Ѿ� ���� ����!");
                continue;
            }

            //Debug.Log($"�Ѿ� ����: {bullet.name} at {firePoint.position}");

            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.velocity = fireDirection * bulletSpeed;
                //Debug.Log($"�Ѿ� �ӵ� ���� �Ϸ�: {bulletRb.velocity}");
            }
            else
            {
                //Debug.LogError("�Ѿ˿� Rigidbody�� �����ϴ�!");
            }

            // �Ѿ� ȸ�� ���� ����
            bullet.transform.forward = fireDirection;
            bullet.GetComponent<EBulletCtrl>().bulletSpeed = bulletSpeed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerBullet")
        {
            TakeDamage(other.GetComponent<BulletCtrl>().Pdmg);
            Destroy(other.gameObject);
            //Debug.Log($"���Ϳ� �÷��̾��� �Ѿ� �浹: {other.name}");
        }
        else if (other.tag == "MonsterBullet")
        {
            //Debug.LogWarning($"������ �Ѿ˰� ���Ͱ� �浹: {other.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "spinball")
        {
            TakeDamage(other.GetComponent<BulletCtrl>().Pdmg);
            //Debug.Log($"���Ͱ� spinball�� �浹: {other.name}");
        }
    }


    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        _canAttack = true; // ��ٿ� �� ���� ���� ���·� ��ȯ
    }

    

    // ���Ͱ� �������� ���� �� ȣ��Ǵ� �޼���
    public void TakeDamage(float damage)
    {
        health -= damage; // ü�� ����
        //Debug.Log($"BoxMonster took {damage} damage. Remaining health: {health}");
        _isPlayerNearby = true;

        if (health <= 0)
        {
            Die(); // ü���� 0 ������ ��� ���� ó��
        }
    }

    // ���� ��� ó��
    private void Die()
    {
        //Debug.Log("BoxMonster died!");
        int rand = Random.Range(5, 8); // �������� ������ ����ġ ���� ���� ����

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
            //Debug.Log("Heart dropped!");
        }

        Destroy(gameObject); // ���� ������Ʈ �ı�
    }
}
