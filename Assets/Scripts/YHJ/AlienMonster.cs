using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AlienMonster : MonoBehaviour
{
    public GameObject _target;            // �÷��̾��� Transform
    public float attackRange = 10f;     // ���� ����
    public float fieldOfView = 120f;    // �þ߰�
    public GameObject projectilePrefab; // �߻�ü ������
    public Transform FirePoint;         // �߻�ü�� ������ ��ġ
    public float attackCooldown = 1f;   // ���� ��Ÿ��
    public float health;        // ���� ü��
    public GameObject exp;
    public GameObject heart;
    

    private NavMeshAgent agent;         // NavMeshAgent ������Ʈ
    private float attackTimer = 0f;     // ���� Ÿ�̸�
    private Animator _animator;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // NavMeshAgent �ʱ�ȭ
        _target = GameObject.FindWithTag("Player");
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (_target == null) return; // �÷��̾ �������� �ʾҴٸ� ����

        // �÷��̾���� �Ÿ� ���
        float distanceToPlayer = Vector3.Distance(transform.position, _target.transform.position);

        // NavMeshAgent�� ���ߴ� �Ÿ��� ����
        agent.stoppingDistance = attackRange / 2; // ���� �Ÿ� ���� (��: ���� ������ ����)

        // �÷��̾ ���� (��ǥ ���� ������Ʈ)
        if (distanceToPlayer > agent.stoppingDistance) // �÷��̾���� �Ÿ��� ���� �Ÿ����� �ָ� ����
        {
            agent.SetDestination(_target.transform.position);
        }
        else
        {
            agent.ResetPath(); // �ʹ� ��������� ���ߵ��� ��� �ʱ�ȭ
        }

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
            if (projectilePrefab == null)
            {
                //Debug.LogError("Projectile Prefab is not assigned!");
                return;
            }

            if (FirePoint == null)
            {
                //Debug.LogError("FirePoint is not assigned!");
                return;
            }

            // �߻�ü ����
            GameObject projectile = Instantiate(projectilePrefab, FirePoint.position, FirePoint.rotation);
            //Debug.Log($"Projectile created at: {FirePoint.position}");
            _animator.Play("Attack01");

            // EBulletCtrl ��ũ��Ʈ�� ã�� �ʱ�ȭ
            EBulletCtrl bulletCtrl = projectile.GetComponent<EBulletCtrl>();
            if (bulletCtrl != null)
            {
                bulletCtrl.bulletSpeed = 300f; // �ʿ信 ���� �ӵ� ����
                bulletCtrl.Edmg = 10f;        // �ʿ信 ���� ������ ����
            }
            else
            {
                //Debug.LogError("EBulletCtrl script is missing on the projectile prefab!");
            }

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

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "spinball")
        {
            TakeDamage(other.GetComponent<BulletCtrl>().Pdmg);
        }
    }



    // ���Ͱ� �������� ���� �� ȣ��Ǵ� �޼���
    public void TakeDamage(float damage)
    {
        //Debug.Log($"TakeDamage called with damage: {damage}");
        health -= damage; // ü�� ����
        //Debug.Log($"Remaining health: {health}");

        if (health <= 0)
        {
            Die(); // ü���� 0 ������ ��� ���� ó��
        }
    }

    
    // ���� ��� ó��
    private void Die()
    {
        //Debug.Log("AlienMonster died!");
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
            //Debug.Log("Heart dropped!");
        }

        Destroy(gameObject); // ���� ������Ʈ �ı�
    }

}
