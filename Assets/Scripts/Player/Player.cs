#define VR
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;

public class Player : NetworkBehaviour
{
    public GameObject LocalVRPawn;
    public GameObject LocalPlayer;

    public static bool IsLocalVR => VRLocalPlayerSpawner.ShouldUseVr;
    [SyncVar]
    public bool IsVr;
    public string DebugData;
    public static Player CurrentLocalPlayer { get; protected set; }

    // Start is called before the first frame update
    void Start()
    {
        if(isLocalPlayer)
            CurrentLocalPlayer = this;
    }


	private void Awake()
	{
        if (this.GetType().Name != nameof(Player))
        {
            return;
        }
        if (isLocalPlayer && shouldBeVR())
        {

            IsVr = true;
        }

        if (IsVr)
        {
            RespawnVrPawn();
        }
        else
            RespawnStandardPawn();
    }

	void RespawnStandardPawn ()
    {
        var go = Instantiate<GameObject>(LocalPlayer, transform);
        var player = go.GetComponent<Player>();
        player.IsVr = IsVr;
        player.LocalPlayer = LocalPlayer;
        player.LocalVRPawn = LocalVRPawn;
        Destroy(this);
    }

    protected void RespawnVrPawn ()
    {
        var go = Instantiate<GameObject>(LocalVRPawn, transform);
        var player = go.GetComponent<Player>();
        player.IsVr = IsVr;
        player.LocalPlayer = LocalPlayer;
        player.LocalVRPawn = LocalVRPawn;
        Destroy(this);
    }


    bool shouldBeVR()
    {
        return !VRLocalPlayerSpawner.ShouldUseVr && XRSettings.enabled && !string.IsNullOrWhiteSpace(XRSettings.loadedDeviceName);
    }


}
