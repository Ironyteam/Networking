using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Constants;

public class NetLogic : MonoBehaviour 
{
	int myReliableChannelId;
	int socketId;
	int socketPort = 5010;
	int connectionId;

    bool isHostingGame;

	public Text messageInputField;
	public Text messageLog;
	public Text ipField;
	public Text portField;

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
		messageLog.text = messageLog.text + "\n" + "Socket open. Socket ID is : " + socketId;			
	}

	// Client connecting to server
	public void connectToGame()
	{
		byte error;
		connectionId = NetworkTransport.Connect (0, ipField.text, socketPort, 0, out error);
		messageLog.text = messageLog.text + "\n" + "Connected to server. ConnectionID: " + connectionId;
	}

    public void hostGame()
    {
        if (!isHostingGame)
        {
            string gameInfo;
            isHostingGame = true;
            gameInfo = ADDGAME + "," + Network.player.ipAddress + "," + GAMENAME + "," + "6" + "," + "6" + "," + "password";
            messageLog.text = messageLog.text + "\nSending: " + gameInfo;
            sendSocketMessage(gameInfo);
        }
        else
        {
            messageLog.text = messageLog.text + "\nError: Already hosting a game.";
        }
    }
	public void sendSocketMessage(string message) 
	{
		byte error;
		byte[] buffer = new byte[1024];
		Stream stream = new MemoryStream (buffer);
		BinaryFormatter formatter = new BinaryFormatter ();

		formatter.Serialize (stream, message);

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
			messageLog.text = messageLog.text + "\n" + "Incoming connection event received";
			break;
		case NetworkEventType.DataEvent:
			Stream stream = new MemoryStream (recBuffer);
			BinaryFormatter formatter = new BinaryFormatter ();
			string message = formatter.Deserialize (stream) as string;
			messageLog.text = messageLog.text + "\n" + message;
			processNetworkMessage(message);
			break;
		case NetworkEventType.DisconnectEvent:
			messageLog.text = messageLog.text + "\n" + "Remote client event disconnected";
			break;
		}
	}

	public void processNetworkMessage(string networkMessage)
	{
		string[] gameInfo = networkMessage.Split (',');

		switch (gameInfo[0])
		{
			case Constants.addGame:         // #, ipAddress, gameName, players, maxPlayers, password, mapName
			case Constants.addPlayer:       // #, ipAddress, password
			case Constants.requestGameList: // #, game:game:game:game...
			case Constants.cancelGame:      // #, ipAddress
			case Constants.gameStarted:     // #, ipAddress
			case Constants.gameEnded:       // #, ipAddress
			case Constants.characterSelect: // #, character
			case Constants.characterResult: // #, characterResult
			case Constants.diceRoll:        // #, number1, number2
			case Constants.buildSettlement: // #, x, y, player
			case Constants.upgradeToCity:   // #, x, y, player
			case Constants.buildRoad:       // #, x, y, player
			case Constants.buildArmy:       // #, x, y, player
			case Constants.attackCity:      // #, x, y, player
			case Constants.moveRobber:      // #, x, y
			case Constants.endTurn:         // #, player
			case Constants.startTurn:       // #, player
			case Constants.sendChat:        // #, player
			case Constants.error:           // #, info
				break;
		}
	}


	void addGame(string[] gameInfo)
	{
		
	}

	void addPlayer(string[] gameInfo)
	{

	}

	void requestGameList(string[] gameInfo)
	{

	}

	void cancelGame(string[] gameInfo)
	{

	}
	
	void gameStarted(string[] gameInfo)
	{

	}

	void gameEnded(string[] gameInfo)
	{

	}

	void characterSelect(string[] gameInfo)
	{

	}
	
	void characterResult(string[] gameInfo)
	{

	}

	void diceRoll(string[] gameInfo)
	{

	}

	void buildSettlement(string[] gameInfo)
	{

	}

	void upgradeToCity(string[] gameInfo)
	{

	}

	void buildRoad(string[] gameInfo)
	{

	}

	void buildArmy(string[] gameInfo)
	{

	}

	void attackCity(string[] gameInfo)
	{

	}

	void moveRobber(string[] gamInfo)
	{

	}

	void endTurn(string[] gamInfo)
	{

	}

	void startTurn(string[] gamInfo)
	{

	}

	void sendChat(string[] gamInfo)
	{

	}

	void networkError(string[] gamInfo)
	{

	}

}