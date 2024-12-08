using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Heart : MonoBehaviour
{
    public GameObject player;
    private float distance;
    public float plusHp = 20;
    public float speed = 1;
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
        }
        else if (GameObject.FindWithTag("Enemy") == null)
        {
            transform.Translate((player.transform.position - transform.position).normalized * speed * Time.deltaTime);

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Player1Ctrl pl = other.GetComponent<Player1Ctrl>();
            if (pl.Hp == pl.maxHp)
            {
                Destroy(gameObject);
            }
            else if(pl.Hp + plusHp > pl.maxHp)
            {
                pl.Hp += pl.maxHp - pl.Hp;
            }
            else
            {
                pl.Hp += plusHp;
            }
            Destroy(gameObject);
        }
    }
}
