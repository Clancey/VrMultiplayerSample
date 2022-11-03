using Mirror.Discovery;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerListItem : MonoBehaviour
{
    public long Id;
    ServerResponse serverResponse;
    public ServerResponse ServerResponse
	{
        get
        {
            return serverResponse;
        }
		set
		{
            serverResponse = value;
            Text.text = value.uri.ToString();
        }
	}

    public Text Text;
    public Text ButtonText;
    // Start is called before the first frame update
    void Start()
    {
        ButtonText.text = "Join";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
