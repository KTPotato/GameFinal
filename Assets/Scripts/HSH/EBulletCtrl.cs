using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EBulletCtrl : MonoBehaviour
{
    private Rigidbody rb;
    public float Edmg = 1;
    public float bulletSpeed = 300;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.AddForce(transform.forward * bulletSpeed);

        Destroy(gameObject, 3f);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
