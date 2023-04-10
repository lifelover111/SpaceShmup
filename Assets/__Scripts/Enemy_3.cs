using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_3 : Enemy
{
    [Header("Set in Inspector: Enemy_3")]
    public float lifeTime = 15;
    [Header("Set Dynamically: Enemy_3")]
    public Vector3[] points;
    public float birthTime;
    private float xRot;
    private float yRot;
    private float zRot;
    // И снова метод Start хорошо подходит для наших целей,
    // потому что не используется суперклассом Enemy
    void Start()
    {
        xRot = this.transform.rotation.eulerAngles.x;
        yRot = this.transform.rotation.eulerAngles.y;
        zRot = this.transform.rotation.eulerAngles.z;

        points = new Vector3[3]; // Инициализировать массив точек
                                 // Начальная позиция уже определена в Main.SpawnEnemy()
        points[0] = pos;
        // Установить xMin и хМах так же, как это делает Main.SpawnEnemy()
        float xMin = -bndCheck.camWidth + bndCheck.radius;
        float xMax = bndCheck.camWidth - bndCheck.radius;
        Vector3 v;
        // Случайно выбрать среднюю точку ниже нижней границы экрана
        v = Vector3.zero;
        v.x = Random.Range(xMin, xMax);
        v.y = -bndCheck.camHeight * Random.Range(1, 2);
        points[1] = v;
        // Случайно выбрать конечную точку выше верхней границы экрана
        v = Vector3.zero;
        v.y = pos.y;
        v.x = Random.Range(xMin, xMax);
        points[2] = v;
        // Записать в birthTime текущее время
        birthTime = Time.time;
        collideOffset = 0f;
        Invoke("Fire",0.2f);
        Invoke("Fire",1.5f);
        Invoke("Fire", 1.9f);
    }
    private void Fire()
    {
        fireDelegate();
    }
    public override void Move()
    {
        Vector3 tempPos = pos;
        // Кривые Безье вычисляются на основе значения и между 0 и 1
        float u = (Time.time - birthTime) / lifeTime;
        if (u > 1)
        {
            // Этот экземпляр Enemy_2 завершил свой жизненный цикл
            Destroy(this.gameObject);
            return;
        }
        // Интерполировать кривую Безье по трем точкам
        Vector3 p01, p12;
        p01 = (1 - u) * points[0] + u * points[1];
        p12 = (1 - u) * points[1] + u * points[2];
        pos = new Vector3((1 - u) * p01.x + u * p12.x, (1 - u) * p01.y + u * p12.y, pos.z);
        if (pos.y - tempPos.y < 0)
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(new Vector3(0, 600 * (tempPos.x - pos.x), 0)) * Quaternion.Euler(xRot, yRot, zRot), 0.01f);
        else
        {
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(new Vector3(250 * (pos.y - tempPos.y), 600 * (tempPos.x - pos.x), 0)) * Quaternion.Euler(xRot, yRot, zRot), 0.01f);
            pos = new Vector3(pos.x, pos.y, -3);
            this.transform.localScale = Vector3.Slerp(this.transform.localScale, new Vector3(0.63f, 0.63f, 0.63f), 0.0005f);
        }
    }

}
