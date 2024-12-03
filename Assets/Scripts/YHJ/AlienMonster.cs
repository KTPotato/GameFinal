using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class AlienMonster : MonoBehaviour
{
    public GameObject player;            // �÷��̾��� Transform
    public float attackRange = 10f;     // ���� ����
    public float fieldOfView = 120f;    // �þ߰�
    public GameObject projectilePrefab; // �߻�ü ������
    public Transform firePoint;         // �߻�ü�� ������ ��ġ
    public float attackCooldown = 1f;   // ���� ��Ÿ��
    private bool _lockOn;

    private NavMeshAgent agent;         // NavMeshAgent ������Ʈ
    private float attackTimer = 0f;     // ���� Ÿ�̸�
    private Animator _animator;
    private NavMeshAgent _monster;
    private GameObject _target;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // NavMeshAgent �ʱ�ȭ
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return; // �÷��̾ �������� �ʾҴٸ� ����

        // �÷��̾ ����
        agent.SetDestination(player.transform.position);

        // �÷��̾���� �Ÿ� ���
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // �þ� ���� �ȿ� �ִ��� Ȯ��
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
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
            Vector3 direction = (player.transform.position - firePoint.position).normalized;
            projectile.transform.forward = direction;

            // Rigidbody�� ���� �߻�ü�� �ӵ� �ο�
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * 50f; // �߻�ü �ӵ� ����
            }

            attackTimer = 0f; // Ÿ�̸� �ʱ�ȭ
        }
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
