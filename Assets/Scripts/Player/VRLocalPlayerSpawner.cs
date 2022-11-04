using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
public class VRLocalPlayerSpawner : MonoBehaviour {

    public static bool HasSetup;

    const string OpenVr = "OpenVR";
    const string Oculus = "Oculus";
    public GameObject VrCameraPrefab;
#if VR
    public static bool ShouldUseVr = true;
#else
    public static bool ShouldUseVr = false;// Application.isEditor;
#endif

	void Start () {
        if (HasSetup)
            return;
        var localPawn = FindObjectOfType<LocalVRPlayer>();
        if (localPawn)
            return;
#if UNITY_ANDROID
        Setup();

#else
        string[] arguments = Environment.GetCommandLineArgs();
        foreach(var arg in arguments) {
            if(arg.StartsWith("-vr")) {
                ShouldUseVr = true;
            }
            else if (arg.StartsWith("-novr"))
			{
                ShouldUseVr = false;
			}
        }

        if (!ShouldUseVr)
        {
            UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.StopSubsystems();

            UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.DeinitializeLoader();

            XRSettings.LoadDeviceByName("None");

            XRSettings.enabled = false;
            HasSetup = true;
            return;
        }

        StartCoroutine(DoInitializeSteamVR());

        //if (!UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.isInitializationComplete || !XRSettings.enabled)
        //{
        //    XRSettings.LoadDeviceByName("Oculus");
        //    UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.InitializeLoaderSync();
        //    UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.StartSubsystems();
        //    XRSettings.enabled = true;
        //}
#endif
    }

    private IEnumerator DoInitializeSteamVR(bool forceUnityVRToOpenVR = false)
    {
        yield return UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.InitializeLoader();
        if (UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.LogError("Initializing XR Failed. Check Editor or Player log for details.");
        }
        else
        {
            Debug.Log("Starting XR...");
            UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.StartSubsystems();
        }

        Setup();
    }

    void Setup()
	{

        var currentAudioListener = FindObjectOfType<AudioListener>();
        currentAudioListener.enabled = false;
        var mainCamera = GameObject.FindObjectOfType<Camera>();
        mainCamera.gameObject.SetActive(false);
        var camera = (GameObject)Instantiate(VrCameraPrefab, Vector3.zero, Quaternion.identity);
        HasSetup = true;
        //For steam VR
        //if (!FindObjectOfType<VRUISystem>())
        //{
        //    var eventSystem = GameObject.Find("EventSystem") ?? this.gameObject;
        //    eventSystem.AddComponent<VRUISystem>();

        //}

    }
}
