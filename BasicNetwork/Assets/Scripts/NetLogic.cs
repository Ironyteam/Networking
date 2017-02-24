using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using namespace Constants

public class NetLogic : MonoBehaviour 
{
 /*   const string ADDGAME         = "1";
    const string ADDPLAYER       = "2";
    const string SENDGAMELIST    = "3";
    const string CANCELGAME      = "4";
	const string GAMESTARTED     = "5";
	const string GAMEENDED       = "6";
	const string CHARACTERSELECT = "7";
	const string DICEROLL        = "8";
	const string BUILDSETTLEMENT = "9";
	const string UPGRADETOCITY   = "10";
	const string BUILDROAD       = "11";
	const string BUILDARMY       = "12";
	const string ROBBERMOVE      = "13";
	const string ATTACKCITY      = "14";
	const string ENDTURN         = "15";
	const string STARTTURN       = "16";
	const string SENDCHAT        = "17";
	const string ERROR           = "18";
*/

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

	void processNetworkMessage(string networkMessage);
	{
		string[] gameInfo = networkMessage.Split (',');
		switch (gameInfo[0])
		{
			case Constants.addGame:
			case Constants.addPlayer:
			case Constants.requestGameList:
			case Constants.cancelGame:
			case Constants.gameStarted:
			case Constants.gameEnded:
			case Constants.characterSelect:
			case Constants.characterResult:
			case Constants.diceRoll:
			case Constants.buildSettlement:
			case Constants.upgradeToCity:
			case Constants.buildRoad:
			case Constants.buildArmy:
			case Constants.attackCity:
			case Constants.moveRobber:
			case Constants.endTurn:
			case Constants.startTurn:
			case Constants.sendChat:
			case Constants.error:
		}
	}

	void addGame(string gameInfo[])
	{

	}

	void addPlayer(string gameInfo[])
	{

	}

	void requestGameList(string gameInfo[])
	{

	}

	void cancelGame(string gameInfo[])
	{

	}
	
	void gameStarted(string gameInfo[])
	{

	}

	void gameEnded(string gameInfo[])
	{

	}

	void characterSelect(string gameInfo[])
	{

	}
	
	void characterResult(string gameInfo[])
	{

	}

	void diceRoll(string gameInfo[])
	{

	}

	void buildSettlement(string gameInfo[])
	{

	}

	void upgradeToCity(string gameInfo[])
	{

	}

	void buildRoad(string gameInfo[])
	{

	}

	void buildArmy(string gameInfo[])
	{

	}

	void attackCity(string gameInfo[])
	{

	}

	void moveRobber(string gameInfo[])
	{

	}

	void endTurn(string gameInfo[])
	{

	}

	void startTurn(string gameInfo[])
	{

	}

	void sendChat(string gameInfo[])
	{

	}

	void networkError(string gameInfo[])
	{

	}

}