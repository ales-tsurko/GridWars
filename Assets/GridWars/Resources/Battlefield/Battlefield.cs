using UnityEngine;
using System.Collections.Generic;

public class Battlefield : MonoBehaviour {
	public static Battlefield current { //TODO: get rid of this
		get {
			return App.shared.battlefield;
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

	public Player player1 {
		get {
			return PlayerNumbered(1);
		}
	}

	public Player player2 {
		get {
			return PlayerNumbered(2);
		}
	}

	public List<Player>localPlayers {
		get {
			return players.FindAll(p => p.isLocal);
		}
	}

	public List <Player> livingPlayers {
		get {
			return players.FindAll(p => !p.IsDead());
		}
	}

	void Start() {
		Application.runInBackground = true;

		App.shared.enabled = true; //Load App so Start gets called
		App.shared.debug = true;

		//SetupTiles();
		players = new List<Player>();
		AddPlayer();
		AddPlayer();
	}

	public void StartGame() {
		App.shared.cameraController.InitCamera();

		foreach (var player in players) {
			player.StartGame();
		}
        
        /*
		foreach (GameObject tile in tiles) {
			tile.GetComponent<BrightFadeInGeneric>().OnEnable();
		}
		*/
	}

	void AddPlayer() {
		var player = this.CreateChild<Player>();
		player.battlefield = this;
		players.Add(player);
		player.gameObject.name = "Player " + player.playerNumber;
	}

	/*
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
	}*/

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
