using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDiscoveryManager : MonoBehaviour
{
    // Start is called before the first frame update
    MyNetworkManager networkManager;
    void Awake()
    {
        networkManager = GameObject.FindObjectOfType<MyNetworkManager>();
        networkManager.IsInPairingMode = true;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
