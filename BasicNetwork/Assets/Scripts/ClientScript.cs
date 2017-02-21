using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ClientScript : NetworkDiscovery {

	public InputField IPAddress;
	public Text Message;

	// Use this for initialization
	void Start () {
		Initialize ();
	}

	public void StartBroadcasting()
	{
		StartAsServer ();
	}

	public void StartListening()
	{
		StartAsClient ();
	}

	public void StopBroadcasting()
	{
		StopBroadcast ();
	}

	public override void OnReceivedBroadcast (string fromAddress, string data) {
		IPAddress.text = fromAddress;
		Message.text = Message.text + "\n" + "Game Found:" + data;
	}
}