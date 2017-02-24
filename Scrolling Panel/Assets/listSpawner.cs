using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class listSpawner : MonoBehaviour {

	public GameObject contentPanel;
	public GameObject gameInfoPrefab;

	public void addGame()
	{
		GameObject game = Instantiate(gameInfoPrefab) as GameObject;
		game.transform.SetParent(contentPanel.transform);
	}
}
