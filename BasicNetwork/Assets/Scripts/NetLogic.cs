using System;
using System.Collections.Generic;
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
   int serverConnectionID = 1; // Should always be zero but may need to reconfigure
   const string myIP = "127.0.0.1";
   public List<Player> lobbyPlayers;

   bool isHostingGame = false;
   bool inGameLobby   = false;

	List<string[]> gameList = new List<string[]>();
	public GameObject gameInfoPanel;
	public GameObject gameListCanvas;
   public GameObject playerInfoPanel;
   NetworkGame myGame;

	public Text messageLog;
	public Text ipField;

   // Game settings input fields
   public Text gameName;
   public Text maxPlayers;
   public Text gamePassword;
   public Text mapName;
   public Text connectionIDTEXT;

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
		
		requestGameList("172.16.51.127~Name~4~5~passwod~map");
	}

   // Function to allow a testing button press, as inpector doesn't allow button function calls that have parameters
   public void connecToGameBTN()
	{
		connectToGame(ipField.text);
	}
	
	// Client connecting to server
	public void connectToGame(string ipAddress)
	{
		byte error;
		messageLog.text = messageLog.text + "\n" + "Trying to connect to: " + ipAddress;
		connectionId = NetworkTransport.Connect (0, ipAddress, socketPort, 0, out error);
		messageLog.text = messageLog.text + "\n" + "ConnectionID: " + connectionId;
	}

	// Send game info to the server
    public void hostGame()
    {
        if (!isHostingGame)
        {
            string gameInfo;
            isHostingGame = true;

            // Initialize the network game for hosting
            myGame = new NetworkGame()
            {
               gameName   = gameName.text,
               maxPlayers = maxPlayers.text,
               password   = gamePassword.text,
               mapName    = mapName.text
            };

            gameInfo = Constants.addGame + Constants.commandDivider + Network.player.ipAddress + Constants.gameDivider + myGame.gameName +
               Constants.gameDivider + "0" + Constants.gameDivider + myGame.maxPlayers + Constants.gameDivider + myGame.password + Constants.gameDivider + myGame.mapName;
            sendSocketMessage(gameInfo, serverConnectionID);
        }
        else
        {
            messageLog.text = messageLog.text + "\nError: Already hosting a game.";
        }
    }
	
	// Send a socket message to connectionId
	public void sendSocketMessage(string message, int connectionNum) 
	{
		byte error;
		byte[] buffer = new byte[1024];
		Stream stream = new MemoryStream (buffer);
		BinaryFormatter formatter = new BinaryFormatter ();

		formatter.Serialize (stream, message);

		int bufferSize = 1024;
		messageLog.text = messageLog.text + "\nSending to ID " + connectionNum + " : " + message;
		NetworkTransport.Send (socketId, connectionNum, myReliableChannelId, buffer, bufferSize, out error);
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
			messageLog.text = messageLog.text + "\nIncoming: " + message;
			processNetworkMessage(message, recConnectionId);
			break;
		case NetworkEventType.DisconnectEvent:
			messageLog.text = messageLog.text + "\n" + "Remote client event disconnected";
			break;
		}
	}

	public void processNetworkMessage(string networkMessage, int recConnectionID)
	{
		string[] gameInfo = networkMessage.Split (Convert.ToChar(Constants.commandDivider));

		switch (gameInfo[0])
		{
			case Constants.addPlayer:         // #, ipAddress, password
            addPlayer(gameInfo[1], recConnectionID);
            messageLog.text = messageLog.text + "\nAdding Player";
            break;
			case Constants.requestGameList:   // #, game, game, game, game...
				requestGameList(gameInfo[1]);
				break;
			case Constants.cancelGame:        // #
				removeGame(gameInfo[1]);
				break;
			case Constants.gameStarted:       // #, ipAddress
			case Constants.gameEnded:         // #, ipAddress
			case Constants.characterSelect:   // #, character
				break;
			case Constants.characterResult:   // #, characterResult
				break;
			case Constants.diceRoll:          // #, number1, number2
				break;
			case Constants.endTurn:           // #, player
				break;
			case Constants.startTurn:         // #, player
				break;
			case Constants.sendChat:          // #, player
				break;
			case Constants.networkError:      // #, info
				networkError(gameInfo[1]);
				break;
		    default:
				networkError(gameInfo[1]);
				break;
		}
	}

   // Send request to host to join game
   public void requestGameJoin()
   {
      messageLog.text = messageLog.text + "\nRequesting to join game, connectionID:" + connectionId;
      string message = Constants.addPlayer + Constants.commandDivider + Network.player.ipAddress;
      sendSocketMessage(message, connectionId);
   }

    // Player connecting to your lobby
	public void addPlayer(string gameInfo, int connectionID)
	{
      messageLog.text = messageLog.text + "\nInside add player";
      // Tell connecting player he failed to to join lobby, it is full or we are not hosting
      if (!isHostingGame || !myGame.addPlayer())
      {
         lobbyFull(connectionID);
         return;
      }

      // Create player class
      Player player = new Player()
      {
         connectionID = connectionID,
         ipAddress = gameInfo,
      };

      lobbyPlayers.Add(player);

      // Create UI GameObject to list player
      // Destroy everything in the panel
      foreach (Transform child in gameListCanvas.transform)
      {
         GameObject.Destroy(child.gameObject);
      }
      Instantiate(playerInfoPanel, gameListCanvas.transform, false);
   }
	
    // Player connecting to full or canceled lobby
    public void lobbyFull(int connectionID)
    {
      string message = Constants.lobbyFull + Constants.commandDivider + Network.player.ipAddress;
      sendSocketMessage(message, connectionID);
    }

	// Ask server to send list of games
	public void requestGameListServer()
	{
		sendSocketMessage(Constants.requestGameList + Constants.commandDivider + myIP, serverConnectionID);
	}
	
	// Updates the local game list with list from server
	void requestGameList(string serverGameList)
	{
		gameList.Clear();
		string[] gameInfo = serverGameList.Split(Convert.ToChar(Constants.gameListDivider));
		messageLog.text = messageLog.text + "\n" + "Adding Games\n";
		foreach (string game in gameInfo)
		{
			string[] tempGame = game.Split(Convert.ToChar(Constants.gameDivider));
			foreach (string item in tempGame)
			{
				messageLog.text = messageLog.text + item;
			}
			gameList.Add(tempGame);
		}
		refreshGameList();
	}

	// Stop game from hosting if server sends a kill command
	public void removeGame(string gameInfo)
	{
		if (gameInfo == Constants.serverKillCode)
		{
			isHostingGame = false;
			messageLog.text = messageLog.text + "\n Your game has been canceled by the server";
		}
	}
	
	public void refreshGameList()
	{
		foreach (Transform child in gameListCanvas.transform)
		{
			GameObject.Destroy(child.gameObject);
		}
		
		foreach (string[] game in gameList)
		{
			GameObject serverGame = Instantiate(gameInfoPanel) as GameObject;
			serverGame.transform.SetParent(gameListCanvas.transform, false);
			Text[] nameText = serverGame.GetComponentsInChildren<Text>();
			nameText[0].text =        game[1];
			nameText[1].text = 		  game[2];
			nameText[2].text = "\\" + game[3];
			nameText[3].text =        game[4];
			nameText[4].text =        game[5];
			nameText[6].text =        game[0];
		}
	}

	// Tell the server that game is canceled
	public void cancelGame()
	{
      if (isHostingGame)
      {
         isHostingGame = false;
         sendSocketMessage(Constants.cancelGame + Constants.commandDivider + Network.player.ipAddress, serverConnectionID);
      }
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
	void networkError(string gamInfo)
	{

	}
}