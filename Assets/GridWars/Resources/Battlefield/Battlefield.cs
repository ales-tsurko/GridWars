using UnityEngine;
using System.Collections.Generic;
using PvEConfig;

public class Battlefield : MonoBehaviour {
	public static Battlefield current { //TODO: get rid of this
		get {
			return App.shared.battlefield;
		}
	}

	public Vector3 bounds = new Vector3(100f, 10f, 100f);
	public List<Player> players;
	public GameUnitCache gameUnitCache;

	public bool isPaused;

	public Vector3 fortressBounds {
		get {
			if (players.Count > 0) {
				if (player1.fortress == null) {
					return Vector3.zero;
				}
				else {
					return new Vector3(
						player1.fortress.bounds.x,
						1f,
						(player2.fortress.transform.position.z - player1.fortress.transform.position.z) + player1.fortress.bounds.z
					);
				}
			}
			else {
				return Vector3.zero;
			}
		}
	}

	public Vector3[] fortressCorners {
		get {
			return new Vector3[] {
				new Vector3(-fortressBounds.x/2, 0f, -fortressBounds.z/2),
				new Vector3(-fortressBounds.x/2, 0f, fortressBounds.z/2),
				new Vector3(fortressBounds.x/2, 0f, -fortressBounds.z/2),
				new Vector3(fortressBounds.x/2, 0f, fortressBounds.z/2)
			};
		}
	}

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

	public bool GameOver() {
		return livingPlayers.Count == 1;
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
    public bool isPvELadder;
	public bool canCheckGameOver; //don't check game over until a unit is received from server

    public bool hasStarted = false;

    public int pveLadderLevel;

	public void Awake() {
		gameUnitCache = new GameUnitCache();
	}

	public void Start() {
		App.shared.enabled = true; //Load App so Start gets called

	}

	/*
	void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			isPaused = !isPaused;
		}
	}
	*/

	public void AddPlayers() {
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
        if (isPvELadder) {
            SetupForPvELadder();
        }
	}

	void AddPlayer() {
		var player = this.CreateChild<Player>();
		player.battlefield = this;
		players.Add(player);
		player.gameObject.name = "Player " + player.playerNumber;
		player.AddedToBattlefield();
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
		//App.shared.timerCenter.CancelAllTimers();
		foreach(var player in players) {
			player.isInGame = false;
		}
		App.shared.stepCache.Reset();
		DestroyEntities();
		DestroyChaff();
		gameUnitCache.Reset();

		Resources.UnloadUnusedAssets();
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
			"Matchmaker",
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

	public bool isPlayingGame {
		get {
			return App.shared.state is PlayingGameState || App.shared.state is PostGameState;
		}
	}

    public GameType GetGameType(){
        if (isInternetPVP) {
            return GameType.InternetPVP;
        }
        if (isPvELadder) {
            return GameType.PvELadder;
        }
        if (isPvP() && !isInternetPVP) {
            return GameType.SharedScreenPVP;
        }
        if (isAIvsAI()) {
			if (player1.isTutorialMode) {
				return GameType.Tutorial;
			}
			else {
				return GameType.AIvsAI;
			}
        }
        if (!isPvP() && !isAIvsAI()) {
            return GameType.PlayervsAI;
        }
        return GameType.Unknown;
    }
    [HideInInspector]
    PvELadderLevelConfig pveConfig;
    [HideInInspector]
    UIMenu PvEMenu;

	CampaignLevel campaignLevel;

    void SetupForPvELadder(){
		campaignLevel = CampaignLevel.Load(pveLadderLevel);
		campaignLevel.Setup();

		pveConfig = campaignLevel.config;
        //power rate
        player1.powerSource.generationRate = pveConfig.playerPowerRate;
        player1.powerSource.generationRateAdjustment = 1;
        player2.powerSource.generationRate = pveConfig.cpuPowerRate;
        player2.powerSource.generationRateAdjustment = 1;
        if (pveConfig.isBossLevel) {
            print("BOSS");
            var unit = pveConfig.boss.prefab.Instantiate();
            unit.player = player2;
            unit.name = "BOSS";
           // unit.releaseZone = player2.fortress.towers[2].transform.position;
            unit.transform.position = player2.fortress.towers[2].transform.position + (player2.fortress.towers[2].transform.forward * 12);
            unit.transform.rotation = player2.fortress.towers[2].transform.rotation;
            pveConfig.AdjustBossUnit(unit);
        }
        Time.timeScale = pveConfig.gameSpeed;

		campaignLevel.Begin();
        //
    }

    public void AdjustUnitForPvELadder(GameUnit _unit, int _playerNumber){
        print(_playerNumber);
        pveConfig.AdjustUnit(_unit, _playerNumber);
        print("Unit Adjusted");
    }

    public void ResetPvEMenu(){
        PvEMenu.Reset();
    }

	/*
	void OnDrawGizmos() {
		Gizmos.color = new Color(0, 0, 0, 0.5f);
		Gizmos.DrawCube(transform.position, new Vector3(fortressBounds.x, 1f, fortressBounds.z));
	}
	//*/
}

public enum GameType { Unknown, InternetPVP, SharedScreenPVP, AIvsAI, PlayervsAI, Tutorial, PvELadder }

