using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ServerManager : MonoBehaviour 
{
	int myReliableChannelId;
	int socketId;
	int socketPort = 5010;
	int connectionId;

	const string ADD_GAME       = "1";
	const string ADD_PLAYER     = "2";
	const string SEND_GAME_LIST = "3";
	const string REMOVE_GAME    = "4";
	const string COMMAND_QUIT   = "1234";

	public Text messagesField;
	public NetworkGame game;
	public GameObject gameListPanel;
	public GameObject gamePanel;
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

	public void sendSocketMessage(string sendMessage, int connectionNumber) 
	{
		byte error;
		byte[] buffer = new byte[1024];
		Stream stream = new MemoryStream (buffer);
		BinaryFormatter formatter = new BinaryFormatter ();

		formatter.Serialize (stream, sendMessage);

		int bufferSize = 1024;

		NetworkTransport.Send (socketId, connectionNumber, myReliableChannelId, buffer, bufferSize, out error);
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
			Debug.Log(recConnectionId);
			break;
		case NetworkEventType.DataEvent:
			Stream stream = new MemoryStream (recBuffer);
			BinaryFormatter formatter = new BinaryFormatter ();
			string message = formatter.Deserialize (stream) as string;
         	messagesField.text = messagesField.text + "\n" + "Incoming data event recieved";
			proccessNetworkMessage(message, recConnectionId);
			break;
		case NetworkEventType.DisconnectEvent:
         messagesField.text = messagesField.text + "\n" + "Remote client event disconnected";
			break;
		}
	}

	// See what type of network message was sent
	void proccessNetworkMessage(string networkMessage, int hostNumber)
	{
		messagesField.text = messagesField.text + "\n" + networkMessage;
		// Parse the recieved string into strings based on commas
		string[] splitMessage = networkMessage.Split (':');
		string[] messageInfo  = splitMessage[1].Split(',');
		switch (splitMessage[0])
		{
		case ADD_GAME:
			messagesField.text = messagesField.text + "\n" + "Adding Game";
			addGame(messageInfo, hostNumber);
			break;
		case ADD_PLAYER:
			messagesField.text = messagesField.text + "\n" + "Adding Player";
			addPlayer (messageInfo[0]);
			break;
		case SEND_GAME_LIST:
			messagesField.text = messagesField.text + "\n" + "Sending Game List to ip" + messageInfo[0];
			sendGameList(messageInfo[0], hostNumber);
			break;
		case REMOVE_GAME:
			messagesField.text = messagesField.text + "\n" + "Removing Game";
			removeGame (messageInfo[0]);
			break;
		}
	}

	public void clearMessages()
	{
		messagesField.text = "Messages Cleared.";
	}

	public void listGames()
	{
		foreach (NetworkGame game in gameList) 
		{
			messagesField.text = messagesField.text + "\n" + game.listGame ();
		}
	}

	// Add a game to the server list
	public void addGame(string[] gameInfo, int hostNumber)
	{
		game = new NetworkGame();
      game.ipAddress       = gameInfo[0];
		game.gameName        = gameInfo[1];
		game.numberOfPlayers = gameInfo[2];
		game.maxPlayers      = gameInfo[3];
		game.password        = gameInfo[4];
		game.mapName         = gameInfo[5];
		game.connectionID    = hostNumber;
		gameList.Add(game);
		GameObject gamePNL = Instantiate (gamePanel) as GameObject;
		gamePNL.transform.SetParent (gameListPanel.transform, false);
		Text[] nameText = gamePNL.GetComponentsInChildren<Text>();
		nameText[0].text =        gameInfo[1];
		nameText[1].text =        gameInfo[2];
		nameText[2].text = "\\" + gameInfo[3];
		nameText[3].text =        gameInfo[4];
		nameText[4].text =        gameInfo[5];
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

	// Remove a game from the server side
	public void forceRemoveGame()
	{
		Text[] gameTextBoxes = this.GetComponentsInParent<Text>();
		removeGame(gameTextBoxes[5].text);
		var targetGame = gameList.FirstOrDefault(o => o.ipAddress == gameTextBoxes[5].text);
		string removeGameMessage = REMOVE_GAME + ":" + COMMAND_QUIT;
		Destroy(this.transform.parent.gameObject);
		sendSocketMessage(removeGameMessage, targetGame.connectionID);
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
	public void sendGameList(string targetIP, int hostNumber)
	{
		string gamesString = SEND_GAME_LIST + ":";
		byte error;
		
		// Makes a string of format 3:game;game;game...
		foreach(NetworkGame game in gameList)
		{
			gamesString = gamesString + game.ipAddress + "," + game.gameName + "," + game.numberOfPlayers + "," + game.maxPlayers + "," + game.password + "," + game.mapName + ";";
		}
		// Removes the last semicolon from the list
		gamesString = gamesString.Substring(0, gamesString.Length-1);

		sendSocketMessage(gamesString, hostNumber);
		NetworkTransport.Disconnect(socketId, connectionId, out error);
	}
}