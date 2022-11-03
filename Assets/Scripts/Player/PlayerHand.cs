using HTC.UnityPlugin.Vive;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public VivePoseTracker LocalHand { get; private set; }
     
    public Vector3 LocalPlayerPosition { get { return LocalHand?.transform?.position ?? Vector3.zero; } }
    public Quaternion LocalPlayerRotation { get { return LocalHand?.transform?.rotation ?? Quaternion.identity; } }
    private void Awake()
    {
    }

    public void SetHand(VivePoseTracker hand)
    {
        LocalHand = hand;
    }

    private void Update()
    {
        UpdateNetworkHandForLocalClient();
    }

    private void UpdateNetworkHandForLocalClient()
    {
        if (LocalHand != null)
        {
            transform.position = LocalPlayerPosition;
            transform.rotation = LocalPlayerRotation;
        }
    }
}