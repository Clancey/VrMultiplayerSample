using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNetworkRoomPlayer : NetworkRoomPlayer
{

    [SyncVar(hook = nameof(HasSyncedChanged))]
    public bool HasSynced;

    [SyncVar]
    public Vector3 SyncedPosition;

    [SyncVar]
    public Quaternion SyncedRotation;

    [SyncVar]
    public bool IsVr;
    public override void OnStartClient()
    {
        // Debug.LogFormat(LogType.Log, "OnStartClient {0}", SceneManager.GetActiveScene().path);

        base.OnStartClient();
    }

    public override void OnClientEnterRoom()
    {
        // Debug.LogFormat(LogType.Log, "OnClientEnterRoom {0}", SceneManager.GetActiveScene().path);
    }

    public override void OnClientExitRoom()
    {
        // Debug.LogFormat(LogType.Log, "OnClientExitRoom {0}", SceneManager.GetActiveScene().path);
    }

    public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
    {
        // Debug.LogFormat(LogType.Log, "ReadyStateChanged {0}", newReadyState);
    }

    public virtual void HasSyncedChanged(bool oldReadyState, bool newReadyState)
    {
        // Debug.LogFormat(LogType.Log, "ReadyStateChanged {0}", newReadyState);
    }

    [Command]
    public void CmdHasSyncedState(bool readyState)
    {
        HasSynced = readyState;
        var room = NetworkManager.singleton as MyNetworkManager;
        if (room != null)
        {
            room.ReadyStatusChanged();
        }
    }
}