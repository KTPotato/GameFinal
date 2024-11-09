using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrap : MonoBehaviour
{
    [SerializeField] private int damageAmount = 10; // �� ������ ��
    [SerializeField] private float damageInterval = 1.0f; // ������ ���� (�� ����)
    private float nextDamageTime = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DealDamage(other);
            nextDamageTime = Time.time + damageInterval; // ���� ������ �ð� ����
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Time.time >= nextDamageTime)
        {
            DealDamage(other);
            nextDamageTime = Time.time + damageInterval; // ���� ������ �ð� ����
        }
    }

    private void DealDamage(Collider player)
    {
        //// Player���� ������ �ִ� ���� (��: Health ������Ʈ�� �����Ͽ� ������ ó��)
        //PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        //if (playerHealth != null)
        //{
        //    playerHealth.TakeDamage(damageAmount);
        //}
    }
}
