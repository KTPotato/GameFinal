using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1Ctrl : MonoBehaviour
{
    public float speed = 5;
    public float rotspeed = 100;
    private float v = 0;
    private float h = 0;
    private Vector3 movedir = Vector3.zero;
    private Animation _ani;


    private Ray mouseRay; //���콺 ray
    private Plane plane; //ray�� �浹�� Ȯ���� �ٴ�

    public GameObject bullet;
    public Transform firePos;
    public int iters = 299;
    public int firespeed = 300;

    public float PlayerHealth;

    // Start is called before the first frame update
    void Start()
    {
        _ani = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        //�̵�
        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");
        v = v * Mathf.Sqrt(1 - (h * h / 2));
        h = h * Mathf.Sqrt(1 - (v * v / 2));
        movedir = Vector3.forward * v + Vector3.right * h;

        //���ͺ�
        PlayerMove();

        //�ѽ��
        FireBullet();
    }
    void PlayerMove()//ž��
    {
        //�÷��̾� �̵�
        transform.position += movedir * speed * Time.deltaTime;
        //�÷��̾� ȸ��
        mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        plane = new Plane(Vector3.up, Vector3.up);
        if (plane.Raycast(mouseRay, out float distance))
        {
            Vector3 direction = mouseRay.GetPoint(distance) - transform.position;
            transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        }
    }

    void FireBullet()
    {
        if(iters % firespeed == 0)
        {
            if (Input.GetMouseButton(0))
            {
                Instantiate(bullet, firePos.position, transform.rotation);
                iters = 0;
            }
        }
        if(iters == firespeed)
        {
            return;
        }
        iters++;
    }
}
