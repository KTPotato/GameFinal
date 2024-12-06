using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EBulletCtrl : MonoBehaviour
{
    private Rigidbody rb;
    public float Edmg = 1;
    public float bulletSpeed = 300;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.AddForce(transform.forward * bulletSpeed);
        //Debug.Log("Bullet created. Will be destroyed in 3 seconds.");

        Destroy(gameObject, 3f);
        //Debug.Log("Bullet destroyed!");
    }

    void Update()
    {
        // Debugging bullet position
       
    }
}
