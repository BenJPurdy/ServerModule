using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkGameObject : MonoBehaviour
{
    public bool isLocal;
    public uint networkID = 0;
    uint localID = 0;
    static uint currentLocalID = 0;

    // Start is called before the first frame update
    void Start()
    {
       

    }

    private void Awake()
    {
        localID = ++currentLocalID;
        NetworkManager.networkObjects.Add(this.gameObject);
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
