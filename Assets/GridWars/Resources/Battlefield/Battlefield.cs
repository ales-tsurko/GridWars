using UnityEngine;
using System.Collections.Generic;

public class Battlefield : MonoBehaviour {
	public static Battlefield current {
		get {
			return GameObject.Find("Battlefield").GetComponent<Battlefield>();
		}
	}

	public Vector3 bounds = new Vector3(100f, 10f, 100f);
	public List<Player> players;

	public Player PlayerNumbered(int playerNumber) {
		if (playerNumber < 1 || playerNumber > players.Count) {
			return null;
		}
		else {
			return players[playerNumber - 1];
		}
	}

	void Start() {
		Application.runInBackground = true;

		Network.shared.enabled = true;

		CameraController.instance.enabled = true;
		SetupTiles();
	}

	public void StartGame() {
		players = new List<Player>();
		AddPlayer();
		AddPlayer();
		/*
		foreach (GameObject tile in tiles) {
			tile.GetComponent<BrightFadeInGeneric>().OnEnable();
		}
		*/
	}

	void FixedUpdate () {
		//print("livingPlayers().Count = " + livingPlayers().Count);

		if (livingPlayers().Count == 1) {
			print("Game Over Man");
			Network.shared.LeaveGame();
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

	public void Reset() {
		
		App.shared.timerCenter.isPaused = true;

		var preservedGameObjectNames = new List<string>(new string[]{
			"Main Camera",
			"Canvas",
			"Directional Light",
			"Battlefield",
			"CameraPositions",
			"KeyboardUICanvas",
			"EventSystem",
			"Network",
			"App",
			"BoltControl",
			"BoltBehaviours"
		});

		var objs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

		foreach (GameObject obj in objs) {
			if (!preservedGameObjectNames.Contains(obj.name)) {
				Destroy(obj);
			}
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

	// --- Music ----------------------------------------------------

	AudioSource _bgAudioSource;
	protected AudioSource bgAudioSource {
		get {
			if (_bgAudioSource == null) {
				_bgAudioSource = gameObject.AddComponent<AudioSource>();
			}
			return _bgAudioSource;
		}
	}

	List <GameObject> tiles;

	void SetupTiles() {
		tiles = new List<GameObject>();

		GameObject tilePrefab = Resources.Load<GameObject> ("FX/Prefabs/Tile");

		//Vector3 size = tilePrefab.GetComponent<Renderer>().bounds.size;
		float r = 5;
		int maxX = 15;
		int maxZ = 30;
		for (int x = -maxX; x < maxX+1; x ++) {
			for (int z = -maxZ; z < maxZ+1; z ++) {
				GameObject tile = Instantiate(tilePrefab);
				tile.transform.position = new Vector3(x*r, 0, z*r);
				//if (x > 0) {
				float v = (float)Mathf.Abs(x)/(float)maxX;
				//tile.PaintDarken(1f - v*v*v*v);
				//}
				tiles.Add(tile);
			}
		}
	}
}
