using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkGameObject : MonoBehaviour
{
    public bool isLocal;
    public uint networkID = 0;
    public uint localID = 0;
    static uint currentLocalID = 0;
    [SerializeField]
    static NetworkManager networkManager = null;

    // Start is called before the first frame update
    void Start()
    {


    }

    private void Awake()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        if (networkManager == null)
        {
            Debug.Log("Failed to capture network manager");
            return;
        }
        localID = ++currentLocalID;
        NetworkManager.networkObjects.Add(this.gameObject);


    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (isLocal)
        {
            TransformPacket t = new TransformPacket(networkID, transform);
            networkManager.SendPacket(t);
        }
    }
}
