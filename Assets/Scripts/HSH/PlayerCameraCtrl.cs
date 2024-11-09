using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerCameraCtrl : MonoBehaviour
{
    private float distance = 8;
    private float height = 3;
    private float dampingTrace = 20;

    private float topDis = -3;
    private float topHeight = 15;

    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        /*
        //3ÀÎÄª
        transform.position = Vector3.Lerp(transform.position,
        player.position - player.forward * distance + Vector3.up * height,
                                          Time.deltaTime * dampingTrace);

        transform.LookAt(player.position + Vector3.up * 1.0f);
        */

        //Å¾ºä
        transform.position = new Vector3(player.position.x, player.position.y + topHeight, player.position.z + topDis);
        transform.LookAt(player.position + Vector3.up * 1.0f);

    }

}
