using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using GameClass;

public class ServerManager : MonoBehaviour 
{
	int myReliableChannelId;
	int socketId;
	int socketPort = 5011;
	int connectionId;
	const string ADD_GAME       = "1";
	const string ADD_PLAYER     = "2";
	const string SEND_GAME_LIST = "3";
	const string REMOVE_GAME    = "4";


	public Text messagesField;
	public NetworkGame game;
	List<NetworkGame> gameList = new List<NetworkGame>(); 

	// Use this for initialization
	void Start () 
	{
		int maxConnections = 5;

		// Opens a port on local computer that will be used for sending and recieving, done on client and server
		NetworkTransport.Init ();
		ConnectionConfig config = new ConnectionConfig();
		myReliableChannelId = config.AddChannel (QosType.Reliable);
		HostTopology topology = new HostTopology (config, maxConnections);
		socketId = NetworkTransport.AddHost (topology, socketPort);
		messagesField.text = messagesField.text + "\n" + "Socket open. Socket ID is : " + socketId;			
	}

	// Client connecting to server
	public void connectToGame(string connectionIP)
	{
		byte error;
		connectionId = NetworkTransport.Connect (0, connectionIP, socketPort, 0, out error);
        messagesField.text = messagesField.text + "\n" + "Connected to server. ConnectionID: " + connectionId;
	}

	public void sendSocketMessage(string sendMessage) 
	{
		byte error;
		byte[] buffer = new byte[1024];
		Stream stream = new MemoryStream (buffer);
		BinaryFormatter formatter = new BinaryFormatter ();

		formatter.Serialize (stream, sendMessage);

		int bufferSize = 1024;

		NetworkTransport.Send (socketId, connectionId, myReliableChannelId, buffer, bufferSize, out error);
	}

	// Called every frame
	void Update() 
	{
		int recHostId;
		int recConnectionId;
		int recChannelId;
		byte[] recBuffer = new byte[1024];
		int bufferSize = 1024;
		int dataSize;
		byte error;
		NetworkEventType recNetworkEvent = NetworkTransport.Receive (out recHostId, out recConnectionId, out recChannelId, recBuffer, bufferSize, out dataSize, out error);

		switch (recNetworkEvent) 
		{
		case NetworkEventType.Nothing:
			break;
		case NetworkEventType.ConnectEvent:
            messagesField.text = messagesField.text + "\n" + "Incoming connection event received";
			break;
		case NetworkEventType.DataEvent:
			Stream stream = new MemoryStream (recBuffer);
			BinaryFormatter formatter = new BinaryFormatter ();
			string message = formatter.Deserialize (stream) as string;
            messagesField.text = messagesField.text + "\n" + "Incoming data event recieved";
            proccessNetworkMessage(message);
			break;
		case NetworkEventType.DisconnectEvent:
            messagesField.text = messagesField.text + "\n" + "Remote client event disconnected";
			break;
		}
	}

	// Add a game to the server list
	public void addGame(string[] gameInfo)
	{
		game = new NetworkGame();
        game.ipAddress       = gameInfo[1];
		game.gameName        = gameInfo[2];
		game.numberOfPlayers = gameInfo[3];
		game.maxPlayers      = gameInfo[4];
		game.password        = gameInfo[5];
		gameList.Add(game);
	}

	// Increase the player count of a game
	public void addPlayer(string targetIP)
	{
		var item = gameList.FirstOrDefault(o => o.ipAddress == targetIP);
		if (item != null)
		{
			if (Int32.Parse(item.maxPlayers) > Int32.Parse(item.numberOfPlayers))
			{
				int currentNumber = int.Parse(item.numberOfPlayers);
				currentNumber++;
				item.numberOfPlayers = currentNumber.ToString();
			}
		}
	}

	// Remove a game from the list
	public void removeGame(string targetIP)
	{
		var item = gameList.FirstOrDefault(o => o.ipAddress == targetIP);
		if (item != null)
		{
			gameList.Remove (item);
		}
	}

	// Send the list of games to the ip address
	public void sendGameList(string targetIP)
	{
		string gamesString = "";
		byte error;
		foreach(NetworkGame game in gameList)
		{
			gamesString = gamesString + game.ipAddress + "," + game.gameName + "," + game.numberOfPlayers + "," + game.maxPlayers + "," + game.password + ":";
		}
		connectToGame(targetIP);
		sendSocketMessage(gamesString);
		NetworkTransport.Disconnect(socketId, connectionId, out error);
	}

	// See what type of network message was sent
	void proccessNetworkMessage(string networkMessage)
	{
        messagesField.text = messagesField.text + "\n" + networkMessage;
		// Parse the recieved string into strings based on commas
		string[] gameInfo = networkMessage.Split (',');
		switch (gameInfo[0])
		{
			case ADD_GAME:
                messagesField.text = messagesField.text + "\n" + "Adding Game";
				//addGame(gameInfo);
				break;
			case ADD_PLAYER:
                messagesField.text = messagesField.text + "\n" + "Adding Player";
				addPlayer (gameInfo [1]);
				break;
			case SEND_GAME_LIST:
                messagesField.text = messagesField.text + "\n" + "Sending Game List";
				sendGameList(gameInfo[1]);
				break;
			case REMOVE_GAME:
                messagesField.text = messagesField.text + "\n" + "Removing Game";
				removeGame (gameInfo [1]);
				break;
		}
	}
}