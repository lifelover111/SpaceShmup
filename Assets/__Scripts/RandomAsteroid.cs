using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAsteroid : MonoBehaviour
{
    public GameObject[] asteroids;
    private GameObject asteroid;
    // Start is called before the first frame update
    void Awake()
    {
        int p = Random.Range(0, asteroids.Length);
        asteroid = Instantiate(asteroids[p], new Vector3(transform.position.x,transform.position.y,transform.position.z + 1), Quaternion.identity);
        Destroy(this.gameObject);
    }
}
