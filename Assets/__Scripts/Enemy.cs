using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    [Header("Set in Inspector: Enemy")]
    public float speed = 10f;
    public float fireRate = 0.3f;
    public float health = 10;
    public int score = 100;
    public float powerUpDropChance = 1f;
    protected BoundsCheck bndCheck;
    public float showDamageDuration = 0.1f;
    public float collideOffset;
    [Header("Set Dinamycally")]
    public Color[] originalColors;
    public Material[] materials;
    public bool showingDamage = false;
    public float damageDoneTime;
    public bool notifiedOfDestruction = false;
    public bool isBoss = false;
    public delegate void WeaponFireDelegate();
    public WeaponFireDelegate fireDelegate;
    public Vector3 direction;
    public float bndRadius;
    
    
    void Awake()
    { // b
        bndCheck = GetComponent<BoundsCheck>();
        bndRadius = bndCheck.radius;
        // Получить материалы и цвет этого игрового объекта и его потомков
        materials = Utils.GetAllMaterials(gameObject); // b
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }

    public Vector3 pos
    {
        get 
        {
            return (this.transform.position);
        }
        set
        {
            this.transform.position = value;
        }
    }
    void Update()
    {
        Move();

        if (showingDamage && Time.time > damageDoneTime && !isBoss)
        { // c
            UnShowDamage();
        }

        if (bndCheck != null && bndCheck.offDown)
        {
            Destroy(gameObject);
        }
    }
    public virtual void Move()
    { // b
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject;
        switch (otherGO.tag)
        {
            case "ProjectileHero": // b
                Projectile p = otherGO.GetComponent<Projectile>();
                // Если вражеский корабль за границами экрана,
                // не наносить ему повреждений.
                if ( !bndCheck.isOnScreen ) { //с
                    if(p.type != WeaponType.laser)
                        Destroy(otherGO);
                    break;
                }
                // Получить разрушающую силу из WEAP_DICT в классе Main,
                health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                ShowDamage();
                if (health <= 0)
                { // d
                  // Сообщить объекту-одиночке Main об уничтожении
                    if (!notifiedOfDestruction)
                    {
                        Main.S.ShipDestroyed(this);
                    }
                    notifiedOfDestruction = true;
                    Destroy(this.gameObject);
                    UI.S.score += score;
                }
                if (p.type != WeaponType.laser)
                    Destroy(otherGO); // e
                break;
            default:
                print("Enemy hit by non-ProjectileHero: " + otherGO.name); // f
                break;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        GameObject otherGO = collision.gameObject;
        if (otherGO.tag == "ProjectileHero")
            if (!bndCheck.isOnScreen)
                return;
            else
            {
                if (otherGO.GetComponent<Projectile>().type == WeaponType.laser)
                {
                    if (health <= 0)
                        Destroy(gameObject);
                    health -= Main.GetWeaponDefinition(WeaponType.laser).continuousDamage;
                    ShowDamage();
                }
            }
    }


    void ShowDamage()
    { // е
        foreach (Material m in materials)
        {
            m.color = Color.red;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }
    protected void UnShowDamage()
    { // f
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
    }
}
