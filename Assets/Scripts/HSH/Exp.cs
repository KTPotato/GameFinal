using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Exp : MonoBehaviour
{
    public GameObject player;
    public float distance;
    public float speed = 1;
    public int exp = 1;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");

    }

    // Update is called once per frame
    void Update()
    {

        distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance < 10)
        {
            transform.Translate((player.transform.position - transform.position).normalized * speed * Time.deltaTime);
        }else if (GameObject.FindWithTag("Enemy") == null)
        {
            transform.Translate((player.transform.position - transform.position).normalized * speed * Time.deltaTime);

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
}
