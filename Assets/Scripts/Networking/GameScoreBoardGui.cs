using Mirror;
using Mirror.Examples.NetworkRoom;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameScoreBoardGui : MonoBehaviour
{
    MyNetworkManager networkManager;

    [Tooltip("Status Text")]
    public Text statusText;
    void Awake()
    {
        networkManager = GameObject.FindObjectOfType<MyNetworkManager>();
       
    }

    // Update is called once per frame
    void Update()
    {
        string text = "";
        foreach(var p in networkManager.roomSlots)
		{
            var score = p.gameObject.GetComponent<PlayerScore>();
            if (score == null)
                continue;
            text += $"Player {score.index + 1}: {score.score}{Environment.NewLine}";
		}
        statusText.text = text; 
    }
}
