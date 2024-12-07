using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public int maxHealth;
    public int curHealth;


    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat=GetComponent<MeshRenderer>().material;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerBullet")
        {
            BulletCtrl bulletCtrl = other.GetComponent<BulletCtrl>();
            curHealth -= (int)bulletCtrl.Pdmg;

            Debug.Log("Range : " + curHealth);
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    
}
