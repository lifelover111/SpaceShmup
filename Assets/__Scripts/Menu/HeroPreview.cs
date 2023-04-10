using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPreview : MonoBehaviour
{
    private float xRot;
    private float yRot;
    private float zRot;
    Vector3 rot;
    public bool loadingCalled = false;
    // Start is called before the first frame update
    void Start()
    {
        xRot = this.transform.rotation.eulerAngles.x;
        yRot = this.transform.rotation.eulerAngles.y;
        zRot = this.transform.rotation.eulerAngles.z;
        rot = new Vector3(xRot, yRot, zRot);
    }

    // Update is called once per frame
    void Update()
    {
        float p = Time.time/10;
        if(!loadingCalled)
            transform.rotation = Quaternion.Euler(5*Mathf.Sin(2 * Mathf.PI * p), 5*Mathf.Cos(2 * Mathf.PI * p), transform.rotation.z) * Quaternion.Euler(rot);
    }
}
