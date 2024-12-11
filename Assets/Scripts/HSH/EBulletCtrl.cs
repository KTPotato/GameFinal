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

        Destroy(gameObject, 3f); // 3�� �� �ı�
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
        // EnemyBullet�� Player�� �浹���� ��
        if (other.CompareTag("Player"))
        {
            // Player1Ctrl ��ũ��Ʈ�� �����ͼ� ���� ������ ó��
            Player1Ctrl playerCtrl = other.GetComponent<Player1Ctrl>();
            if (playerCtrl != null)
            {
                playerCtrl.Hp -= Edmg; // �÷��̾� HP ����
                //Debug.Log($"Player hit! Remaining HP: {playerCtrl.Hp}");
            }

            // �߻�ü �ı�
            //Debug.Log("Bullet Destroyed!");
            Destroy(gameObject);
        }
        else if(other.tag != "EnemyBullet" && other.tag != "Bullet"&& other.tag != "Enemy"&& other.tag != "PlayerBullet"&& other.tag != "EXP"&& other.tag != "Hp"&& other.tag != "Heart"&& other.tag != "spinball")
        {
            Destroy(gameObject);
        }
    }

    

}
