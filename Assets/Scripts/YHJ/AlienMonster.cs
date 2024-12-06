using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AlienMonster : MonoBehaviour
{
    public GameObject _target;            // �÷��̾��� Transform
    public float attackRange = 10f;     // ���� ����
    public float fieldOfView = 120f;    // �þ߰�
    public GameObject projectilePrefab; // �߻�ü ������
    public Transform firePoint;         // �߻�ü�� ������ ��ġ
    public float attackCooldown = 1f;   // ���� ��Ÿ��
    public float health;        // ���� ü��
    

    private NavMeshAgent agent;         // NavMeshAgent ������Ʈ
    private float attackTimer = 0f;     // ���� Ÿ�̸�
    private Animator _animator;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // NavMeshAgent �ʱ�ȭ
        _target = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (_target == null) return; // �÷��̾ �������� �ʾҴٸ� ����

        // �÷��̾ ����
        agent.SetDestination(_target.transform.position);

        // �÷��̾���� �Ÿ� ���
        float distanceToPlayer = Vector3.Distance(transform.position, _target.transform.position);

        // �þ� ���� �ȿ� �ִ��� Ȯ��
        Vector3 directionToPlayer = (_target.transform.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (distanceToPlayer <= attackRange && angleToPlayer <= fieldOfView / 2)
        {
            // ���� ���� ����
            AttackPlayer();
        }
    }

    void AttackPlayer()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            // �߻�ü ����
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            // �߻�ü�� ���� ����
            Vector3 direction = (_target.transform.position - firePoint.position).normalized;
            projectile.transform.forward = direction;

            // Rigidbody�� ���� �߻�ü�� �ӵ� �ο�
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * 50f; // �߻�ü �ӵ� ����
            }

            // �浹 ��Ȱ��ȭ
            //Collider projectileCollider = projectile.GetComponent<Collider>();
            //Collider alienCollider = GetComponent<Collider>();
            //if (projectileCollider != null && alienCollider != null)
            //{
            //    Physics.IgnoreCollision(projectileCollider, alienCollider);
            //}

            attackTimer = 0f; // Ÿ�̸� �ʱ�ȭ
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
        Debug.Log($"TakeDamage called with damage: {damage}");
        health -= damage; // ü�� ����
        Debug.Log($"Remaining health: {health}");

        if (health <= 0)
        {
            Die(); // ü���� 0 ������ ��� ���� ó��
        }
    }

    // ���� ��� ó��
    private void Die()
    {
        Debug.Log("AlienMonster died!");
        Destroy(gameObject); // ���� ������Ʈ �ı�
    }
}
