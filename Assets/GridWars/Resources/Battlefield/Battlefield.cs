using UnityEngine;
using System.Collections.Generic;

public class Battlefield : Bolt.GlobalEventListener {
	public static Battlefield current {
		get {
			return GameObject.Find("Battlefield").GetComponent<Battlefield>();
		}
	}

	public Vector3 bounds = new Vector3(100f, 10f, 100f);
	public List<Player> players;

	public Player PlayerNumbered(int playerNumber) {
		return players[playerNumber - 1];
	}

	void Start() {
		Application.runInBackground = true;

		//Network.shared.enabled = false;
		CameraController.instance.enabled = false;

		if (!Network.shared.enabled) {
			StartGame();
		}
	}

	public override void BoltStartDone() {
		StartGame();
	}

	void StartGame() {
		players = new List<Player>();
		AddPlayer();
		AddPlayer();
	}

	void FixedUpdate () {
		//print("livingPlayers().Count = " + livingPlayers().Count);

		if (livingPlayers().Count == 1) {
			print("Game Over Man");
			Pause();
			UnityEngine.SceneManagement.SceneManager.LoadScene("BattleField");
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
