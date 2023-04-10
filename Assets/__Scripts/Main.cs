using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    static public Main S; // Объект-одиночка Main
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
      // Сгенерировать бонус с заданной вероятностью
        if (Random.value <= e.powerUpDropChance)
        { // d
          // Выбрать тип бонуса
          // Выбрать один из элементов в powerUpFrequency
            int ndx = Random.Range(0, powerUpFrequency.Length); // e
            WeaponType puType = powerUpFrequency[ndx];
            // Создать экземпляр PowerUp
            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            // Установить соответствующий тип WeaponType
            pu.SetType(puType); // f
            pu.transform.position = new Vector3(e.transform.position.x, e.transform.position.y, Hero.S.transform.position.z);
        }
    }

        void Awake()
    {
        S = this;
        // Записать в bndCheck ссылку на компонент BoundsCheck этого игрового
        // объекта
        bndCheck = GetComponent<BoundsCheck>();
        // Вызывать SpawnEnemy() один раз (в 2 секунды при значениях по умолчанию)
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
        // Выбрать случайный шаблон Enemy для создания
        int ndx = Random.Range(0, prefabEnemies.Length); // b
        //int ndx = ChooseEnemy();

        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]); // c
                                                                     // Разместить вражеский корабль над экраном в случайной позиции х
        float enemyPadding = enemyDefaultPadding; // d
        if (go.GetComponent<BoundsCheck>() != null)
        { // e
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }
        // Установить начальные координаты созданного вражеского корабля // f
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;
        // Снова вызвать SpawnEnemyO
        Invoke("SpawnEnemy", 1f/enemySpawnPerSecond ); // g
    }

    public void DelayedRestart(float delay)
    {
        // Вызвать метод RestartQ через delay секунд
        Invoke("Restart", delay);
    }
    public void Restart()
    {
        // Перезагрузить _Scene_0, чтобы перезапустить игру
        SceneManager.LoadScene("_Scene_0");
    }

    /// <summary>  Статическая функция, возвращающая WeaponDefinition из статического
    /// защищенного поля WEAP_DICT класса Main.
    /// </summary>
    //  <returnsЭкземпляр WeaponDefinition или, если нет такого определения
    /// для указанного WeaponType, возвращает новый экземпляр WeaponDefinition
    /// с типом none.</returns>
    /// <param name = "wt" > Tnn оружия WeaponType, для которого требуется получить
    /// WeaponDefinition</param>
    static public WeaponDefinition GetWeaponDefinition(WeaponType wt)
    { //a
      // Проверить наличие указанного ключа в словаре
      // Попытка извлечь значение по отсутствующему ключу вызовет ошибку,
      // поэтому следующая инструкция играет важную роль.
        if (WEAP_DICT.ContainsKey(wt))
        { // b
            return (WEAP_DICT[wt]);
        }
        // Следующая инструкция возвращает новый экземпляр WeaponDefinition
        // с типом оружия WeaponType.попе, что означает неудачную попытку
        // найти требуемое определение WeaponDefinition
        return (new WeaponDefinition()); //с
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

