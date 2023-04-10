using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public static float size = 20f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, size * transform.localScale, 0.001f);
        if (transform.localScale.x >= size)
            Destroy(gameObject);
    }
    void Explode()
    {
        
    }
}
