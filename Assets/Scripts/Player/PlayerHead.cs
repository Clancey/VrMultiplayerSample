using HTC.UnityPlugin.Vive;
using Mirror;
using UnityEngine;

public class PlayerHead : MonoBehaviour
{
    VRCameraHook head;

    public Vector3 LocalPlayerPosition => head?.transform?.position ?? Vector3.zero; 
    public Quaternion LocalPlayerRotation => head?.transform ?.rotation ?? Quaternion.identity; 

    private void Update()
    {
        if (head != null)
        {
            transform.position = head.transform.position;
            transform.rotation = head.transform.rotation;
            //renderer.enabled = false;
			//LocalTransform = headTransform;
		}
		else if(player != null)
		{
            Destroy(player.gameObject);
		}
    }
    VRPlayer player;
    internal void SetTransform(VRPlayer player, VRCameraHook head)
    {
        this.head = head;
        this.player = player;   
    }
}