using System.Collections;
using UnityEngine;

public class BoxMonster : MonoBehaviour
{
    private GameObject _target;
    private Animator _animator;
    private bool _lockOn = false;
    private bool _canAttack = true; // 공격 가능 여부

    public float health = 50f; // 몬스터 체력
    public float detectionRange = 15f; // 플레이어를 감지하는 범위
    public GameObject exp;
    public GameObject heart;
    public GameObject bulletPrefab; // 발사할 총알 프리팹
    public Transform firePoint; // 총알 발사 위치
    public float bulletSpeed = 10f; // 총알 속도
    public float attackCooldown = 2f; // 공격 쿨다운 시간

    private bool _isPlayerNearby = false; // 플레이어 감지 여부



    void Start()
    {
        _animator = GetComponent<Animator>();
        _target = GameObject.FindWithTag("Player");

        if (_animator == null || _target == null || bulletPrefab == null || firePoint == null)
        {
            //Debug.LogError("필요한 컴포넌트가 설정되지 않았습니다!");
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
        

        // 플레이어가 범위 안에 있을 때 공격
        if (_isPlayerNearby && _canAttack)
        {
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        _canAttack = false; // 공격 가능 상태 비활성화

        // 공격 애니메이션 재생
        _animator.Play("Attack02");

        // 총알 발사 로직
        FireBullets();

        // 공격 쿨다운 시작
        StartCoroutine(AttackCooldown());
    }

    private void FireBullets()
    {
        // 플레이어 방향 계산
        Vector3 directionToPlayer = (_target.transform.position - transform.position).normalized;

        // 부채꼴 각도 설정
        float[] angles = { -15f, 0f, 15f }; // 총알이 발사될 각도들

        foreach (float angle in angles)
        {
            // 각도를 회전값으로 변환
            Quaternion rotation = Quaternion.Euler(0, angle, 0);

            // 발사 방향 계산
            Vector3 fireDirection = rotation * directionToPlayer;

            // 총알 생성 및 초기화
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            if (bullet == null)
            {
                //Debug.LogError("총알 생성 실패!");
                continue;
            }

            //Debug.Log($"총알 생성: {bullet.name} at {firePoint.position}");

            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.velocity = fireDirection * bulletSpeed;
                //Debug.Log($"총알 속도 설정 완료: {bulletRb.velocity}");
            }
            else
            {
                //Debug.LogError("총알에 Rigidbody가 없습니다!");
            }

            // 총알 회전 방향 설정
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
            //Debug.Log($"몬스터와 플레이어의 총알 충돌: {other.name}");
        }
        else if (other.tag == "MonsterBullet")
        {
            //Debug.LogWarning($"몬스터의 총알과 몬스터가 충돌: {other.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "spinball")
        {
            TakeDamage(other.GetComponent<BulletCtrl>().Pdmg);
            //Debug.Log($"몬스터가 spinball과 충돌: {other.name}");
        }
    }


    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        _canAttack = true; // 쿨다운 후 공격 가능 상태로 전환
    }

    

    // 몬스터가 데미지를 받을 때 호출되는 메서드
    public void TakeDamage(float damage)
    {
        health -= damage; // 체력 감소
        //Debug.Log($"BoxMonster took {damage} damage. Remaining health: {health}");
        _isPlayerNearby = true;

        if (health <= 0)
        {
            Die(); // 체력이 0 이하일 경우 죽음 처리
        }
    }

    // 몬스터 사망 처리
    private void Die()
    {
        //Debug.Log("BoxMonster died!");
        int rand = Random.Range(5, 8); // 랜덤으로 생성할 경험치 구슬 개수 설정

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

        // 5% 확률로 Heart 프리팹 생성
        float dropChance = Random.Range(0f, 100f);
        if (dropChance <= 5f)
        {
            Vector3 heartSpawnPosition = transform.position + Vector3.up; // 몬스터 위치 위에 생성
            Instantiate(heart, heartSpawnPosition, Quaternion.identity);
            //Debug.Log("Heart dropped!");
        }

        Destroy(gameObject); // 몬스터 오브젝트 파괴
    }
}
