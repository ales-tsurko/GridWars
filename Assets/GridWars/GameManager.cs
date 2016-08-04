using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	List<Player> players;

	// Use this for initialization
	void Start() {
		players = new List<Player>();
		AddPlayer();
		AddPlayer();
		foreach(var p in players) {
			p.Start();
		}
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
