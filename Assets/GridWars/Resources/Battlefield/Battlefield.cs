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
	public GameUnitCache gameUnitCache;

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

	public Player localPlayer1 {
		get {
			if (localPlayers.Count > 0) {
				return localPlayers[0];
			}
			else {
				return null;
			}
		}
	}

	public Player localPlayer2 {
		get {
			if (localPlayers.Count > 1) {
				return localPlayers[1];
			}
			else {
				return null;
			}
		}
	}

	public List <Player> livingPlayers {
		get {
			return players.FindAll(p => !p.IsDead());
		}
	}

	public bool isEmpty {
		get {
			return players.TrueForAll((p) => {
				return (p.units.Count == 0) && (p.fortress == null || p.fortress.powerSource == null);
			});
		}
	}

	public bool isInternetPVP;
	public bool isAiVsAi;
	public bool canCheckGameOver; //don't check game over until a unit is received from server

	void SetupResolution() {
		Resolution[] res = Screen.resolutions;
		Resolution r = res[res.Length - 1];

		Screen.SetResolution(r.width, r.height, true); // last arg is bool for fullscreen
	}

	void Start() {
		SetupResolution();
		Application.runInBackground = true;

		App.shared.enabled = true; //Load App so Start gets called
		App.shared.debug = true;

		gameUnitCache = new GameUnitCache();

		AddPlayers();
	}

	void AddPlayers() {
		players = new List<Player>();
		AddPlayer();
		AddPlayer();
	}

	public void UpdatePlayerInputs() {
		
	}

	public void StartGame() {
		foreach (var player in players) {
			player.StartGame();
		}

		App.shared.cameraController.InitCamera(); //depends on fortress.  call after StartGame
	}

	void AddPlayer() {
		var player = this.CreateChild<Player>();
		player.battlefield = this;
		players.Add(player);
		player.gameObject.name = "Player " + player.playerNumber;
	}

	public void HardReset() {
		//Debug.Log("HardReset");
		SoftReset();

		foreach(var player in players) {
			Destroy(player.gameObject);
		}

		AddPlayers();
	}

	public void SoftReset() {
		App.shared.timerCenter.CancelAllTimers();
		App.shared.stepCache.Reset();
		DestroyEntities();
		DestroyChaff();
		gameUnitCache.Reset();
		foreach(var player in players) {
			player.isInGame = false;
		}
	}

	void DestroyEntities() {
		foreach (var entity in new List<BoltEntity>(BoltNetwork.entities)) {
			var gameUnit = entity.gameObject.GameUnit();
			if (gameUnit == null) {
				//BoltNetwork.Destroy(entity.gameObject);
				throw new System.Exception("All entities should be game units!");
			}
			else {
				gameUnit.RemoveFromGame();
			}
		}
	}

	void DestroyChaff() {
		var preservedGameObjectNames = new List<string>(new string[]{
			"Main Camera",
			"Canvas",
			"Directional Light",
			"Battlefield",
			"CameraController",
			"KeyboardUICanvas",
			"EventSystem",
			"Network",
			"App",
			"BoltControl",
			"BoltBehaviours",
			"SocketIO",
			"InControl"
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

	/*
	void SetupTiles() {
		tiles = new List<GameObject>();

		GameObject tilePrefab = App.shared.LoadGameObject("FX/Prefabs/Tile");

		//Vector3 size = tilePrefab.GetComponent<Renderer>().bounds.size;
		float r = 5;
		int maxX = 15;
		int maxZ = 30;
		for (int x = -maxX; x < maxX+1; x ++) {
			for (int z = -maxZ; z < maxZ+1; z ++) {
				GameObject tile = Instantiate(tilePrefab);
				tile.transform.position = new Vector3(x*r, 0, z*r);
				//if (x > 0) {
				//float v = (float)Mathf.Abs(x)/(float)maxX;
				//tile.PaintDarken(1f - v*v*v*v);
				//}
				tiles.Add(tile);
			}
		}
	}
	*/

	public bool isPvP() {
		return player1.npcModeOn == false && player2.npcModeOn == false;
	}

	public bool isAIvsAI() {
		return player1.npcModeOn == true && player2.npcModeOn == true;
	}

	public bool isPvsAI() {
		return player1.isLocal == true && player2.npcModeOn == true;
	}
}
