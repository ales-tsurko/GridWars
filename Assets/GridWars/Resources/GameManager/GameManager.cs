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
		var unitTypes = new List <System.Type> { typeof(Jeep), typeof(LightTank), typeof(Chopper), typeof(Tank) };


		for (int playerNum = 0; playerNum < 2; playerNum ++) {
			float z = 35*(playerNum == 0 ? -1 : 1);

			//print("--- adding towers for player " + (playerNum + 1));

			int maxTowers = unitTypes.Count;
			for (int towerNum = 0; towerNum < maxTowers; towerNum ++) {
				var tower = GameUnit.Instantiate<Tower>();

				if (playerNum == 0) {
					tower.unitPrefab = GameUnit.Load(unitTypes[towerNum]);
				} else {
					tower.unitPrefab = GameUnit.Load(unitTypes[maxTowers - 1 - towerNum]);
				}

				float x = 50*((((float)towerNum) / (float)maxTowers) - 0.5f);

				tower.setX (x);
				tower.setY (0.0f);
				tower.setZ (z);

				tower.setRotY (180*playerNum);
				//print("adding tower " + towerNum + " for player " + players [playerNum].playerNumber);
				tower.player = players[playerNum];

				//if ((playerNum == 0 && towerNum == 0) || (playerNum == 1 && towerNum == 2)) {
					//tower.ReleaseUnit ();
				//}

			}
		}


		//print ("created tower " + tower);
	}

	void AddPlayer() {
		var p = new Player();
		p.playerNumber = players.Count + 1;
		players.Add(p);
	}
}
