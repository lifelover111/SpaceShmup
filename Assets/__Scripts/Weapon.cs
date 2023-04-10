using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum WeaponType
{
    none,     //По умолчанию / нет оружия
    blaster,  //Простой бластер
    enemyBlaster,
    enemyMissile,
    spread,   //Веерная пушка, стреляющая несколькими снарядами
    phaser,   //[HP] Волновой фазер
    missile,  //[HP] Самонаводящиеся ракеты
    laser,    //[HP] Наносит повреждения при долговременном воздействии
    shield    //Увеличивает shieldLevel
}

[System.Serializable]
public class WeaponDefinition
{
    public WeaponType type = WeaponType.none;
    public string letter;
    public Color color = Color.white;
    public GameObject projectilePrefab;
    public Color projectileColor = Color.white;
    public float damageOnHit = 0;
    public float continuousDamage = 0;

    public float delayBetweenShots = 0;
    public float velocity = 20;
    public AudioClip weaponSound;
}

public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;
    [Header("Set Dynamically")] [SerializeField]
    private WeaponType _type = WeaponType.none;
    public WeaponDefinition def;
    public float lastShotTime;
    private Projectile laserProjectile;
    private AudioSource weaponSound;

    void Start()
    {
        // Вызвать SetType(), чтобы заменить тип оружия по умолчанию
        // WeaponType.попе
        def = Main.GetWeaponDefinition(_type);
        SetType(_type);

        if (PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }
        GameObject rootGO = transform.root.gameObject;
        if (rootGO.GetComponent<Hero>() != null)
        {
            rootGO.GetComponent<Hero>().fireDelegate += Fire;
        }
        if (rootGO.GetComponent<Enemy>() != null)
        {
            if (rootGO.tag != "Boss")
                rootGO.GetComponent<Enemy>().fireDelegate += Fire;
        }
        weaponSound = gameObject.GetComponent<AudioSource>();
        if (def.weaponSound != null)
            weaponSound.clip = def.weaponSound;
    }
    void Update()
    {
        if(laserProjectile != null && Input.GetAxis("Jump") == 0)
        {
            Destroy(laserProjectile.gameObject);
            laserProjectile = null;
        }
        if (laserProjectile != null && type != WeaponType.laser)
        {
            Destroy(laserProjectile.gameObject);
            laserProjectile = null;
        }
    }
    public WeaponType type
    {
        get { return ( _type ); }
        set { SetType(value);
            weaponSound.clip = def.weaponSound;
        }
    }
    public void SetType(WeaponType wt)
    {
        _type = wt;
        if (type == WeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        def = Main.GetWeaponDefinition(_type);
        lastShotTime = 0; // Сразу после установки „type можно выстрелить 
    }
    public void Fire()
    {
        // Если this.gameObject неактивен, выйти
        if (!gameObject.activeInHierarchy) return; // h
                                                   // Если между выстрелами прошло недостаточно много времени, выйти
        if (Time.time - lastShotTime < def.delayBetweenShots)
        { // i
            return;
        }
        Projectile p;
        Vector3 vel;
        if (transform.root.gameObject.tag == "Hero")
            vel = Vector3.up * def.velocity; // j
        else
        {
            vel = transform.root.gameObject.GetComponent<Enemy>().direction;
            vel.z = 0;
            vel = Vector3.Normalize(vel);
            vel = vel * def.velocity;
        }
        if(weaponSound.clip != null)
        {
            weaponSound.Play();
        }
        
        switch (type)
        { // k
            case WeaponType.blaster:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                break;
            case WeaponType.spread:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                p = MakeProjectile(); // Снаряд, летящий вправо
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                p = MakeProjectile(); // Снаряд, летящий влево
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                break;
            case WeaponType.enemyBlaster:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                p.transform.rotation = Quaternion.FromToRotation(Vector3.up, vel);
                break;
            case WeaponType.enemyMissile:
                p = MakeProjectile();
                vel = Vector3.down * def.velocity;
                p.rigid.velocity = vel;
                p.transform.rotation = Quaternion.FromToRotation(Vector3.up, vel);
                break;
            case WeaponType.laser:
                if (laserProjectile == null)
                { 
                    laserProjectile = MakeProjectile();
                }
                laserProjectile.gameObject.transform.position = new Vector3(transform.position.x,transform.position.y + laserProjectile.gameObject.transform.localScale.y / 2, transform.position.z);
                if (laserProjectile.isCollides)
                {
                    if (laserProjectile.collideWith != null)
                        laserProjectile.gameObject.transform.localScale = new Vector3(laserProjectile.gameObject.transform.localScale.x, Mathf.Abs(laserProjectile.collideWith.transform.position.y - laserProjectile.collideWith.GetComponent<Enemy>().collideOffset - transform.position.y), laserProjectile.gameObject.transform.localScale.z);
                    else
                        laserProjectile.gameObject.transform.localScale = new Vector3(laserProjectile.gameObject.transform.localScale.x, (laserProjectile.gameObject.transform.localScale.y + vel.magnitude), laserProjectile.gameObject.transform.localScale.z);
                }
                else
                {
                    if (laserProjectile.transform.localScale.y < Camera.main.orthographicSize * 2)
                        laserProjectile.gameObject.transform.localScale = new Vector3(laserProjectile.gameObject.transform.localScale.x, (laserProjectile.gameObject.transform.localScale.y + vel.magnitude), laserProjectile.gameObject.transform.localScale.z);
                    else
                        laserProjectile.gameObject.transform.localScale = new Vector3(laserProjectile.gameObject.transform.localScale.x, Camera.main.orthographicSize * 2, laserProjectile.gameObject.transform.localScale.z);
                }
                break;
            case WeaponType.phaser:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                break;
            case WeaponType.missile:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                break;
        }
    }
    public Projectile MakeProjectile()
    { // m
        GameObject go = Instantiate<GameObject>(def.projectilePrefab);
        if (transform.parent.gameObject.tag == "Hero" ) 
        {
            go.tag = "ProjectileHero";
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        } 
        else
        {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }
        go.transform.position = transform.position;
        go.transform.SetParent(PROJECTILE_ANCHOR, true); // о
        Projectile p = go.GetComponent<Projectile>();
        p.type = type;
        lastShotTime = Time.time; // p
        return (p);
    }
}
