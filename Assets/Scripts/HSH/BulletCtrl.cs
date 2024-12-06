using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    private Rigidbody rb;
    public float Pdmg; // 데미지 값
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
            

            // AlienMonster 처리
            AlienMonster alienMonster = collision.gameObject.GetComponent<AlienMonster>();
            if (alienMonster != null)
            {
                Debug.Log("AlienMonster hit!");
                alienMonster.TakeDamage(Pdmg);
                Destroy(gameObject);
                return; // 처리 완료 후 종료
            }

            // Monster 처리
            Monster normalMonster = collision.gameObject.GetComponent<Monster>();
            if (normalMonster != null)
            {
                Debug.Log("Monster hit!");
                normalMonster.TakeDamage(Pdmg);
                Destroy(gameObject);
                return; // 처리 완료 후 종료
            }

            // Enemy 태그는 있지만 스크립트가 없을 경우 로그 출력
            //Debug.LogWarning("Enemy tagged object has no damageable script!");
        }
    }


}
