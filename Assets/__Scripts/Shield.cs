using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [Header("Set in Inspector")]
    public float rotationsPerSecond = 0.1f;
    [Header("Set Dynamically")]
    public int levelShown = 0;
    // ������� ����������, �� ������������ � ����������
    Material mat; // �hh
    void Awake()
    {
        mat = GetComponent<Renderer>().material; // b
        mat.mainTextureOffset = new Vector2(0, 0);
    }
    void Update()
    {
        // ��������� ������� �������� ��������� ���� �� �������-�������� ����
        int currLevel = Mathf.FloorToInt(Hero.S.shieldLevel); // c
                                                              // ���� ��� ���������� �� levelShown...
        if (levelShown != currLevel)
        {
            levelShown = currLevel;
            // ��������������� �������� � ��������, ����� ���������� ���� � ������
            // ���������
            mat.mainTextureOffset = new Vector2(0.2f*levelShown, 0); // d
        }
        // ������������ ���� � ������ ����� � ���������� ���������
        float rZ = -(rotationsPerSecond * Time.time * 360) % 360f; // e
        transform.rotation = Quaternion.Euler(0, 0, rZ);
    }
}
