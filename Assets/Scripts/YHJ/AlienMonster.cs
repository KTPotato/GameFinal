using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AlienMonster : MonoBehaviour
{
    public Transform player;            // �÷��̾��� Transform
    public float attackRange = 10f;     // ���� ����
    public float fieldOfView = 120f;    // �þ߰�
    public GameObject projectilePrefab; // �߻�ü ������
    public Transform firePoint;         // �߻�ü�� ������ ��ġ
    public float attackCooldown = 2f;   // ���� ��Ÿ��

    private NavMeshAgent agent;         // NavMeshAgent ������Ʈ
    private float attackTimer = 0f;     // ���� Ÿ�̸�

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // NavMeshAgent �ʱ�ȭ
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return; // �÷��̾ �������� �ʾҴٸ� ����

        // �÷��̾ ����
        agent.SetDestination(player.position);

        // �÷��̾���� �Ÿ� ���
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // �þ� ���� �ȿ� �ִ��� Ȯ��
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
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
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

            // �߻�ü�� ���� ����
            Vector3 direction = (player.position - firePoint.position).normalized;
            projectile.transform.forward = direction;

            // Rigidbody�� ���� �߻�ü�� �ӵ� �ο�
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * 10f; // �߻�ü �ӵ� ����
            }

            attackTimer = 0f; // Ÿ�̸� �ʱ�ȭ
        }
    }
}
