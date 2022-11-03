using HTC.UnityPlugin.Vive;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSyncRequest : NetworkBehaviour
{
    //[SyncVar]
    //public bool aPressed;
    //public bool bPressed;
    public Player Player;
    MyNetworkManager networkManager;
    void Awake()
    {
        networkManager = GameObject.FindObjectOfType<MyNetworkManager>();
        if (!isLocalPlayer)
            return;
        if(isServer)
        {
            networkManager.SetHost(this.gameObject, true);
        }
    }
    // Update is called once per frame
    bool didSend = false;
    void Update()
    {
        if (!isLocalPlayer)
            return;
        bool isLeftDown = ViveInput.GetPressEx(HandRole.LeftHand, ControllerButton.AKey);
        bool isRightDown = ViveInput.GetPressEx(HandRole.RightHand, ControllerButton.AKey);
  
        if(!isLeftDown || !isRightDown)
		{
            didSend = false;
            if (isLocalPlayer && !isServer)
            {
            }
			else if(networkManager.IsHostRequesting)
			{
				//If we are running the server in the editor, I leave it in pairing mode! MAkes it easier to test
				if (!Application.isEditor)
					networkManager.SetHost(this.gameObject, false);
			}
            return;
        }
           
        if (isLeftDown && isRightDown && !didSend)
        {
            didSend = true;
            Debug.Log("Both buttons pressed");


			var players = FindObjectsOfType<VRPlayer>();
			var data = "";
//			foreach (VRPlayer player in players)
//			{
//				var direction = player.GetComponent<PlayerDirection>();
//				data += $@"
//Pos: {player.transform.position}
//Rot:{player.transform.rotation.eulerAngles}
//Head Pos:{direction.Head.position}
//Head Rot:{direction.Head.rotation.eulerAngles}
//RHand Pos:{direction.RightHand.position}
//RHand Rot:{direction.RightHand.rotation.eulerAngles}
//LHand Pos:{direction.LeftHand.position}
//LHand Rot:{direction.LeftHand.rotation.eulerAngles}
//";
//			}
//			Player.CurrentLocalPlayer.DebugData = data;


			if (isLocalPlayer && !isServer)
            {
                Debug.Log("Sending sync request to server");
                CmdSyncRequestState(gameObject);
            }
			else
            {
                Debug.Log("Setting is Host, ready to sync");
                networkManager.SetHost(this.gameObject, true);
            }
        }
    }


    [Command(requiresAuthority = false)]
    public void CmdSyncRequestState(GameObject sender)
    {
        networkManager.RequestSync(sender);
    }

	public override void OnStartLocalPlayer()
	{
		base.OnStartLocalPlayer();
	}


}
