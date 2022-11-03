using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VRCameraToggle : MonoBehaviour
{
    public Camera DisplayCamera;
    public bool showDeviceView;
    public float RenderScale;
    // Start is called before the first frame update
    void Start()
    {
        updateCamera();
    }

    void updateCamera()
    {
        DisplayCamera.enabled = showDeviceView;
        XRSettings.showDeviceView = !showDeviceView;
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            showDeviceView = !showDeviceView;
            updateCamera();
        }
    }
}
