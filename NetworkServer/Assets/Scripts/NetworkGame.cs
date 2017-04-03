using UnityEngine;

public class NetworkGame
{
	public NetworkGame(){}
	// string will send with format ip, name, players, password
	public string ipAddress;
	public string gameName;
 	public string numberOfPlayers;
   public string maxPlayers;
 	public string password;
	public string mapName;
	public int    hostID;
   public GameObject gamePNL;


   public string listGame()
	{
		return (ipAddress + "," + gameName + "," + numberOfPlayers + "," + maxPlayers + "," + password + "," + mapName);
	}
}