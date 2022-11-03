using HTC.UnityPlugin.Vive;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VRPlayer : Player
{

    public static Vector3 VROffSet;
    public static Quaternion VRRotation;
    public GameObject LeftHandDefaultObject;
    public GameObject RightHandDefaultObject;

    [SerializeField]
    private PlayerHand networkLeftHand;
    [SerializeField]
    private PlayerHand networkRightHand;
    [SerializeField]
    private PlayerHead networkHead;

    private GameObject networkLeftHandEquipedItem;
    private GameObject networkRightHandEquipedItem;


    Vector3 leftHandItemPosition;
    Vector3 rightHandItemPosition;
    Quaternion leftHandItemRotation;
    Quaternion rightHandItemRotation;

    
    [SyncVar]
    uint leftHandEquipedObjectNetId;
    uint currentLeftHandEquipedNetId;

    [SyncVar]
    public uint rightHandEquipedObjectNetId;
    uint currentRightHandEquipedNetId;

	void leftHandNetIdChanged (uint id) => UpdateHandEquiped(id, true);
    void rightHandNetIdChanged (uint id) => UpdateHandEquiped(id, false);

    void UpdateHandEquiped(uint id, bool isLeftHand)
    {
        var current = isLeftHand ? currentLeftHandEquipedNetId : currentRightHandEquipedNetId;
        var currentObject = isLeftHand ? networkLeftHandEquipedItem : networkRightHandEquipedItem;
        if (id == current)
            return;
        var equipedObject = NetworkIdentity.spawned[id].gameObject;
        if (equipedObject == currentObject)
            return;
        var interactable = equipedObject.GetComponent<IVRHandInteractable>();

        if (isLeftHand) {
            currentLeftHandEquipedNetId = id;
            networkLeftHandEquipedItem = equipedObject;
            _leftHandObject = interactable;
        }
        else {
            currentRightHandEquipedNetId = id;
            networkRightHandEquipedItem = equipedObject;
            _rightHandObject = interactable;
        }
        if(isLocalPlayer)
        {

            //Debug.LogError("Local pawn is setting!");
            var parent = isLeftHand ? leftHand : rightHand;
            equipedObject.transform.parent = parent.transform;
            equipedObject.transform.localPosition = isLeftHand ? leftHandItemPosition : rightHandItemPosition;
            equipedObject.transform.localRotation = isLeftHand ? leftHandItemRotation : rightHandItemRotation;
            CmdSetAuthority(equipedObject.GetComponent<NetworkIdentity>(), this.GetComponent<NetworkIdentity>());

        }
        //var parent = isLeftHand ? networkLeftHand.transform : networkRightHand.transform;

        //var localPosition = equipedObject.transform.localPosition;
        //equipedObject.transform.parent = parent;
        //equipedObject.transform.localPosition = localPosition;
    }

    PlayerDirection playerDirection;
    // Start is called before the first frame update
    void Start()
    {
        IsVr = true;
        playerDirection = this.GetComponent<PlayerDirection>();
       // renderer = GetComponent<Renderer>();
    }
    //public override void OnStartClient ()
    //{
    //    if (leftHandEquipedObjectNetId > 0)
    //        UpdateHandEquiped(leftHandEquipedObjectNetId, true);
    //    if(rightHandEquipedObjectNetId > 0)
    //        UpdateHandEquiped(leftHandEquipedObjectNetId, false);
    //    base.OnStartClient();
    //}

    Renderer renderer;

    GameObject localVrPawn;

    MyNetworkRoomPlayer networkPlayer;
    public override void OnStartLocalPlayer ()
    {
        CurrentLocalPlayer = this;
		//we need to hunt down and kill any othe random clients that are not authority
		{
            var oldClients = FindObjectsOfType<VRPlayer>().Where(x => x.isClient && !x.hasAuthority).ToList();
            foreach(var oc in oldClients)
			{
                if(oc != this && oc.isLocalPlayer)
                    Destroy(oc.gameObject);
			}
		}
        networkPlayer = this.GetComponentInChildren<MyNetworkRoomPlayer>();
        foreach (var renderer in this.GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }

        if(localVrPawn == null)
            localVrPawn = FindObjectOfType<LocalVRPlayer>()?.gameObject;
        if (!localVrPawn)
        {
            
            localVrPawn = Instantiate(LocalVRPawn);
           // DontDestroyOnLoad(localVrPawn);

        }
        localVrPawn.transform.position = this.transform.position;
        LocalVRPawn.transform.rotation = this.transform.rotation;
        ConnectVRToLocal();
    }
    VivePoseTracker leftHand;
    VivePoseTracker rightHand;
    void ConnectVRToLocal()
    {
        var hands = localVrPawn.GetComponentsInChildren<VivePoseTracker>();
        var leftProp = ViveRoleProperty.New(HandRole.LeftHand);
        var rightProp = ViveRoleProperty.New(HandRole.RightHand);
        leftHand = hands.FirstOrDefault(x => x.viveRole == leftProp);
        rightHand = hands.FirstOrDefault(x => x.viveRole == rightProp);
        if (leftHand)
        {
            networkLeftHand.SetHand(leftHand);
            if (LeftHandDefaultObject)
            {
                leftHandItemPosition = LeftHandDefaultObject?.transform?.position ?? Vector3.zero;
                leftHandItemRotation = LeftHandDefaultObject?.transform?.rotation ?? Quaternion.identity;
            }
            CmdSpawnLeftHandItem();
            ViveInput.AddPressDown(HandRole.LeftHand, ControllerButton.Trigger, LeftTriggerPressed);
            ViveInput.AddPressUp(HandRole.LeftHand, ControllerButton.Trigger, LeftTriggerReleased);
        }
        if (rightHand)
        {
            networkRightHand.SetHand(rightHand);
            if (RightHandDefaultObject)
            {
                rightHandItemPosition = RightHandDefaultObject?.transform?.position ?? Vector3.zero;
                rightHandItemRotation = RightHandDefaultObject?.transform?.rotation ?? Quaternion.identity;
            }
            CmdSpawnRightHandItem();
            ViveInput.AddPressDown(HandRole.RightHand, ControllerButton.Trigger, RightTriggerPressed);
            ViveInput.AddPressUp(HandRole.RightHand, ControllerButton.Trigger, RightTriggerReleased);
        }

        var head = localVrPawn.GetComponentInChildren<VRCameraHook>();
        if (head)
            networkHead.SetTransform(this,head);
    }

    [Command]
    void CmdSpawnLeftHandItem() => SpawnHandItem(true);

    [Command]
    void CmdSpawnRightHandItem () => SpawnHandItem(false);

    private void Update()
    {
        if (localVrPawn != null) {

            this.transform.position = localVrPawn.transform.position = VROffSet;
            this.transform.rotation = localVrPawn.transform.rotation = VRRotation;
        }
    }

    void SpawnHandItem(bool isLeftHand)
    {
        var prefab = isLeftHand ? LeftHandDefaultObject : RightHandDefaultObject;
        if (!prefab)
            return;

        var parent = isLeftHand ? networkLeftHand.gameObject : networkRightHand.gameObject;
        var item = Instantiate<GameObject>(prefab);
       
        var interactable = item.GetComponent<IVRHandInteractable>();
        NetworkServer.Spawn(item, this.connectionToClient);
        var netId = item.GetComponent<NetworkIdentity>().netId;
        //item.transform.parent = parent.transform;
        if (isLeftHand)
        {
            _leftHandObject = interactable;
            leftHandItemPosition = prefab?.transform.position ?? Vector3.zero;
            leftHandItemRotation = prefab?.transform.rotation ?? Quaternion.identity;
            networkLeftHandEquipedItem = item;
            leftHandEquipedObjectNetId = netId;
        }
        else
        {
            _rightHandObject = interactable;
            rightHandItemPosition = prefab?.transform.position ?? Vector3.zero;
            rightHandItemRotation = prefab?.transform.rotation ?? Quaternion.identity;
            networkRightHandEquipedItem = item;
            rightHandEquipedObjectNetId = netId;
        }
        if (isLocalPlayer)
        {

            //Debug.LogError("Local pawn is setting!");
            var localPosition = item.transform.localPosition;
            item.transform.parent = (isLeftHand ? leftHand : rightHand).transform;
            item.transform.localPosition = localPosition;

        }
        //RpcSyncBlockOnce(prefab.transform.localPosition, prefab.transform.localRotation, item, isLeftHand);
        RpcUpdateSpawnedHandObject(netId, isLeftHand);

    }

    [ClientRpc]
    void RpcUpdateSpawnedHandObject (uint id, bool isLeftHand) => UpdateHandEquiped(id, isLeftHand);

    IVRHandInteractable _leftHandObject;
    IVRHandInteractable leftHandObject => _leftHandObject ??( _leftHandObject = GetHandInteractable(leftHand));
    IVRHandInteractable _rightHandObject;
    IVRHandInteractable rightHandObject => _rightHandObject ?? (_rightHandObject = GetHandInteractable(rightHand));
    IVRHandInteractable GetHandInteractable (VivePoseTracker hand) => hand.gameObject.GetComponentInChildren<IVRHandInteractable>();
    void RightTriggerPressed()
    {
        rightHandObject?.TriggerPressed();
    }
    void RightTriggerReleased()
    {
        rightHandObject?.TriggerReleased();

    }
    void LeftTriggerPressed ()
    {

        leftHandObject?.TriggerPressed();
    }
    void LeftTriggerReleased ()
    {

        leftHandObject?.TriggerReleased();
    }
    
    public void Teleport(GameObject host)
	{
        if (!isServer)
            return;
        RpcTeleport(host);
	}

    [ClientRpc]
    public void RpcTeleport(GameObject hostGameObject)
    {
		if (localVrPawn != null)
		{


            var Host = hostGameObject.GetComponentInChildren<PlayerDirection>();
            (var hostRotation, var hostMidpoint) = Host.CalculateRotation();
            Debug.Log($"Host Position: {hostMidpoint} Rotation: {hostRotation.eulerAngles}");
            var targetPos = hostMidpoint + Host.Mid.forward * .1f;
            //(var playerRotation, var playerMidpoint) = Player2Diretion.CalculateRotation();

            ////Flip around to get the rotation we want to match
            var facingHostRotation = hostRotation * Quaternion.Euler(0, 180, 0);
            Debug.Log($"Host Position: {targetPos} Rotation: {facingHostRotation.eulerAngles}");
            if(Host == playerDirection)
			{
                Debug.Log("WHy are they the same?");
			}

            //This sets the playerDirection.Mid as well
            playerDirection.CalculateRotation();

            //Get the player rotation compared to the mid direction
            var targetRot = facingHostRotation * Quaternion.Inverse(playerDirection.Mid.localRotation);
            //We only care about the Y axis

            VRRotation = gameObject.transform.rotation = localVrPawn.transform.rotation = targetRot;

            //Put the player just a little in front of the middle point


            Vector3 GlobalCameraPosition = playerDirection.Mid.position;  //get the global position of VRcamera

            Vector3 GlobalPlayerPosition = gameObject.transform.position;

            Vector3 GlobalOffsetCameraPlayer = new Vector3(GlobalCameraPosition.x - GlobalPlayerPosition.x, GlobalCameraPosition.y - GlobalPlayerPosition.y, GlobalCameraPosition.z - GlobalPlayerPosition.z);

            Vector3 newRigPosition = new Vector3(targetPos.x - GlobalOffsetCameraPlayer.x, targetPos.y - GlobalOffsetCameraPlayer.y, targetPos.z - GlobalOffsetCameraPlayer.z);


            Debug.Log($"Position: {newRigPosition} Rotation: {targetRot.eulerAngles}");
            VROffSet = gameObject.transform.position = localVrPawn.transform.position = newRigPosition;
          

		}
    }


    [Command]
    void CmdSetAuthority (NetworkIdentity grabID, NetworkIdentity playerID)
    {
        grabID.AssignClientAuthority(connectionToClient);
    }

    [Command]
    void CmdRemoveAuthority (NetworkIdentity grabID, NetworkIdentity playerID)
    {
        grabID.RemoveClientAuthority();
    }

    public PlayerTransformData GetLocalPlayerTransformData ()
    {
        return new PlayerTransformData()
        {
            Position = transform.position,
            Rotation = transform.rotation,
            

            HeadPosition = networkHead.LocalPlayerPosition,
            LeftHandPosition = networkLeftHand.LocalPlayerPosition,
            RightHandPosition = networkRightHand.LocalPlayerPosition,

            HeadRotation = networkHead.LocalPlayerRotation,
            LeftHandRotation = networkLeftHand.LocalPlayerRotation,
            RightHandRotation = networkRightHand.LocalPlayerRotation,

            IsLeftHandEquiped = leftHandEquipedObjectNetId,
            LeftHandEquipedPosition = networkLeftHandEquipedItem?.transform.position ?? Vector3.zero,
            LeftHandEquipedRotation = networkLeftHandEquipedItem?.transform.rotation ?? Quaternion.identity,

            IsRightHandEquiped = rightHandEquipedObjectNetId,
            RightHandEquipedPosition = networkRightHandEquipedItem?.transform.position ?? Vector3.zero,
            RightHandEquipedRotation = networkRightHandEquipedItem?.transform.rotation ?? Quaternion.identity,
        };
    }

    public bool ignoreNetworkUpdates;
    public void SetTransformsToNetworkValues (PlayerTransformData playerTransformData)
    {
        if (ignoreNetworkUpdates)
            return;
		transform.position = playerTransformData.Position;
		transform.rotation = playerTransformData.Rotation;

		networkLeftHand.transform.position = playerTransformData.LeftHandPosition;
        networkLeftHand.transform.rotation = playerTransformData.LeftHandRotation;

        networkRightHand.transform.position = playerTransformData.RightHandPosition;
        networkRightHand.transform.rotation = playerTransformData.RightHandRotation;

        networkHead.transform.position = playerTransformData.HeadPosition;
        networkHead.transform.rotation = playerTransformData.HeadRotation;
        //If the local hands haven't been associated. Lets hook them up!
        if(currentLeftHandEquipedNetId  != playerTransformData.IsLeftHandEquiped)
        {
            UpdateHandEquiped(playerTransformData.IsLeftHandEquiped, true);
        }
        if(currentRightHandEquipedNetId != playerTransformData.IsRightHandEquiped)
        {
            UpdateHandEquiped(playerTransformData.IsRightHandEquiped, false);
        }
        if(networkLeftHandEquipedItem)
        {
            networkLeftHandEquipedItem.transform.position = playerTransformData.LeftHandEquipedPosition;
            networkLeftHandEquipedItem.transform.rotation = playerTransformData.LeftHandEquipedRotation;
        }

        if(networkRightHandEquipedItem)
        {
            networkRightHandEquipedItem.transform.position = playerTransformData.RightHandEquipedPosition;
            networkRightHandEquipedItem.transform.rotation = playerTransformData.RightHandEquipedRotation;
        }
    }
    private void OnDestroy ()
    { 
        ViveInput.RemovePressDown(HandRole.LeftHand, ControllerButton.Trigger, RightTriggerPressed);
        ViveInput.RemovePressUp(HandRole.LeftHand, ControllerButton.Trigger, RightTriggerReleased);
        var net = this.netId;
        if (networkLeftHandEquipedItem != null)
            Destroy(networkLeftHandEquipedItem);
        if (networkRightHand != null)
            Destroy(networkRightHand);
        networkLeftHand?.SetHand(null);
        networkRightHand?.SetHand(null);
        networkHead?.SetTransform(null,null);
    }
}
