using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace GameClass
{

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
      /*
		public string IpAddress { get { return ipAddress; } set { ipAddress = value; } }
		public string GameName { get; set;}
		public string NumberOfPlayers { get; set;}
		public string MaxPlayers { get; set;}
		public string Password { get; set;}
      */
	}
}

