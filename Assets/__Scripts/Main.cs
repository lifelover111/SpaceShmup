using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    static public Main S; // ������-�������� Main
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Set in Inspector")]
    public GameObject[] prefabEnemies;
    public float enemySpawnPerSecond = 0.5f;
    public float enemyDefaultPadding = 1.5f;
    public WeaponDefinition[] weaponDefinitions;
    public bool surviveMode = true;
    

    public GameObject prefabPowerUp;
    public WeaponType[] powerUpFrequency = new WeaponType[] { WeaponType.blaster, WeaponType.blaster, WeaponType.spread, WeaponType.shield };

    private BoundsCheck bndCheck;

    public void ShipDestroyed(Enemy e)
    { // c
      // ������������� ����� � �������� ������������
        if (Random.value <= e.powerUpDropChance)
        { // d
          // ������� ��� ������
          // ������� ���� �� ��������� � powerUpFrequency
            int ndx = Random.Range(0, powerUpFrequency.Length); // e
            WeaponType puType = powerUpFrequency[ndx];
            // ������� ��������� PowerUp
            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            // ���������� ��������������� ��� WeaponType
            pu.SetType(puType); // f
            pu.transform.position = new Vector3(e.transform.position.x, e.transform.position.y, Hero.S.transform.position.z);
        }
    }

        void Awake()
    {
        S = this;
        // �������� � bndCheck ������ �� ��������� BoundsCheck ����� ��������
        // �������
        bndCheck = GetComponent<BoundsCheck>();
        // �������� SpawnEnemy() ���� ��� (� 2 ������� ��� ��������� �� ���������)
        if (surviveMode)
            Invoke("SpawnEnemy", 1f / enemySpawnPerSecond); //a

        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach(WeaponDefinition def in weaponDefinitions)
        {
            WEAP_DICT[def.type] = def;
        }
    }
    public void SpawnEnemy()
    {
        // ������� ��������� ������ Enemy ��� ��������
        int ndx = Random.Range(0, prefabEnemies.Length); // b
        //int ndx = ChooseEnemy();

        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]); // c
                                                                     // ���������� ��������� ������� ��� ������� � ��������� ������� �
        float enemyPadding = enemyDefaultPadding; // d
        if (go.GetComponent<BoundsCheck>() != null)
        { // e
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }
        // ���������� ��������� ���������� ���������� ���������� ������� // f
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;
        // ����� ������� SpawnEnemyO
        Invoke("SpawnEnemy", 1f/enemySpawnPerSecond ); // g
    }

    public void DelayedRestart(float delay)
    {
        // ������� ����� RestartQ ����� delay ������
        Invoke("Restart", delay);
    }
    public void Restart()
    {
        // ������������� _Scene_0, ����� ������������� ����
        SceneManager.LoadScene("_Scene_0");
    }

    /// <summary>  ����������� �������, ������������ WeaponDefinition �� ������������
    /// ����������� ���� WEAP_DICT ������ Main.
    /// </summary>
    //  <returns��������� WeaponDefinition ���, ���� ��� ������ �����������
    /// ��� ���������� WeaponType, ���������� ����� ��������� WeaponDefinition
    /// � ����� none.</returns>
    /// <param name = "wt" > Tnn ������ WeaponType, ��� �������� ��������� ��������
    /// WeaponDefinition</param>
    static public WeaponDefinition GetWeaponDefinition(WeaponType wt)
    { //a
      // ��������� ������� ���������� ����� � �������
      // ������� ������� �������� �� �������������� ����� ������� ������,
      // ������� ��������� ���������� ������ ������ ����.
        if (WEAP_DICT.ContainsKey(wt))
        { // b
            return (WEAP_DICT[wt]);
        }
        // ��������� ���������� ���������� ����� ��������� WeaponDefinition
        // � ����� ������ WeaponType.����, ��� �������� ��������� �������
        // ����� ��������� ����������� WeaponDefinition
        return (new WeaponDefinition()); //�
    }

    int ChooseEnemy() 
    {
        int ri = Random.Range(0, 10);
        if (ri < 6)
            return 0;
        else if (ri < 9)
            return 1;
        return 2;
    }

}

