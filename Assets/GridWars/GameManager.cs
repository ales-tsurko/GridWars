using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	List<Player> players;

	public GameObject prefabTower;
	public GameObject prefabChopper;
	public GameObject prefabTank;
	public GameObject prefabLightTank;
	public GameObject prefabJeep;

	// Use this for initialization
	void Start() {
		players = new List<Player> ();
		AddPlayer ();
		AddPlayer ();
		foreach (var p in players) {
			p.Start ();
		}

		SetupTowers ();
	}

	void SetupTowers() {
		//GameObject towerGameObject = Instantiate(Resources.Load("Tower")) as GameObject;

		var prefabs = new List <GameObject> { prefabJeep, prefabChopper, prefabTank, prefabLightTank };


		for (int playerNum = 0; playerNum < 2; playerNum ++) {
			float z = 35*(playerNum == 0 ? -1 : 1);

			print("--- adding towers for player " + (playerNum + 1));

			int maxTowers = prefabs.Count;
			for (int towerNum = 0; towerNum < maxTowers; towerNum ++) {
				GameObject towerGameObject = Instantiate(prefabTower); 
				Tower tower = towerGameObject.GetComponent<Tower> ();

				if (playerNum == 0) {
					tower.prefabUnit = prefabs [towerNum];
				} else {
					tower.prefabUnit = prefabs [maxTowers - 1 - towerNum];
				}

				float x = 50*((((float)towerNum) / (float)maxTowers) - 0.5f);

				tower.setX (x);
				tower.setY (0.0f);
				tower.setZ (z);

				tower.setRotY (180*playerNum);
				print("adding tower " + towerNum + " for player " + players [playerNum].playerNumber);
				tower.player = players[playerNum];
				tower.constructUnit ();
			}
		}


		//print ("created tower " + tower);
	}
	
	// Update is called once per frame
	void FixedUpdate() {
		foreach (var p in players) {
			p.FixedUpdate();
		}
	}

	void AddPlayer() {
		var p = new Player();
		p.playerNumber = players.Count + 1;
		players.Add(p);
	}
}
