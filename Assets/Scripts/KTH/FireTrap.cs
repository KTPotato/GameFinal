using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrap : MonoBehaviour
{
    [SerializeField] private int damageAmount = 10; // 줄 데미지 양
    [SerializeField] private float damageInterval = 1.0f; // 데미지 간격 (초 단위)
    private float nextDamageTime = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DealDamage(other);
            nextDamageTime = Time.time + damageInterval; // 다음 데미지 시간 설정
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Time.time >= nextDamageTime)
        {
            DealDamage(other);
            nextDamageTime = Time.time + damageInterval; // 다음 데미지 시간 설정
        }
    }

    private void DealDamage(Collider player)
    {
        //// Player에게 데미지 주는 로직 (예: Health 컴포넌트를 참조하여 데미지 처리)
        //PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        //if (playerHealth != null)
        //{
        //    playerHealth.TakeDamage(damageAmount);
        //}
    }
}
