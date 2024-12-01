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
    public float firespeed = 1f;

    public float PlayerHealth;

    public float maxHp = 100;
    public float Hp = 100;
    public float dmg = 1;

    public Slider HpBarSlider;
    public int playerLevel = 0;
    public int crossfireLevel = 1;
    public float fan_fireLevel = 0;

    private bool isAttack = false;

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

        movedir = Vector3.forward * v + Vector3.right * h;
 
        if (isAttack == true)
        {
            //플레이어 회전
            mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            plane = new Plane(Vector3.up, Vector3.up);
            if (plane.Raycast(mouseRay, out float distance))
            {
                Vector3 direction = mouseRay.GetPoint(distance) - transform.position;
                transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            }
            ani.SetBool("move", false);
        }
        else
        {
            transform.position += movedir * speed * Time.deltaTime;
            
            if (v != 0 || h != 0) 
            {
                transform.rotation = Quaternion.LookRotation(movedir);
                ani.SetBool("move", true);
            }
            else if (v == 0 && h == 0) 
            {
                mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                plane = new Plane(Vector3.up, Vector3.up);
                if (plane.Raycast(mouseRay, out float distance))
                {
                    Vector3 direction = mouseRay.GetPoint(distance) - transform.position;
                    transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                }
                ani.SetBool("move", false);
            }
        }
        
    }

    void FireBullet()
    {
        if(Input.GetMouseButton(0) && !isAttack && Hp > 0)
        {
            StartCoroutine(crossAttack());
        }

    }
    IEnumerator crossAttack()
    {
        isAttack = true;
        ani.Play("Attack01", -1, 0);
        yield return new WaitForSeconds(0.1f);
        //CreateBullet();
        for(int i =  0; i < crossfireLevel; i++)
        {
            if (crossfireLevel > 1) 
            {
                Vector3 vec = Vector3.zero;
                vec -= Vector3.right * 0.5f * crossfireLevel / 2;
                vec += Vector3.right * 0.5f * i;
                GameObject Bullet = Instantiate(bullet, firePos.position +vec, transform.rotation);
                /*Vector3 bulletPos = firePos.transform.position;
                bulletPos.x -= 1 * crossfireLevel / 2;
                bulletPos.x += 1 * i;
                Bullet.transform.rotation = transform.rotation;
                Bullet.transform.position += bulletPos;*/
                Bullet.GetComponent<BulletCtrl>().Pdmg = dmg;
            }
            else
            {
                GameObject Bullet = Instantiate(bullet, firePos.position, transform.rotation);
                Bullet.GetComponent<BulletCtrl>().Pdmg = dmg;
            }
            
        }

        yield return new WaitForSeconds(firespeed - 0.1f);
        isAttack = false;
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
