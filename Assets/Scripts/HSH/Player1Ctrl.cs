using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player1Ctrl : MonoBehaviour
{
    public float speed = 5;
    public float rotspeed = 100;
    private float v = 0;
    private float h = 0;
    private Vector3 movedir = Vector3.zero;
    private Animator ani;


    private Ray mouseRay; //마우스 ray
    private Plane plane; //ray의 충돌을 확인할 바닥

    public GameObject bullet;
    public Transform firePos;
    public double iters = 299;
    public int firespeed = 300;

    public float PlayerHealth;

    public float maxHp = 100;
    public float Hp = 100;
    public float dmg = 1;

    public Slider HpBarSlider;

    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animator>();
        CheckHp();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
        FireBullet();
        Die();
        CheckHp();
    }
    void PlayerMove()
    {
        if(Hp <= 0) return;

        v = Input.GetAxisRaw("Vertical");
        h = Input.GetAxisRaw("Horizontal");
        //v = v * Mathf.Sqrt(1 - (h * h / 2));
        //h = h * Mathf.Sqrt(1 - (v * v / 2));

        if (v != 0 || h != 0)
        {
            ani.SetBool("move", true);
        }else if(v == 0 && h == 0)
        {
            ani.SetBool("move", false);
        }
        




        movedir = Vector3.forward * v + Vector3.right * h;

        //플레이어 이동
        transform.position += movedir * speed * Time.deltaTime;
        //플레이어 회전
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
                if (Hp <= 0) return;

                ani.Play("Attack01", -1, 0);
                Invoke("CreateBullet", 0.1f);
                
                
                iters = 0;
            }
        }
        if(iters == firespeed)
        {
            return;
        }
        iters++;
    }
    void CreateBullet()
    {
        GameObject newBullet = Instantiate(bullet, firePos.position, transform.rotation);
        newBullet.GetComponent<BulletCtrl>().Pdmg = dmg;
    }

    void Die()
    {
        if(transform.position.y < -10)
        {
            Hp = 0;
        }
        if (Hp <= 0)
        {
            Destroy(gameObject, 3);
            ani.Play("Die");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //GetHit
        if (other.tag == "EnemyBullet")
        {
            if(Hp <= 0) return;
            Hp -= other.GetComponent<EBulletCtrl>().Edmg;
            Destroy(other.gameObject);
            ani.Play("GetHit");


        }
    }
    public void CheckHp()
    {
        if (HpBarSlider != null)
            HpBarSlider.value = Hp / maxHp;
    }
}
