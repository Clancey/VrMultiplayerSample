using UnityEngine;
using System.Collections;
using Mirror;

public class NetworkSyncTransform : NetworkBehaviour
{
    public NetworkSyncTransform()
    {
        this.syncInterval = .01f;
    }
    [SerializeField]
    GameObject playerCamera;
    [SerializeField]
    float _posLerpRate = 15;
    [SerializeField]
    float _rotLerpRate = 15;
    [SerializeField]
    float _posThreshold = 0.1f;
    [SerializeField]
    float _rotThreshold = 1f;

    [SyncVar]
    Vector3 _lastPosition;

    [SyncVar]
    Vector3 _lastRotation;

    [SyncVar]
    Vector3 _lastCamRotation;

    void Update()
    {
        if (isLocalPlayer)
            return;
        InterpolatePosition();
        InterpolateRotation();
        InterpolateCamRotation();
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        var posChanged = IsPositionChanged();

        if (posChanged)
        {
            CmdSendPosition(transform.position);
            _lastPosition = transform.position;
        }

        var rotChanged = IsRotationChanged();

        if (rotChanged)
        {
            CmdSendRotation(transform.localEulerAngles);
            _lastRotation = transform.localEulerAngles;
        }

        var camrotChanged = IsCamRotationChanged();

        if (camrotChanged)
        {
            CmdSendCamRotation(playerCamera.transform.localEulerAngles);
            _lastCamRotation = playerCamera.transform.localEulerAngles;
        }
    }

    public void InterpolatePosition()
    {
        transform.position = Vector3.Lerp(transform.position, _lastPosition, Time.deltaTime * _posLerpRate);
    }

     void InterpolateRotation()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(_lastRotation), Time.deltaTime * _rotLerpRate);
    }

    void InterpolateCamRotation()
    {
        if (playerCamera == null)
            return;
        playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, Quaternion.Euler(_lastCamRotation), Time.deltaTime * _rotLerpRate);
    }

    [Command(channel = Channels.Unreliable)]
    public void CmdSendPosition(Vector3 pos)
    {
        _lastPosition = pos;
    }

    [Command(channel = Channels.Unreliable)]
     void CmdSendRotation(Vector3 rot)
    {
        _lastRotation = rot;
    }

    [Command(channel = Channels.Unreliable)]
    void CmdSendCamRotation(Vector3 rot)
    {
        _lastCamRotation = rot;
    }

    bool IsPositionChanged()
    {
        return Vector3.Distance(transform.position, _lastPosition) > _posThreshold;
    }

    bool IsRotationChanged() => Vector3.Distance(transform.localEulerAngles, _lastRotation) > _rotThreshold;

    bool IsCamRotationChanged() => playerCamera != null ? Vector3.Distance(playerCamera.transform.localEulerAngles, _lastCamRotation) > _rotThreshold : false;

    //public override int GetNetworkChannel() => Channels.DefaultUnreliable;

    //public override float GetNetworkSendInterval() => 0.01f;

}