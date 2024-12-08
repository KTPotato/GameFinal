using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    private NavMeshAgent _monster;
    private GameObject _target;
    private Animator _animator;

    public float health = 50f; // 몬스터 체력
    public float attackRange = 1.5f; // 공격 범위
    public float detectionRange = 10f; // 플레이어를 감지하는 범위
    public float attackDamage = 10f; // 플레이어에게 줄 데미지
    public float attackCooldown = 1.5f; // 공격 쿨다운 시간
    public GameObject exp; // 경험치 아이템 프리팹

    private bool _lockOn;
    private bool _canAttack = true; // 공격 가능 여부

    void Start()
    {
        _monster = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _target = GameObject.FindWithTag("Player");

        if (_monster == null || _animator == null || _target == null)
        {
            Debug.LogError("필요한 컴포넌트가 설정되지 않았습니다!");
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
        _canAttack = false; // 공격 가능 상태 비활성화

        Player1Ctrl playerCtrl = _target.GetComponent<Player1Ctrl>();
        if (playerCtrl != null)
        {
            playerCtrl.Hp -= attackDamage; // 플레이어의 Hp 감소
            Debug.Log($"Monster attacked! Player HP: {playerCtrl.Hp}");
        }

        // 공격 쿨다운 시작
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        _canAttack = true; // 쿨다운 후 공격 가능 상태로 전환
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerBullet")
        {
            TakeDamage(other.GetComponent<BulletCtrl>().Pdmg);
            Destroy(other.gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage; // 체력 감소
        Debug.Log($"Monster took {damage} damage. Remaining health: {health}");

        if (health <= 0)
        {
            Die(); // 체력이 0 이하일 경우 사망 처리
        }
    }

    private void Die()
    {
        Debug.Log("Monster died!");
        int rand = Random.Range(8, 15); // 랜덤한 경험치 아이템 생성

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

        Destroy(gameObject); // 몬스터 오브젝트 파괴
    }
}
