using Mirror;
using UnityEngine;

[RequireComponent(typeof(VRPlayer))]
public class NetworkPlayerTransformHandler : NetworkBehaviour
{
    [SerializeField]
    private int networkUpdatesPerSecond = 30;

    private VRPlayer networkPlayer;

    private PlayerTransformData mostRecentPlayerTransformData;

    private float networkSendTimer;

    private void Awake ()
    {
        networkPlayer = GetComponent<VRPlayer>();
    }

    [Command]
    private void CmdSendNetworkPlayerTransformDataToServer (byte[] data)
    {
        mostRecentPlayerTransformData = networkPlayerTransformWriter.FromBytes(data);
        RpcSendNetworkPlayerTransformDataToClients(data);
    }

    [ClientRpc]
    private void RpcSendNetworkPlayerTransformDataToClients (byte[] data)
    {
        if (isLocalPlayer)
            return;

        mostRecentPlayerTransformData = networkPlayerTransformWriter.FromBytes(data);
        networkPlayer.SetTransformsToNetworkValues(mostRecentPlayerTransformData);
    }

    [Client]
    private void Update ()
    {
        if (ShouldSendPlayerTransformDataToServer())
        {
            networkSendTimer = 1f / (float)networkUpdatesPerSecond;

            var playerTransformDataToSendServer = networkPlayer.GetLocalPlayerTransformData();

            var data = networkPlayerTransformWriter.GetBytes(playerTransformDataToSendServer);
            if(NetworkClient.ready)
                CmdSendNetworkPlayerTransformDataToServer(data);
        }
    }

    private NetworkPlayerTransformWriter networkPlayerTransformWriter = new NetworkPlayerTransformWriter();

    [Client]
    private bool ShouldSendPlayerTransformDataToServer ()
    {
        if (!isLocalPlayer || !isActiveAndEnabled || !NetworkClient.ready)
            return false;

        networkSendTimer -= Time.deltaTime;
        return networkSendTimer <= 0f;
    }
}