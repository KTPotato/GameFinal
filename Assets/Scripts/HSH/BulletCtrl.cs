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
        if(gameObject.name == "PlayerBullet(Clone)")
        {
            rb.AddForce(transform.forward * bulletSpeed);
        }
    }
    private void Update()
    {
        if (transform.position.x > 50 || transform.position.x < -50 || transform.position.z > 50 || transform.position.z < -50)
        {
            Destroy(gameObject);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if(gameObject.name == "PlayerBullet(Clone)")
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {


                // AlienMonster ó��
                AlienMonster alienMonster = collision.gameObject.GetComponent<AlienMonster>();
                if (alienMonster != null)
                {
                    //Debug.Log("AlienMonster hit!");
                    alienMonster.TakeDamage(Pdmg);
                    Destroy(gameObject);
                    return; // ó�� �Ϸ� �� ����
                }

                // Monster ó��
                Monster normalMonster = collision.gameObject.GetComponent<Monster>();
                if (normalMonster != null)
                {
                    //Debug.Log("Monster hit!");
                    normalMonster.TakeDamage(Pdmg);
                    Destroy(gameObject);
                    return; // ó�� �Ϸ� �� ����
                }

                // Enemy �±״� ������ ��ũ��Ʈ�� ���� ��� �α� ���
                //Debug.LogWarning("Enemy tagged object has no damageable script!");
            }
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(gameObject.name == "PlayerBullet(Clone)")
        {
            if (other.tag != "Player" && other.tag != "PlayerBullet" && other.tag != "EnemyBullet" && other.tag != "EXP" && other.tag != "Heart" && other.tag != "spinball")
            {
                Destroy(gameObject);
            }
        }

    }

}
