using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxMonster : MonoBehaviour
{
    private GameObject _target;
    private Animator _animator;
    private bool _lockOn;
    private bool _canAttack = true; // 공격 가능 여부

    public float health = 50f; // 몬스터 체력
    public float attackRange = 1.5f; // 공격 범위
    public float detectionRange = 10f; // 플레이어를 감지하는 범위
    public GameObject exp;
    public float attackDamage = 10f; // 플레이어에게 줄 데미지
    public float attackCooldown = 1.5f; // 공격 쿨다운 시간

    private bool _isPlayerNearby; // 플레이어 감지 여부

    void Start()
    {
        _animator = GetComponent<Animator>();
        _target = GameObject.FindWithTag("Player");

        if (_animator == null || _target == null)
        {
            Debug.LogError("필요한 컴포넌트가 설정되지 않았습니다!");
        }

        _isPlayerNearby = false;
    }

    void Update()
    {
        if (_target == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _target.transform.position);

        // 플레이어가 감지 범위에 들어왔는지 확인
        if (distanceToPlayer <= detectionRange)
        {
            _isPlayerNearby = true;
        }
        else
        {
            _isPlayerNearby = false;
        }

        // 플레이어 근처에 있을 때만 행동
        if (_isPlayerNearby)
        {
            if (distanceToPlayer > attackRange)
            {
                _animator.SetBool("isWalking", false);
                _animator.SetBool("isAttacking", false);
            }
            else
            {
                _animator.SetBool("isWalking", false);
                _animator.SetBool("isAttacking", true);
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
        _canAttack = false; // 공격 가능 상태 비활성화

        Player1Ctrl playerCtrl = _target.GetComponent<Player1Ctrl>();
        if (playerCtrl != null)
        {
            playerCtrl.Hp -= attackDamage; // 플레이어의 Hp 감소
            Debug.Log($"BoxMonster attacked! Player HP: {playerCtrl.Hp}");
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

    // 몬스터가 데미지를 받을 때 호출되는 메서드
    public void TakeDamage(float damage)
    {
        health -= damage; // 체력 감소
        Debug.Log($"BoxMonster took {damage} damage. Remaining health: {health}");

        if (health <= 0)
        {
            Die(); // 체력이 0 이하일 경우 죽음 처리
        }
    }

    // 몬스터 사망 처리
    private void Die()
    {
        Debug.Log("BoxMonster died!");
        int rand = Random.Range(5, 8); // 랜덤으로 생성할 경험치 구슬 개수 설정

        for (int i = 0; i < rand; i++)
        {
            // 경험치 구슬을 몬스터 주변의 랜덤한 위치에 생성
            Vector3 randomOffset = new Vector3(
                Random.Range(-1f, 1f),  // X축 랜덤 위치
                Random.Range(0f, 1f),  // Y축 약간 위로 띄우기
                Random.Range(-1f, 1f)  // Z축 랜덤 위치
            );

            Vector3 spawnPosition = transform.position + randomOffset;

            Instantiate(exp, spawnPosition, Quaternion.identity);
        }

        Destroy(gameObject); // 몬스터 오브젝트 파괴
    }
}
