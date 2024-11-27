using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    private Rigidbody rb;
    public float Pdmg;
    public float bulletSpeed = 500;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.AddForce(transform.forward * bulletSpeed);
        
        //Destroy(gameObject, 3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
