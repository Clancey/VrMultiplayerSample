using Mirror;
using Mirror.Discovery;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RoomMangerGui : MonoBehaviour
{

    Text readyText;
    MyNetworkManager networkManager;

    [Tooltip("Ready UI Button")]
    public GameObject readyButtonGameObject;


    [Tooltip("Exit Game button")]
    public GameObject exitGameButton;
    [Tooltip("Start Button")]
    public GameObject startButton;

    [Tooltip("Status Text")]
    public Text statusText;
    void Awake()
    {
        networkManager = GameObject.FindObjectOfType<MyNetworkManager>();
        if (networkManager != null)
        {
            networkManager.PlayersReady = (ready) =>
            {
                if (NetworkServer.active && NetworkClient.isConnected)
                {
                    startButton.SetActive(ready);
                }
            };
        }
        readyText = readyButtonGameObject.GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if(networkManager == null) return;

        statusText.text = string.Join(Environment.NewLine, networkManager.roomSlots.Select(x => $"{x.name} - IsReady: {x.readyToBegin}"));
		//statusText.text = player.DebugData;
	}

    public void ToggleReady()
	{
        var networkPlayer = Player.CurrentLocalPlayer.GetComponentInChildren<MyNetworkRoomPlayer>();
        var isReady = !networkPlayer.readyToBegin;
        networkPlayer.CmdChangeReadyState(isReady);
        readyText.text = isReady ? "Not Ready!" : "Ready Up!";
		if (networkPlayer.isServer)
		{
            startButton.SetActive(true);
		}

	}
    public void StartGame()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            networkManager.TryStart();
        }
       
    }

    public void ExitGame()
    {

        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        // stop client if client-only
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }

        FindObjectOfType<NetworkDiscovery>()?.StopDiscovery();

    }
	private void OnDestroy()
	{
		if(networkManager!= null)
		{
            networkManager.PlayersReady = null;
        }
	}
}
