using Mirror;
using Mirror.Discovery;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(NetworkManager))]
[RequireComponent(typeof(NetworkDiscovery))]
public class NetworkHud : MonoBehaviour
{
    NetworkManager manager;
    public VStackItemSource ServerSource;
    readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();

    public NetworkDiscovery networkDiscovery;

    void Awake()
    {
        manager = FindObjectOfType<MyNetworkManager>();
    }

    void OnValidate()
    {
        if (networkDiscovery == null)
        {
            networkDiscovery = GetComponent<NetworkDiscovery>();
//#if !UNITY_ANDROID 
//            UnityEditor.Events.UnityEventTools.AddPersistentListener(networkDiscovery.OnServerFound, OnDiscoveredServer);
//            UnityEditor.Undo.RecordObjects(new Object[] { this, networkDiscovery }, "Set NetworkDiscovery");
//#endif
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartServer()
    {
        manager.StartHost();

        networkDiscovery.StopDiscovery();
        networkDiscovery.AdvertiseServer();
    }

    void Connect(ServerResponse info)
    {
        NetworkManager.singleton.StartClient(info.uri);
    }

    public void Search()
	{
        discoveredServers.Clear();
        ServerSource?.SetItems(discoveredServers);
        networkDiscovery.StartDiscovery();
    }

    public void OnDiscoveredServer(ServerResponse info)
    {
        // Note that you can check the versioning to decide if you can connect to the server or not using this method
        discoveredServers[info.serverId] = info;
        Connect(info);
        ServerSource?.SetItems(discoveredServers);
    }
}
