using Mirror;
using Mirror.Examples.NetworkRoom;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNetworkManager : NetworkRoomManagerExt
{

    public GameObject RemoteVRPawn;
    public GameObject NonVrPawn;

    //public override void OnServerAddPlayer (NetworkConnection conn)
    //{
    //    var player = Instantiate(playerPrefab);
    //    var playerScript = player.GetComponent<Player>();
    //    playerScript.LocalVRPawn = LocalVRPawn;
    //    //playerScript.RemoteVRPawn = RemoteVRPawn;
    //    //playerScript.NonVrPawn = NonVrPawn;
    //    NetworkServer.AddPlayerForConnection(conn, player);
    //}
    string currentScene;
    public override void OnRoomServerSceneChanged(string sceneName)
    {
        currentScene = sceneName;
        // spawn the initial batch of Rewards
        if (sceneName == GameplayScene)
        {
            IsInPairingMode = false;
            base.OnRoomServerSceneChanged(sceneName);
        }
        else if(sceneName == RoomScene)
		{
            IsInPairingMode = true;
		}
    }

    GameObject hostGameObject;
    public void SetHost(GameObject gameObject, bool isRequesting)
	{
        hostGameObject = gameObject;
        hostIsRequesting = isRequesting;

        Debug.Log($"Host is set to sync location :{isRequesting}");

    }
    bool hostIsRequesting;
    Dictionary<uint, (Vector3 location, Quaternion rotation)> CurrentLocationOffset = new Dictionary<uint, (Vector3 location, Quaternion rotation)>();

    public bool IsHostRequesting => hostIsRequesting;
    public bool IsInPairingMode { get; set; }
    public void RequestSync(GameObject sender)
	{
        if (!IsInPairingMode)
            return;
        try
        {
            Debug.Log($"Sync request recieved: host{hostGameObject != null} - {hostIsRequesting}");
            if (hostGameObject != null && hostIsRequesting)
            {
                var playerVr = sender.GetComponentInChildren<VRPlayer>();
                playerVr.Teleport(hostGameObject);
            }
        }
        catch(Exception e)
		{
            Console.WriteLine(e);
		}
	}
    public void TryStart()
	{
       
        if (allPlayersReady)
        {
            ServerChangeScene(GameplayScene);
        }
    }
    public Action<bool> PlayersReady { get; set; }
	public override void OnRoomServerPlayersReady()
	{
		base.OnRoomServerPlayersReady();
        PlayersReady?.Invoke(true);

    }

    public override void OnRoomServerAddPlayer(NetworkConnectionToClient conn)
    {
        var shouldVr = ((VRAuthenticator.AuthRequestMessage)conn.authenticationData).IsVr;
        var player = Instantiate(shouldVr ? RemoteVRPawn : NonVrPawn, Vector3.zero, Quaternion.identity);
        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    //public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    //{
    //    if (currentScene == GameplayScene)
    //    {
    //        var shouldVr = ((VRAuthenticator.AuthRequestMessage)conn.authenticationData).IsVr;
    //        var player = Instantiate(shouldVr ? RemoteVRPawn : NonVrPawn, Vector3.zero, Quaternion.identity);
    //        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
    //        NetworkServer.AddPlayerForConnection(conn, player);
    //    }
    //    else if (currentScene == RoomScene)
    //    {
    //        var player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
    //        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
    //        NetworkServer.AddPlayerForConnection(conn, player);
    //    }
    //    else
    //    {
    //        //base.OnServerAddPlayer(conn);
    //    }
    //}
    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
    {
        //We can check the room player, to get data about what kind of pawn it should be!
        //Use this to custumize the look/feel
        var shouldVr = ((VRAuthenticator.AuthRequestMessage)conn.authenticationData).IsVr;
        var player = Instantiate(shouldVr ? RemoteVRPawn : NonVrPawn, Vector3.zero, Quaternion.identity);
        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        return player;
    }
    public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnectionToClient conn)
    {
        return base.OnRoomServerCreateRoomPlayer(conn);
    }
    public override void OnRoomServerPlayersNotReady()
	{
		base.OnRoomServerPlayersNotReady();
        PlayersReady?.Invoke(false);
    }

    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
    {
		PlayerScore playerScore = gamePlayer.GetComponent<PlayerScore>();
		var networkRoomPlayer = gamePlayer.GetComponent<MyNetworkRoomPlayer>();

		var oldScore = roomPlayer.GetComponent<PlayerScore>();
		var oldRoomPlayer = roomPlayer.GetComponent<MyNetworkRoomPlayer>();
        var player = gamePlayer.GetComponent<Player>();
        var isVr = conn.authenticationData is VRAuthenticator.AuthRequestMessage authMessage ? authMessage.IsVr : Player.IsLocalVR;
        player.IsVr = isVr;

        playerScore.index = oldScore.index;
		playerScore.score = oldScore.score;
		networkRoomPlayer.SyncedPosition = gamePlayer.transform.position = oldRoomPlayer.SyncedPosition;
		networkRoomPlayer.SyncedRotation = gamePlayer.transform.rotation = oldRoomPlayer.SyncedRotation;
		NetworkServer.Destroy(roomPlayer);
		Destroy(roomPlayer);
		return true;
    }

    public override void OnStartClient()
    {
        if (roomPlayerPrefab == null || roomPlayerPrefab.gameObject == null)
            Debug.LogError("NetworkRoomManager no RoomPlayer prefab is registered. Please add a RoomPlayer prefab.");
        else
            NetworkClient.RegisterPrefab(roomPlayerPrefab.gameObject);
        

        if (RemoteVRPawn == null)
            Debug.LogError("NetworkRoomManager no RemoteVRPawn prefab is registered. Please add a RemoteVRPawn prefab.");
        else
            NetworkClient.RegisterPrefab(RemoteVRPawn.gameObject);

        if (NonVrPawn == null)
            Debug.LogError("NetworkRoomManager no NonVrPawn prefab is registered. Please add a NonVrPawn prefab.");
        else
            NetworkClient.RegisterPrefab(NonVrPawn.gameObject);

        OnRoomStartClient();
    }

    //public override GameObject OnRoomServerCreateGamePlayer(NetworkConnection conn, GameObject roomPlayer)
    //{

    //       //NetworkServer.Destroy(oldPlayerPawn);
    //       //Destroy(oldPlayerPawn);
    //       var oldPlayer = roomPlayer.GetComponent<Player>();
    //       var prefab = oldPlayer.IsVr ? RemoteVRPawn : NonVrPawn;

    //       var gamePlayer = Instantiate(prefab, roomPlayer.transform.position, roomPlayer.transform.rotation);

    //       PlayerScore playerScore = gamePlayer.GetComponent<PlayerScore>();
    //	var networkRoomPlayer = gamePlayer.GetComponent<MyNetworkRoomPlayer>();

    //	var oldScore = roomPlayer.GetComponent<PlayerScore>();
    //	var oldRoomPlayer = roomPlayer.GetComponent<MyNetworkRoomPlayer>();

    //	playerScore.index = oldScore.index;
    //	playerScore.score = oldScore.score;
    //	networkRoomPlayer.SyncedPosition = gamePlayer.transform.position = oldRoomPlayer.SyncedPosition;
    //	networkRoomPlayer.SyncedRotation = gamePlayer.transform.rotation = oldRoomPlayer.SyncedRotation;

    //	NetworkServer.Destroy(roomPlayer);
    //	Destroy(roomPlayer);
    //	return gamePlayer;
    //	//return base.OnRoomServerCreateGamePlayer(conn, roomPlayer);
    //}
    //   public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnection conn)
    //   {
    //       var isVr = conn.authenticationData is VRAuthenticator.AuthRequestMessage authMessage ? authMessage.IsVr : Player.IsLocalVR;
    //       var prefab = isVr ? RemoteVRPawn : NonVrPawn;
    //       var gamePlayer = Instantiate(prefab, Vector3.zero, Quaternion.identity);
    //       return gamePlayer;
    //   }
}
