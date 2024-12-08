using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private Rigidbody rb;

    private Ray mouseRay; //마우스 ray
    private Plane plane; //ray의 충돌을 확인할 바닥

    public GameObject bullet;
    public Transform firePos;
    public float firespeed = 1f;

    public float PlayerHealth;

    public float maxHp = 100;
    public float Hp = 100;
    public float dmg = 1;

    public int playerLevel = 0;
    public float playerExp = 0;
    public float playerMaxExp = 100;
    public int crossfireLevel = 1;
    public int fan_fireLevel = 0;
    public int spinballLevel = 0;

    private bool isAttack = false;
    private bool canMove = true;
    public bool isLevelUp = false;

    public Image hpImage;
    public TMP_Text hpText;
    public Image expImage;
    public TMP_Text expText;



    // Start is called before the first frame update
    void Awake()
    {
        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
        FireBullet();
        Die();
        HpCheck();
        EXPCheck();
    }
    void PlayerMove()
    {
        if (Hp <= 0) return;

        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");
        v = v * Mathf.Sqrt(1 - (h * h / 2));
        h = h * Mathf.Sqrt(1 - (v * v / 2));

        movedir = Vector3.forward * v + Vector3.right * h;

        if (canMove == false)
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
        if (Input.GetMouseButton(0) && !isAttack && Hp > 0)
        {
            StartCoroutine(Attack());

        }

    }
    IEnumerator Attack()
    {
        isAttack = true;
        canMove = false;
        ani.Play("Attack01", -1, 0);
        yield return new WaitForSeconds(0.01f);

        //crossAttack
        for (int i = 0; i < crossfireLevel; i++)
        {
            if (crossfireLevel > 1)
            {

                // 로컬 좌표계에서의 오프셋 계산
                Vector3 localOffset = Vector3.right * 0.5f * (i - (crossfireLevel - 1) / 2.0f);

                // 로컬 좌표계에서 월드 좌표계로 변환
                Vector3 spawnPosition = firePos.transform.TransformPoint(localOffset + firePos.localPosition);
                GameObject Bullet = Instantiate(bullet, spawnPosition, transform.rotation);
                Bullet.GetComponent<BulletCtrl>().Pdmg = dmg;
            }
            else
            {
                GameObject Bullet = Instantiate(bullet, firePos.position, transform.rotation);
                Bullet.GetComponent<BulletCtrl>().Pdmg = dmg;
            }

        }
        //fan_Attack
        if (fan_fireLevel >= 1)
        {
            for (int i = 1; i <= fan_fireLevel; i++)
            {
                float angle = i * (90 / (fan_fireLevel + 1));
                Quaternion rotation = transform.rotation * Quaternion.Euler(0, angle, 0);
                GameObject Bullet = Instantiate(bullet, firePos.position, rotation);
                Bullet.GetComponent<BulletCtrl>().Pdmg = dmg;
            }
            for (int i = 1; i <= fan_fireLevel; i++)
            {
                float angle = -1 * i * (90 / (fan_fireLevel + 1));
                Quaternion rotation = transform.rotation * Quaternion.Euler(0, angle, 0);
                GameObject Bullet = Instantiate(bullet, firePos.position, rotation);
                Bullet.GetComponent<BulletCtrl>().Pdmg = dmg;
            }
        }
        yield return new WaitForSeconds(0.3f);
        canMove = true;

        yield return new WaitForSeconds(firespeed);
        isAttack = false;
    }
    void Die()
    {
        if (transform.position.y < -10)
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
            if (Hp <= 0) return;
            if (other.gameObject.name == "HP_Rock(Clone)")
            {
                Hp -= other.GetComponent<G_Stone>().dmg;

            }
            else if (other.gameObject.name == "HP_Golem")
            {
                Hp -= other.GetComponent<Boss_Golem>().dmg;
            }
            else if (other.gameObject.name == "PunchPoint")
            {
                Hp -= other.GetComponent<G_Stone>().dmg;
            }
            else if (other.gameObject.name == "Bloody_Axe(Clone)")
            {
                Hp -= other.GetComponent<O_Axe>().dmg;
            }
            else if (other.gameObject.name == "GruntHP")
            {
                Hp -= other.GetComponent<Boss_Orc>().dmg;
            }
            else if (other.gameObject.name == "AXEPoint")
            {
                Hp -= other.GetComponent<O_Axe>().dmg;
            }
            else if (other.gameObject.name == "PunchPoint2")
            {
                Hp -= other.GetComponent<O_Axe>().dmg;
            }
            else if (other.gameObject.name == "Red")
            {
                Hp -= other.GetComponent<Boss_Dragon>().dmg;
            }
            else if (other.gameObject.name == "BitePoint")
            {
                Hp -= other.GetComponent<D_Fire>().dmg;
            }
            else if (other.gameObject.name == "DrivePoint")
            {
                Hp -= other.GetComponent<D_Fire>().dmg;
            }
            else if (other.gameObject.name == "CFXR Fire Breath(Clone)")
            {
                Hp -= other.GetComponent<D_Fire>().dmg;
            }
            else if (other.gameObject.name == "EnemyBullet(Clone)")
            {
                Debug.Log("맞음");

                Hp -= other.GetComponent<EBulletCtrl>().Edmg;
                Destroy(other.gameObject);
            }
            ani.Play("GetHit");
        }

        //getEXP
        if (other.tag == "EXP")
        {
            playerExp += other.GetComponent<Exp>().exp;
            if (playerExp >= playerMaxExp)
            {
                playerExp += other.GetComponent<Exp>().exp;
                playerLevel += 1;
                playerExp -= playerMaxExp;
                playerMaxExp += 50;
                isLevelUp = true;
            }

            //Debug.Log(playerExp);
        }
    }
    void HpCheck()
    {
        if (hpImage == null) return;
        hpImage.fillAmount = Hp / maxHp;
        hpText.text = Hp.ToString() + "/" + maxHp.ToString();

    }
    void EXPCheck()
    {
        if (expImage == null) return;
        expImage.fillAmount = playerExp / playerMaxExp;
        expText.text = "Lv" + playerLevel.ToString();
    }
}
