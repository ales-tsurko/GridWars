using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	List<Player> players;

	public GameObject prefabTower;
	public GameObject prefabTank;
	public GameObject prefabLightTank;
	public GameObject prefabChopper;
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
		GameObject towerGameObject = Instantiate(prefabTower); 

		Tower tower = towerGameObject.GetComponent<Tower> ();




		tower.setX (10.0f);
		tower.setZ (10.0f);
		tower.setY (14.0f);
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
