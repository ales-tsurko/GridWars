using UnityEngine;
using System.Collections.Generic;

public class Battlefield : MonoBehaviour {
	public Vector3 bounds = new Vector3(100f, 10f, 100f);
	public List<Player> players;

	// Use this for initialization
	void Start () {
		players = new List<Player>();
		AddPlayer();
		AddPlayer();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void AddPlayer() {
		var player = this.CreateChild<Player>();
		player.battlefield = this;
		players.Add(player);
		player.gameObject.name = "Player " + player.playerNumber;
	}
}
