using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Part
{     
    public float health;
    public string[] protectedBy;
    public string name;

    [HideInInspector]
    public GameObject go;
    [HideInInspector]
    public Material[] mat;
    [HideInInspector]
    public Color[] colors;
    [HideInInspector]
    public bool isDestroyed = false;
}


public class Boss_1 : Enemy
{
    [Header("Set in Inspector: Enemy_4")]
    public Part[] parts;
    private float duration = 4; // ����������������� �����������
    public float rollMult = -45;
    public float pitchMult = 15;

    [Header("Set Dinamycally")]
    private Vector3 p0, p1; // ��� ����� ��� ������������
    private float timeStart; // ����� �������� ����� �������
    private float xRot;
    private float yRot;
    private float zRot;


    void Start()
    {
        // ��������� ������� ��� ������� � Main.SpawnEnemy(),
        // ������� ������� �� ��� ��������� �������� � �0 � pl
        p0 = p1 = pos; //
        InitMovement();
        isBoss = true;
        ShowLocalizedDestruction();
        collideOffset = 0.5f;
        direction = transform.forward;
        //Invoke("MissileFire", 3f);

        Transform t;
        foreach (Part prt in parts)
        {
            t = transform.Find(prt.name);
            if (t != null)
            {
                prt.go = t.gameObject;
                prt.mat = Utils.GetAllMaterials(prt.go);
                prt.colors = new Color[prt.mat.Length];
                for(int i = 0; i < prt.mat.Length; i++)
                {
                    prt.colors[i] = prt.mat[i].color;
                }
            }
        }

        xRot = this.transform.rotation.eulerAngles.x;
        yRot = this.transform.rotation.eulerAngles.y;
        zRot = this.transform.rotation.eulerAngles.z;
    }
    void InitMovement()
    { // b
        p0 = p1; // ���������� pl � p0
                 // ������� ����� ����� pl �� ������
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(0, hgtMinRad);
        // �������� �����
        timeStart = Time.time;
    }
    public override void Move()
    { // c
      // ���� ����� �������������� Enemy.Move() � ���������
      // �������� ������������
        float u = (Time.time - timeStart) / duration;
        if(u > 0.5f)
        {
            transform.GetChild(4).GetComponent<Weapon>().Fire();
            transform.GetChild(5).GetComponent<Weapon>().Fire();
        }
        if (u >= 1)
        {
            MissileFire();
            InitMovement();
            u = 0;
        }
        
        u = 1 - Mathf.Pow(1 - u, 2); // ��������� ������� ����������
        //pos = (1 - u) * p0 + u * p1; // ������� �������� ������������
        pos = Vector3.Slerp(p0, p1, u);

        int xAxis = (int) Mathf.Sign(p1.x - p0.x); // b
        int yAxis = (int) Mathf.Sign(p1.y - p0.y);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0) * Quaternion.Euler(xRot, yRot, zRot), 0.001f);

        
        if (showingDamage && Time.time > damageDoneTime)
        { // c
            UnShowDamage();
        }
    }

    void MissileFire()
    {
        transform.GetChild(6).GetComponent<Weapon>().Fire();
        //Invoke("MissileFire", 3f);
    }


    Part FindPart(string n)
    { // �
        foreach (Part prt in parts)
        {
            if (prt.name == n)
            {
                return (prt);
            }
        }
        return (null);
    }
    Part FindPart(GameObject go)
    { // b
        foreach (Part prt in parts)
        {
            if (prt.go == go)
            {
                return (prt);
            }
        }
        return (null);
    }
    // ��� ������� ���������� true, ���� ������ ����� ����������
    bool Destroyed(GameObject go)
    { // �
        return (Destroyed(FindPart(go)));
    }
    bool Destroyed(string n)
    {
        return (Destroyed(FindPart(n)));
    }
    bool Destroyed(Part prt)
    {
        if (prt == null)
        { // ���� ������ �� ����� �� ���� ��������
            return (true); // ������� true (�� ����: ��, ���� ����������)
        }
        // ������� ��������� ���������: prt.health <= 0
        // ���� prt.health <= 0, ������� true (��, ���� ����������)
        return (prt.health <= 0);
    }
    // ���������� � ������� ������ ���� �����, � �� ���� �������.
    void ShowLocalizedDamage(Material[] m)
    { // d
        foreach (Material mat in m)
        {
            mat.color = Color.red;
            damageDoneTime = Time.time + showDamageDuration;
            showingDamage = true;
        }
    }
    // �������������� ����� OnCollisionEnter �� �������� Enemy.cs.
    void OnCollisionEnter(Collision coll)
    {
        GameObject other = coll.gameObject;
        switch (other.tag)
        {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                // ���� ������� �� ��������� ������, �� ���������� ���.
                if ( !bndCheck.isOnScreen ) 
                {
                    if (other.GetComponent<Projectile>().type != WeaponType.laser)
                        Destroy(other);
                    break;
                }
                // �������� ��������� �������
                GameObject goHit = coll.contacts[0].thisCollider.gameObject; // f
                Part prtHit = FindPart(goHit);
                if (prtHit == null)
                { // If prtHit wasn't found... // g
                    goHit = coll.contacts[0].otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }
                // ���������, �������� �� ��� ��� ����� �������
                if (prtHit.protectedBy != null) { // h
                    foreach (string s in prtHit.protectedBy)
                    {
                        // ���� ���� �� ���� �� ���������� ������ ���
                        // �� ���������...
                        if (!Destroyed(s))
                        {
                            // ...�� �������� ����������� ���� �����
                            if(other.GetComponent<Projectile>().type != WeaponType.laser)
                                Destroy(other); // ���������� ������ ProjectileHero
                            return; // �����, �� ��������� Enemy_4
                        }
                    }
                }
                // ��� ����� �� ��������, ������� �� �����������
                // �������� ����������� ���� �� Projectile.type � Main.WEAP_DICT
                prtHit.health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                // �������� ������ ��������� � �����
                ShowLocalizedDamage(prtHit.mat);
                if (prtHit.health <= 0)
                { // i
                  // ������ ���������� ����� �������
                  // �������������� ������������ �����
                  //prtHit.go.SetActive(false);
                    prtHit.isDestroyed = true;
                    prtHit.go.GetComponent<Collider>().enabled = false;
                }
                // ���������, ��� �� ������� ��������� ��������
                bool allDestroyed = true; // ������������, ��� ��������
                foreach (Part prt in parts)
                {
                    if (!Destroyed(prt))
                    { // ���� �����-�� ����� ��� ����������...
                        allDestroyed = false; // ...�������� false � allDestroyed
                        break; // � �������� ���� foreach
                    }
                }
                if (allDestroyed)
                { // ���� ������� �������� ���������... // j
                  // ...��������� ������-�������� Main, ��� ���� ������� ��������
                    Main.S.ShipDestroyed(this);
                    // ���������� ���� ������ Enemy
                    Destroy(this.gameObject);
                }
                if(other.GetComponent<Projectile>().type != WeaponType.laser)
                    Destroy(other); // ���������� ������ ProjectileHero
                break;
        }
    }
    private void OnCollisionStay(Collision coll)
    {
        GameObject other = coll.gameObject;
        if (other.tag == "ProjectileHero")
        {
            Projectile p = other.GetComponent<Projectile>();
            if (p.type == WeaponType.laser)
            {
                if (!bndCheck.isOnScreen)
                    return;
                GameObject goHit = coll.contacts[0].thisCollider.gameObject; // f
                Part prtHit = FindPart(goHit);
                if (prtHit == null)
                { // If prtHit wasn't found... // g
                    goHit = coll.contacts[0].otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }
                // ���������, �������� �� ��� ��� ����� �������
                if (prtHit.protectedBy != null)
                { // h
                    foreach (string s in prtHit.protectedBy)
                    {
                        // ���� ���� �� ���� �� ���������� ������ ���
                        // �� ���������...
                        if (!Destroyed(s))
                        {
                            return; // �����, �� ��������� Enemy_4
                        }
                    }
                }
                // ��� ����� �� ��������, ������� �� �����������
                // �������� ����������� ���� �� Projectile.type � Main.WEAP_DICT
                prtHit.health -= Main.GetWeaponDefinition(p.type).continuousDamage;
                // �������� ������ ��������� � �����
                ShowLocalizedDamage(prtHit.mat);
                if (prtHit.health <= 0)
                { // i
                  // ������ ���������� ����� �������
                  // �������������� ������������ �����
                  //prtHit.go.SetActive(false);
                    prtHit.isDestroyed = true;
                    prtHit.go.GetComponent<Collider>().enabled = false;
                }
                // ���������, ��� �� ������� ��������� ��������
                bool allDestroyed = true; // ������������, ��� ��������
                foreach (Part prt in parts)
                {
                    if (!Destroyed(prt))
                    { // ���� �����-�� ����� ��� ����������...
                        allDestroyed = false; // ...�������� false � allDestroyed
                        break; // � �������� ���� foreach
                    }
                }
                if (allDestroyed)
                { // ���� ������� �������� ���������... // j
                  // ...��������� ������-�������� Main, ��� ���� ������� ��������
                    Main.S.ShipDestroyed(this);
                    // ���������� ���� ������ Enemy
                    Destroy(this.gameObject);
                }
            }
        }
    }

    public void ShowLocalizedDestruction()
    {
        foreach (Part prt in parts)
        {
            if (prt.isDestroyed)
            {
                foreach (Material m in prt.mat)
                {
                    m.color = Color.red;
                }
            }
        }
        Invoke("UnShowLocalizedDestruction", 0.5f);
    }
    public void UnShowLocalizedDestruction()
    {
        foreach (Part prt in parts)
        {
            if (prt.isDestroyed)
            {
                for (int i = 0; i < prt.mat.Length; i++)
                {
                    prt.mat[i].color = prt.colors[i];
                }
            }
        }
        Invoke("ShowLocalizedDestruction", 0.5f);
    }
}

