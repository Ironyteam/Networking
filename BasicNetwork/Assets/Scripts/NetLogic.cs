using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetLogic : MonoBehaviour 
{
	int myReliableChannelId;
	int socketId;
	int socketPort = 5010;
	int connectionId;
	string gameName = "My Game";
    bool isHostingGame;
	list<string[]> gameList = new list<string[]>();

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

	// Function to allow a testing button press
	public void connecToGameBTN()
	{
		connectToGame(ipField.text);
	}
	
	// Client connecting to server
	public void connectToGame(string ipAddress)
	{
		byte error;
		connectionId = NetworkTransport.Connect (0, ipAddress, socketPort, 0, out error);
		messageLog.text = messageLog.text + "\n" + "Connected to server. ConnectionID: " + connectionId + " IP: " + ipField.text;
	}

	// Send game info to the server
    public void hostGame()
    {
        if (!isHostingGame)
        {
            string gameInfo;
            isHostingGame = true;
            gameInfo = Constants.addGame + "," + Network.player.ipAddress + "," + gameName + "," + "6" + "," + "6" + "," + "password" + "," + "Binary";
            messageLog.text = messageLog.text + "\nSending: " + gameInfo;
            sendSocketMessage(gameInfo);
        }
        else
        {
            messageLog.text = messageLog.text + "\nError: Already hosting a game.";
        }
    }
	
	// Send a socket message to connectionId
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
			// Commented out cases shouldn't be needed on the clients
			//case Constants.addGame:         // #, ipAddress, gameName, players, maxPlayers, password, mapName
			//case Constants.addPlayer:       // #, ipAddress, password
			case Constants.requestGameList: // #, game, game, game, game...
				requestGameList(gameInfo);
				break;
			//case Constants.cancelGame:      // #, ipAddress
			//case Constants.gameStarted:     // #, ipAddress
			//case Constants.gameEnded:       // #, ipAddress
			case Constants.characterSelect: // #, character
				break;
			case Constants.characterResult: // #, characterResult
				break;
			case Constants.diceRoll:        // #, number1, number2
				break;
			case Constants.buildSettlement: // #, x, y, player
				break;
			case Constants.upgradeToCity:   // #, x, y, player
				break;
			case Constants.buildRoad:       // #, x, y, player
				break;
			case Constants.buildArmy:       // #, x, y, player
				break;
			case Constants.attackCity:      // #, x, y, player
				break;
			case Constants.moveRobber:      // #, x, y
				break;
			case Constants.endTurn:         // #, player
				break;
			case Constants.startTurn:       // #, player
				break;
			case Constants.sendChat:        // #, player
				break;
			case Constants.networkError:    // #, info
				break;
			default:
				networkError(gameInfo)
				break;
		}
	}


	void addGame(string[] gameInfo)
	{
		
	}

	void addPlayer(string[] gameInfo)
	{

	}

	// Gets a list of games from the server
	void requestGameList(string[] gameInfo)
	{
		gameList.Clear();
		
		foreach (string game in gameInfo)
		{
			string[] tempGame = game.Split(":");
			gameList.Add(tempGame);
		}
	}

	// Tell the server that game is canceled
	void cancelGame(string[] gameInfo)
	{

	}
	
	// Tell the server the game has started
	void gameStarted(string[] gameInfo)
	{

	}

	// Tell the server the game has finished
	void gameEnded(string[] gameInfo)
	{

	}

	// Send character select choice
	void characterSelect(string[] gameInfo)
	{

	}
	
	// Recieve message telling if character choice was successful
	void characterResult(string[] gameInfo)
	{

	}

	// Get the result of a dice roll
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

	// Called if invalid network message is sent
	void networkError(string[] gamInfo)
	{

	}

}