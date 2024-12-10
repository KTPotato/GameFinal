using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerCameraCtrl : MonoBehaviour
{
    [SerializeField] private float distance = -15;
    [SerializeField] private float height = 10;
    [SerializeField] private float dampingTrace = 20;



    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (player != null) 
        {
            //ƒı≈Õ∫‰
            transform.position = new Vector3(player.position.x, player.position.y + height, player.position.z + distance);
            transform.LookAt(player.position + Vector3.up * 1.0f);
        }
        else
        {
            transform.position = transform.position;
        }
        

        
    }

}
