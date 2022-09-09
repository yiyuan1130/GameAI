using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VOOb : MonoBehaviour
{
    public Vector3 speed;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Time.deltaTime * speed;
    }
}
