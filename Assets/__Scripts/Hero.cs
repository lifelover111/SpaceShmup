using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S;
    [Header("Set in Inspector")]
    // Поля, управляющие движением корабля
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 15;
    public float gameRestartDelay = 2f;
    public Weapon[] weapons;
    public float invulnerabilityTime = 2f; 
    

    [Header("Set Dynamically")]
    [SerializeField] private float _shieldLevel = 0;

    public delegate void WeaponFireDelegate();
    public WeaponFireDelegate fireDelegate;

    private float takeDamageTime;
    private Renderer[] heroRend;
    private int damageTaking = 1;
    public bool isInvulnerabilious = false;

    public float shieldLevel
    {
        get
        {
            return (_shieldLevel);
        }
        set
        {
            _shieldLevel = Mathf.Min(value, 4);
            if (value < 0)
            {
                Destroy(this.gameObject);
                Main.S.DelayedRestart(gameRestartDelay);
            }
        }
    }


    void Awake()
    {
        if (S == null)
        {
            S = this; // Сохранить ссылку на одиночку // а
        }
        else
        {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }
        heroRend = S.gameObject.GetComponentsInChildren<Renderer>();
    }
    void Start()
    {
        weapons[0].type = WeaponType.blaster;
    }
    void Update()
    {
        // Извлечь информацию из класса Input
        float xAxis = Input.GetAxis("Horizontal"); // b
        float yAxis = Input.GetAxis("Vertical"); // b
                                                 // Изменить transform.position, опираясь на информацию по осям
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;
        // Повернуть корабль, чтобы придать ощущение динамизма // с
        transform.rotation = Quaternion.Euler(yAxis * pitchMult - 90, xAxis * rollMult, 0);

        
        if (Input.GetAxis("Jump") == 1 && fireDelegate != null)
        {
            fireDelegate();
        }

    }


    void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        print("Triggered: " + go.name);

        
        if (go.tag == "Boss")
        {
            if (!isInvulnerabilious)
            {
                shieldLevel--;
                takeDamageTime = Time.time;
                InvulnerabilityWindow();
            }
        }
        else if (go.tag == "Enemy")
        {
            if (!isInvulnerabilious)
            {
                shieldLevel--; // Уменьшить уровень защиты на 1
                takeDamageTime = Time.time;
                Destroy(go); // ... и уничтожить врага И е
                InvulnerabilityWindow();
            }
        }
        else if (go.tag == "PowerUp")
        {
            // Если защитное поле столкнулось с бонусом
            AbsorbPowerUp(go);
        }
        else if(other.gameObject.tag == "ProjectileEnemy")
        {
            if (!isInvulnerabilious)
            {
                shieldLevel--;
                takeDamageTime = Time.time;
                InvulnerabilityWindow();
            }
        }
        else
        {
            print("Triggered by non-Enemy: " + go.name);
        }
    }

    public void InvulnerabilityWindow()
    {
        isInvulnerabilious = true;
        InvulnerabilityAnimation();
    }
    private void InvulnerabilityAnimation()
    {
        if(Time.time - takeDamageTime < invulnerabilityTime)
        {
            foreach(Renderer rend in heroRend)
                rend.enabled = System.Convert.ToBoolean((1 - damageTaking) / 2);
            damageTaking *= -1;
            Invoke("InvulnerabilityAnimation", 0.1f);
        }
        else
        {
            foreach (Renderer rend in heroRend)
                rend.enabled = true;
            damageTaking = 1;
            isInvulnerabilious = false;
            return;
        }
    }
    public void AbsorbPowerUp(GameObject go)
    {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type)
        {
            case WeaponType.shield:
                shieldLevel++;
                break;
            case WeaponType.blaster:
                if (weapons[0].type == WeaponType.blaster)
                {
                    ClearWeapons();
                    weapons[1].type = WeaponType.blaster;
                    weapons[2].type = WeaponType.blaster;
                }
                else if (weapons[1].type == WeaponType.blaster)
                {
                    break;
                }
                else
                {
                    ClearWeapons();
                    weapons[0].type = WeaponType.blaster;
                }
                break;
            case WeaponType.spread:
                ClearWeapons();
                weapons[0].type = WeaponType.spread;
                break;
            case WeaponType.laser:
                ClearWeapons();
                weapons[4].type = WeaponType.laser;
                break;
            case WeaponType.phaser:
                ClearWeapons();
                weapons[3].type = WeaponType.phaser;
                break;
            case WeaponType.missile:
                ClearWeapons();
                weapons[3].type = WeaponType.missile;
                break;
        }
        pu.AbsorbedBy(this.gameObject);
    }
    void ClearWeapons()
    {
        foreach (Weapon w in weapons)
        {
            w.SetType(WeaponType.none);
        }
    }

}
