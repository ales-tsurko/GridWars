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

	void FixedUpdate () {
		//print("livingPlayers().Count = " + livingPlayers().Count);

		if (livingPlayers().Count == 1) {
			print("Game Over Man");
			Pause();
		}
	}

	void AddPlayer() {
		var player = this.CreateChild<Player>();
		player.battlefield = this;
		players.Add(player);
		player.gameObject.name = "Player " + player.playerNumber;
	}

	List <Player> livingPlayers() {
		List <Player> results = new List<Player>();

		foreach (Player player in players) {
			if (!player.IsDead()) {
				results.Add(player);
			}
		}

		return results;
	}

	void Pause() {
		List <GameObject> objs = activeGameObjects();

		foreach (GameObject obj in objs) {
			obj.SetActive(false);
		}
	}

	public virtual List<GameObject> activeGameObjects() {
		GameObject[] objs = (GameObject[])UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
		var results = new List<GameObject>();
		foreach (GameObject obj in objs) {
			if (obj.activeInHierarchy) {
				results.Add(obj);
			}
		}
		return results;
	}
}
