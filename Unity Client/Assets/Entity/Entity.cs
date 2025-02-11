using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    uint uID;
    static uint localID = 0;
    // Start is called before the first frame update
    void Start()
    {
        uID = ++localID;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
