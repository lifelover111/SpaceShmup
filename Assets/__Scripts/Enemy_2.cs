using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : Enemy
{
    [Header("Set in Inspector: Enemy_2")]
    // Определяют, насколько ярко будет выражен синусоидальный характер движения
    public float sinEccentricity = 0.6f;
    public float lifeTime = 10;
    public Weapon[] weapons;
    
    [Header("Set Dynamically: Enemy_2")]
    // Enemy_2 использует линейную интерполяцию между двумя точками,
    // изменяя результат по синусоиде
    public Vector3 p0;
    public Vector3 p1;
    public float birthTime;
    private Vector3 moveDirection;
    private float xRot;
    private float yRot;
    private float zRot;
    static private float zeroVelocityT1;
    static private float zeroVelocityT2;
    private Vector3 rotAxis;



    void Start()
    {
        xRot = this.transform.rotation.eulerAngles.x;
        yRot = this.transform.rotation.eulerAngles.y;
        zRot = this.transform.rotation.eulerAngles.z;
        zeroVelocityT1 = Mathf.Acos(-1 / (sinEccentricity * 2 * Mathf.PI)) / (2 * Mathf.PI);
        zeroVelocityT2 = -Mathf.Acos(-1 / (sinEccentricity * 2 * Mathf.PI)) / (2 * Mathf.PI) + 1;
        rotAxis = Vector3.right;

        // Выбрать случайную точку на левой границе экрана
        p0 = Vector3.zero; // b
        p0.x = -bndCheck.camWidth - bndCheck.radius;
        p0.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);
        // Выбрать случайную точку на правой границе экрана
        p1 = Vector3.zero;
        p1.x = bndCheck.camWidth + bndCheck.radius;
        p1.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);
        // Случайно поменять начальную и конечную точку местами
        if (Random.value > 0.5f)
        {
            // Изменение знака .х каждой точки
            // переносит ее на другой край экрана
            p0.x *= -1;
            p1.x *= -1;
        }
        // Записать в birthTime текущее время
        birthTime = Time.time;
        moveDirection = new Vector3(p1.x - p0.x, p1.y - p0.y, 0);
        
        this.transform.rotation = Quaternion.FromToRotation(Vector3.down, moveDirection) * Quaternion.Euler(xRot, yRot, zRot);
        Invoke("Fire", 0f);
        Invoke("Fire", 0.3f);
        Invoke("Fire", 0.6f);

        Invoke("Fire", 4.2f);
        Invoke("Fire", 4.5f);

        Invoke("Fire", 7.2f);
        Invoke("Fire", 7.5f);
        Invoke("Fire", 7.8f);
        direction = transform.forward;
        collideOffset = 1f;
    }
    private void Fire()
    {
        if (fireDelegate != null)
        {
            fireDelegate();
        }
    }

    public override void Move()
    {
        Vector3 tempPos = pos;
        bool isReverted = false;
        // Кривые Безье вычисляются на основе значения и между 0 и 1
        float t = (Time.time - birthTime) / lifeTime;
        // Если u>1, значит, корабль существует дольше, чем lifeTime
        if (t > 1)
        {
            // Этот экземпляр Enemy_2 завершил свой жизненный цикл
            Destroy(this.gameObject); // d
            return;
        }
        // Скорректировать u добавлением значения кривой, изменяющейся по синусоиде
        float u = t + sinEccentricity * (Mathf.Sin(t * Mathf.PI * 2));
        pos = new Vector3((1 - u) * p0.x + u * p1.x, (1 - u) * p0.y + u * p1.y,pos.z);
        moveDirection.x = pos.x - tempPos.x;
        moveDirection.y = pos.y - tempPos.y;


        if (Mathf.Abs(zeroVelocityT1 - t) < 0.1)
        {
            StartRotatingX();
            this.transform.localScale = Vector3.Slerp(this.transform.localScale, new Vector3(0.28f, 0.28f, 0.28f), 0.005f);
            pos = new Vector3(pos.x,pos.y,5);
            direction = transform.forward;
        }
        else if (t - zeroVelocityT1 < 0.2 && t > zeroVelocityT1 - 0.1)
        {
            StartRotatingY();
        }
        if (Mathf.Abs(zeroVelocityT2 - t) < 0.1)
        {
            if(!isReverted)
            {
                rotAxis = Vector3.left;
                isReverted = false;
            }
            StartRotatingX();
            this.transform.localScale = Vector3.Slerp(this.transform.localScale, new Vector3(0.4f, 0.4f, 0.4f), 0.005f);
            pos = new Vector3(pos.x,pos.y,0);
            direction = transform.forward;
        }
        else if (t - zeroVelocityT2 < 0.2 && t > zeroVelocityT2 - 0.1)
        {
            StartRotatingY();
        }
    }

    private void StartRotatingX()
    {
        this.transform.Rotate(rotAxis, 90*Time.deltaTime);
    }
    private void StartRotatingY()
    {
        this.transform.Rotate(Vector3.forward, 180 * Time.deltaTime);
    }
}
