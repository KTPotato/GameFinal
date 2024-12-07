using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spinballPosCtrl : MonoBehaviour
{
    public GameObject player;
    public int ballLevel = 0;
    public GameObject ball;
    private int childCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        ballLevel = player.GetComponent<Player1Ctrl>().spinballLevel;
        transform.position = player.transform.position + new Vector3(0,0.7f,0);
        transform.Rotate(transform.up);
        if(transform.childCount != ballLevel * 2)
        {
            if (ballLevel >= 1)
            {
                //기존에 있던거 지움
                for (int i = 0; i < transform.childCount; i++)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }

                float angle = 360 / (ballLevel * 2);
                for (int i = 1; i <= ballLevel; i++)
                {
                    float angle_ = angle * i;
                    Quaternion rotation = transform.rotation * Quaternion.Euler(0, angle_, 0);
                    //GameObject Ball = Instantiate(ball, transform.position + transform.forward * 5, rotation);
                    GameObject Ball = Instantiate(ball);
                    Ball.transform.SetParent(transform, false);
                    Ball.transform.rotation = rotation;
                    Ball.transform.position = transform.position + Ball.transform.forward * 5;
                }
                for (int i = 1; i <= ballLevel; i++)
                {
                    float angle_ = 180 + angle * i;
                    Quaternion rotation = transform.rotation * Quaternion.Euler(0, angle_, 0);
                    //GameObject Ball = Instantiate(ball, transform.position + transform.forward * 5, rotation);
                    GameObject Ball = Instantiate(ball);
                    Ball.transform.SetParent(transform, false);
                    Ball.transform.rotation = rotation;
                    Ball.transform.position = transform.position + Ball.transform.forward * 5;
                }
            }
        }
    }
}
