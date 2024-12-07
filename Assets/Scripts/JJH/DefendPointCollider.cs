using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendPointCollider : MonoBehaviour
{
    private void Start()
    {
        
    }
    private void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            // ¹æÆÐ¿¡ ºÎµúÈù ÃÑ¾Ë »èÁ¦
            Destroy(other.gameObject);
        }
    }
}
