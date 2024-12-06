using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    private Rigidbody rb;
    public float Pdmg; // ������ ��
    public float bulletSpeed = 500;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * bulletSpeed);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            

            // AlienMonster ó��
            AlienMonster alienMonster = collision.gameObject.GetComponent<AlienMonster>();
            if (alienMonster != null)
            {
                Debug.Log("AlienMonster hit!");
                alienMonster.TakeDamage(Pdmg);
                Destroy(gameObject);
                return; // ó�� �Ϸ� �� ����
            }

            // Monster ó��
            Monster normalMonster = collision.gameObject.GetComponent<Monster>();
            if (normalMonster != null)
            {
                Debug.Log("Monster hit!");
                normalMonster.TakeDamage(Pdmg);
                Destroy(gameObject);
                return; // ó�� �Ϸ� �� ����
            }

            // Enemy �±״� ������ ��ũ��Ʈ�� ���� ��� �α� ���
            //Debug.LogWarning("Enemy tagged object has no damageable script!");
        }
    }


}
