using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EBulletCtrl : MonoBehaviour
{
    private Rigidbody rb;
    public float Edmg = 1;
    public float bulletSpeed = 600;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * bulletSpeed);

        Destroy(gameObject, 3f); // 3초 뒤 파괴
    }
    private void Update()
    {
        if (transform.position.x > 50 || transform.position.x < -50 || transform.position.z > 50 || transform.position.z < -50)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // EnemyBullet이 Player와 충돌했을 때
        if (other.CompareTag("Player"))
        {
            // Player1Ctrl 스크립트를 가져와서 직접 데미지 처리
            Player1Ctrl playerCtrl = other.GetComponent<Player1Ctrl>();
            if (playerCtrl != null)
            {
                playerCtrl.Hp -= Edmg; // 플레이어 HP 감소
                //Debug.Log($"Player hit! Remaining HP: {playerCtrl.Hp}");
            }

            // 발사체 파괴
            //Debug.Log("Bullet Destroyed!");
            Destroy(gameObject);
        }
        else if(other.tag != "EnemyBullet" && other.tag != "Bullet"&& other.tag != "Enemy"&& other.tag != "PlayerBullet"&& other.tag != "EXP"&& other.tag != "Hp"&& other.tag != "Heart"&& other.tag != "spinball")
        {
            Destroy(gameObject);
        }
    }

    

}
