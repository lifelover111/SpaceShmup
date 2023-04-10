using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [Header("Set in Inspector")]
    public float scrollSpeed = 10f;
    public static LevelController controller;
    public WeaponType startBonusType = WeaponType.none;
    [Header("Set Dynamically")]
    private GameObject[] enemies;
    private Main main;
    private BoundsCheck bndCheck;
    // Start is called before the first frame update
    void Awake()
    {
        controller = this;
        bndCheck = GetComponent<BoundsCheck>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            enemy.SetActive(false);
        }
    }
    void Start()
    {
        main = Main.S;
        CreateStartBonus(startBonusType);
    }

    // Update is called once per frame
    void Update()
    {
        int i = 0;
        foreach (var enemy in enemies)
        {
            if (enemy.transform.position.y + enemy.GetComponent<Enemy>().bndRadius <= bndCheck.camHeight)
            {
                enemy.SetActive(true);
                DeleteEnemy(i);
                i--;
            }
            enemy.transform.position -= new Vector3(0, Time.deltaTime * scrollSpeed, 0);
            i++;
        }
    }

    public void DeleteEnemy(int index)
    {
        GameObject[] temp = new GameObject[enemies.Length - 1];
        System.Array.Copy(enemies, 0, temp, 0, index);
        System.Array.Copy(enemies, index + 1, temp, index, enemies.Length - index - 1);
        enemies = temp;
    }

    public void AddEnemy(GameObject enemy)
    {
        GameObject[] temp = new GameObject[enemies.Length + 1];
        temp[temp.Length] = enemy;
        enemies = temp;
    }

    void CreateStartBonus(WeaponType type = WeaponType.none)
    {
        if (type == WeaponType.none)
        {
            int p = Random.Range(0, main.powerUpFrequency.Length);
            WeaponType puType = main.powerUpFrequency[p];
            GameObject bonus = Instantiate(main.prefabPowerUp) as GameObject;
            PowerUp pu = bonus.GetComponent<PowerUp>();
            pu.SetType(puType);
            bonus.transform.position = new Vector3(0, bndCheck.camHeight-1, 0);
            bonus.GetComponent<Rigidbody>().velocity = new Vector3(0, -5, 0);
        }
        else 
        {
            GameObject bonus = Instantiate(main.prefabPowerUp) as GameObject;
            PowerUp pu = bonus.GetComponent<PowerUp>();
            pu.SetType(type);
            bonus.transform.position = new Vector3(0, bndCheck.camHeight-1, 0);
            bonus.GetComponent<Rigidbody>().velocity = new Vector3(0, -5, 0);
        }
    }
}
