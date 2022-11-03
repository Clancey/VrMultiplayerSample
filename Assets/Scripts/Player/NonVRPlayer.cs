using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class NonVRPlayer : Player
{
    public GameObject localCamera;
    public GameObject playerCameraFollow;
    public GameObject eventSystem;
    public GameObject mobileInterface;
    public GameObject localPawnControls;
    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            //Set the camera to follow!
            var mainCamera = Camera.main;
            if(mainCamera != localCamera && localCamera)
            {
                if(mainCamera != null && mainCamera.gameObject != null)
                    mainCamera?.gameObject?.SetActive(false);
                localCamera.SetActive(true);
            }
            playerCameraFollow.SetActive(true);
        }
        var shouldBeACtive = isLocalPlayer;

        if (localPawnControls != null)
            localPawnControls?.SetActive(shouldBeACtive);
        if (eventSystem != null)
            eventSystem?.SetActive(shouldBeACtive);
        //if (isLocalPlayer)
        //{
        //}
        //else
        //{
        //    if(mainCamera != null)
        //        mainCamera?.SetActive(false);
        //    if(localPawnControls != null)
        //        localPawnControls?.SetActive(false);
        //    followCamera?.SetActive(false);
        //    eventSystem?.SetActive(false);
        //    mobileInterface?.SetActive(false);
        //}
        //var camera = GameObject.FindGameObjectWithTag("MainCamera");
        //var cinamaBrain = camera.GetComponent<CinemachineBrain>();
        //cinamaBrain.camer = this.GetComponentInChildren<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
