using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Enemy
{
    [Header("Set in Inspector: Enemy_1")]
    // ����� ������ ������� ����� ���������
    public float waveFrequency = 2;
    // ������ ��������� � ������
    public float waveWidth = 4;
    public float waveRotY = 30;
    public Weapon[] weapons;
    public AudioClip startSound;

    [Header("Set Dynamically: Enemy_1")]
    private float x0;
    private float birthTime;
    
    private float xRot;
    private float yRot;
    private float zRot;

    void Start()
    {
        // ���������� ��������� ���������� X ������� Enemy_l
        x0 = pos.x; // b
        birthTime = Time.time;
        xRot = this.transform.rotation.eulerAngles.x;
        yRot = this.transform.rotation.eulerAngles.y;
        zRot = this.transform.rotation.eulerAngles.z;
        direction = -transform.up;
        collideOffset = 3.5f;
        gameObject.GetComponent<AudioSource>().clip = startSound;
        gameObject.GetComponent<AudioSource>().Play();
    }

    public override void Move()
    { // c
      // ��� ��� pos * ��� ��������, ������ �������� �������� pos.x
      // ������� ������� pos � ���� ������� Vector3, ���������� ��� ���������
        Vector3 tempPos = pos;
        // �������� theta ���������� � �������� �������
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;
        if (1 - Mathf.Abs(sin) <= 0.05f)
        {
            fireDelegate();
        }
        // ��������� ������� ������������ ��� Y\
        Vector3 rot = new Vector3(xRot, yRot + sin * waveRotY, zRot);
        this.transform.rotation = Quaternion.Euler(rot);
        // base.Move() ������������ �������� ����, ����� ��� Y
        base.Move(); // d
                     // print( bndCheck.isOnScreen );
    }
}
