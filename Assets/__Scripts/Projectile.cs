using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{   
    private BoundsCheck bndCheck;
    private Renderer rend;
    [Header("Set Dynamically")]
    public Rigidbody rigid;
    private Color color;
    [SerializeField]
    private WeaponType _type;
    public bool isCollides = false;
    public GameObject collideWith;
    private float birthTime;
    private float x0;
    [Header("Additional Projectile")]
    public GameObject additionalProjectile = null;

    // a
    // b
    // Это общедоступное свойство маскирует поле _type и обрабатывает
    // операции присваивания ему нового значения
    public WeaponType type
    {
        get { return _type; }
        set { SetType(value); }
    }


    void Awake() 
    {
        bndCheck = GetComponent<BoundsCheck>();
        rend = GetComponent<Renderer>();
        rigid = GetComponent<Rigidbody>();
        color = rend.material.color;
    }
    void Start()
    {
        x0 = transform.position.x;
        birthTime = Time.time;
    }

    void Update() 
    {
        if(type!=WeaponType.laser)
            if (bndCheck.offUp || bndCheck.offDown || bndCheck.offLeft || bndCheck.offRight)
            {
                Destroy(gameObject);
            }
        if(type == WeaponType.phaser)
        {
            float theta = Mathf.PI * 2 * (Time.time - birthTime) / 0.25f;
            float sin = Mathf.Sin(theta);
            transform.position = new Vector3(x0 + 2f * sin, transform.position.y, transform.position.z);
        }
        if(type == WeaponType.missile)
        {
            rigid.velocity += Time.deltaTime*25*Vector3.Normalize(rigid.velocity);
        }
        if (type == WeaponType.enemyMissile)
        {
            float velX = 8;
            if (Hero.S != null && transform.position.x > Hero.S.transform.position.x)
                velX = -velX;
            rigid.velocity += Time.deltaTime * 15 * Vector3.Normalize(rigid.velocity);
            if (Hero.S != null)
            {
                transform.position = new Vector3(transform.position.x + velX*Time.deltaTime, transform.position.y, transform.position.z);
            }
            if(Time.time - birthTime > 2f || transform.position.y < -bndCheck.camHeight + Explosion.size/2 + 2)
            {
                GameObject explosion = Instantiate<GameObject>(additionalProjectile);
                explosion.transform.position = transform.position;
                Destroy(gameObject);
            }
            if ((int)(Time.time * 5) % 2 == 1)
            {
                rend.material.color = Color.red;
            }
            else
            {
                rend.material.color = color;
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "PowerUp")
        {
            isCollides = true;
            collideWith = collision.gameObject;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag != "PowerUp")
        {
            isCollides = true;
            collideWith = collision.gameObject;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag != "PowerUp")
        {
            isCollides = false;
            collideWith = null;
        }
    }

    public void SetType(WeaponType eType)
    { // e
        _type = eType;
        WeaponDefinition def = Main.GetWeaponDefinition(_type);
        rend.material.color = def.projectileColor;
    }
}
