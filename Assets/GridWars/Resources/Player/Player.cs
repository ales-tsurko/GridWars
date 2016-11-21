using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Player : MonoBehaviour {
	public static string primaryColorMaterialName = "PrimaryColor";
	public static string secondaryColorMaterialName = "SecondaryColor";

	public Battlefield battlefield;
	public Fortress fortress;
	public float separation = 0.9f;
	public Material primaryMaterial;
	public Material secondaryMaterial;

	//https://en.wikipedia.org/wiki/Federal_Standard_595_camouflage_colours

	// human player colors

	Color[] primaryColors = new Color[]{ 
		new Color(81f/255, 0f/255, 0f/255),    // red
		new Color(22f/255, 22f/255, 191f/255) // blue
	};

	Color[] secondaryColors = new Color[]{ 
		new Color(181f/255, 181f/255, 0f/255),  // yellow
		new Color(181f/255, 181f/255, 0f/255)   // yellow
	};

	// npc colors

	Color[] npcPrimaryColors = new Color[]{ 
		new Color(81f/255, 0f/255, 0f/255),    // red
		new Color(22f/255, 22f/255, 191f/255) // blue
	};


	Color[] npcSecondaryColors = new Color[]{ 
		new Color(181f/255, 181f/255, 0f/255),  // yellow
		new Color(181f/255, 181f/255, 0f/255)   // yellow 
	};
		
	//public List<GameObject> ownedObjects;

	public int playerNumber {
		get {
			return battlefield.players.IndexOf(this) + 1;
		}
	}
		
	private bool _npcModeOn;

	public bool npcModeOn {
		get {

			return _npcModeOn;
		}

		set {
			_npcModeOn = value;
			UpdateColors();
		}
	}

	public Color primaryColor {
		get {
			if (npcModeOn) {
				return npcPrimaryColors[playerNumber - 1];
			}

			return primaryColors[playerNumber - 1];
		}
	}

	public Color secondaryColor {
		get {
			if (npcModeOn) {
				return npcSecondaryColors[playerNumber - 1];
			}
			return secondaryColors[playerNumber - 1];
		}
	}

	public PowerSource powerSource {
		get {
			return fortress.powerSource;
		}
	}

	public List<GameUnit> units;

	public bool isLocal;

	public int localNumber {
		get {
			return App.shared.battlefield.localPlayers.IndexOf(this) + 1;
		}
	}

	public bool isLocalPlayer1 {
		get {
			return localNumber == 1;
		}
	}

	public bool isLocalPlayer2 {
		get {
			return localNumber == 2;
		}
	}

	public string description {
		get {
			return (playerNumber == 1 ? "Red" : "Blue") + " Player";
		}
	}

	public InGameMenu inGameMenu;

	// MonoBehaviour

	void Awake() {
		units = new List<GameUnit>();

		if (App.shared.testEndOfGameMode) {
			separation = 0.35f;
		}
	}

	public void AddedToBattlefield() {
		primaryMaterial = new Material(Resources.Load("Materials/" + primaryColorMaterialName) as Material);
		secondaryMaterial = new Material(Resources.Load("Materials/" + secondaryColorMaterialName) as Material);
		UpdateColors();

		gameObject.transform.parent = battlefield.transform;
		gameObject.transform.rotation = Quaternion.LookRotation(Vector3.forward * ((playerNumber % 2 == 0) ? -1 : 1), Vector3.up);
		gameObject.transform.localPosition = new Vector3(0f, 0f, -separation * gameObject.transform.forward.z * battlefield.bounds.z / 2f);
	}

	// --- NPC Handicaping --------------------------------------

	public void DidWin() {
		if (IsNpcPlayingHuman()) {
			DecreaseNpcHandicap();
		}
	}

	public void DidLose() {
		if (IsNpcPlayingHuman()) {
			IncreaseNpcHandicap();
		}
	}

	public bool IsNpcPlayingHuman() {
		return npcModeOn && !opponent.npcModeOn;
	}
		
	public void DecreaseNpcHandicap() {
		SetNpcHandicap(NpcHandicap() + 0.16f);
	}

	public void IncreaseNpcHandicap() {
		SetNpcHandicap(NpcHandicap() - 0.16f);
	}

	public void SetNpcHandicap(float v) {
		App.shared.prefs.npcHandicap = Mathf.Clamp(v, 0, 1);
	}

	public float NpcHandicap() {
		return App.shared.prefs.npcHandicap;
	}

	void SetupNpcHandicap() { 
		float maxHandicap = 0.5f;
		float h = 1f - NpcHandicap() * maxHandicap;
		Debug.Log("NpcHandicap " + NpcHandicap() + " generationRateAdjustment " + h);
		fortress.powerSource.generationRateAdjustment = h;
	}
			
	// -----------------------------------------------------------------

	void UpdateColors() {
		primaryMaterial.color = primaryColor;
		secondaryMaterial.color = secondaryColor;
	}

	void OnDestroy() {
		DestroyInputs();
	}

	// Game

	public bool isInGame;

	public void StartGame() {
		if (fortress == null) { //fortress will exist for rematches
			fortress = this.CreateChild<Fortress>();
			fortress.name = "Fortress Player" + playerNumber;
			fortress.player = this;
			fortress.transform.localPosition = Vector3.zero;
			fortress.transform.localRotation = Quaternion.identity;
		}

		units = new List<GameUnit>();

		fortress.StartGame();

		isInGame = true;
		firstTutorial = null;


		if (npcModeOn) {
			SetupNpcHandicap();
		}

	}

	// Painting

	public void Paint(GameObject gameObject) {
		gameObject.PaintPrimaryColor(primaryMaterial.color);
		gameObject.PaintSecondaryColor(secondaryMaterial.color);

		/*
		gameObject.EachRenderer(r => {
			if (r.material.name.StartsWith("PrimaryColor")) {
				r.material = primaryMaterial;
			}
			else if (r.material.name.StartsWith("SecondaryColor")) {
				r.material = secondaryMaterial;
			}
		});
		*/
	}
		
	string resourcesPath {
		get {
			return "Player/" + playerNumber + "/";
		}
	}

	string ResourcePath(string resourceName) {
		return resourcesPath + resourceName;
	}


	public virtual bool IsDead() {
		return units.Count == 0;
	}

	// --- Friend / Enemy ---------------------------------------

	public virtual bool IsFriendOf(Player otherPlayer) {
		if (otherPlayer == null) {
			return false;
		}
		return playerNumber == otherPlayer.playerNumber;
	}

	public virtual bool IsEnemyOf(Player otherPlayer) {
		if (otherPlayer == null) {
			return false;
		}
		return playerNumber != otherPlayer.playerNumber;
	}

	public List<Player> enemyPlayers {
		get {
			return Battlefield.current.players.FindAll(p => p.playerNumber != playerNumber);
		}
	}

	public Player EnemyPlayer() {
		return enemyPlayers[0];
	}

	public Player opponent {
		get {
			return enemyPlayers[0];
		}
	}

	public virtual List<GameObject> EnemyObjects() {
		var enemyObjects = new List<GameObject>();
		foreach(var enemyPlayer in enemyPlayers) {
			foreach (var unit in enemyPlayer.units) {
				if (unit != null) {
					enemyObjects.Add(unit.gameObject);
				}
			}
		}
		return enemyObjects;
	}
		
	public virtual List<GameUnit> FriendlyUnitsOfType(System.Type aType) {
		return units.Where(unit => unit.IsOfType(aType)).ToList<GameUnit>();
	}

	public virtual List<GameUnit> EnemyUnitsOfType(System.Type aType) {
		return EnemyPlayer().units.Where(unit => unit.IsOfType(aType)).ToList<GameUnit>();
	}

	public List <GameUnit> UnitsTargetingObj(GameObject targetObj) {
		var unitsTargeting = new List<GameUnit>();
		foreach(var unit in units) {
			if (unit.target == targetObj) {
				unitsTargeting.Add(unit);
			}
		}
		return unitsTargeting;
	}
		
	// --- Networking ---------------------------------------

	public void TakeControlOf(GameUnit gameUnit) {
		if (isLocal || !battlefield.isInternetPVP) {
			//take control as server
			//App.shared.Log("TakeControl: " + gameUnit);
			gameUnit.entity.TakeControl();
		}
		else {
			//give control to client
			//App.shared.Log("AssignControl: " + gameUnit);
			gameUnit.entity.AssignControl(App.shared.network.connection);
		}
	}

	private int npcThinkFrequency = 20;

	/*
	thinkThrottle = new Throttle();
	thinkThrottle.behaviour = this;
	thinkThrottle.period = 25;
	*/

	public void FixedUpdate () {
		//base.ServerFixedUpdate(); 

		//|| Input.GetKey(KeyCode.Space)
		if (npcModeOn && BoltNetwork.isServer && isInGame) {
			if (App.shared.timeCounter % npcThinkFrequency == 0) {
				AI();
			}
		}
	}

	public float DistanceRatioOfFriendlyToEnemyFortress() {
		return 1f;
	}

	private bool useCostEffectiveness = true;
	private float minPowerRatio = 0.0f;

	private void SetupAI() {
		useCostEffectiveness = (playerNumber == 1);
	}

	private void AI() {
		SetupAI();

		if (isTutorialMode) {
			TutorialStep();
			return;
		}

		if (EnemyObjects().Count == 0) {
			return;
		}

		// conserve power unless enemies are near
		if (playerNumber == 2) {
			bool enemyIsCloseToBase = fortress.DistanceRatioOfClosestEnemy() < 0.4f;
			bool weAreCloseToEnemy = EnemyPlayer().fortress.DistanceRatioOfClosestEnemy() < 0.4f;

			if (enemyIsCloseToBase || weAreCloseToEnemy) {
				minPowerRatio = 0f;
			} else {
				minPowerRatio = 1f;
			}

		}

		if (powerSource.PowerRatio() >= minPowerRatio) {
			
			Tower bestTower = null;
			float bestEffectiveness = 0f;
			foreach (var tower in fortress.towers) {
				float e = useCostEffectiveness ? tower.CostBasedEffectiveness() : tower.CountBasedEffectiveness();

				if (e > bestEffectiveness) {
					bestTower = tower;
					bestEffectiveness = e;
				}
			}

			if (bestEffectiveness > 0f) {
				//float r = UnityEngine.Random.value;
				if (powerSource.IsAtMax() && 
					(bestTower.name.Contains("Tank") ||  bestTower.name.Contains("Chopper"))) {
					bestTower.SendAttemptQueueUnit(IntCoinFlip());
					bestTower.SendAttemptQueueUnit(IntCoinFlip());
					bestTower.SendAttemptQueueUnit(IntCoinFlip());
				}

				bestTower.SendAttemptQueueUnit(IntCoinFlip());
			} else if (powerSource.IsAtMax()) {
				Tower aTower = fortress.towers.PickRandom();
				if (aTower) {
					aTower.SendAttemptQueueUnit(IntCoinFlip());
				}
			}
		}
	}

	private int IntCoinFlip() {
		return UnityEngine.Random.value > .5 ? 1 : 0;
	}

	// -- Camera --------------------------------

	public Vector3 cameraDirection {
		get {
			var vectorToCamera = transform.position - App.shared.cameraController.cam.transform.position;
			Debug.Log(vectorToCamera);
			vectorToCamera = Vector3.Scale(
				vectorToCamera,
				new Vector3(1, 0, 1)
			);
			Debug.Log(vectorToCamera);
			vectorToCamera = vectorToCamera.normalized;
			Debug.Log(vectorToCamera);

			return vectorToCamera;
		}
	}

	// -- Input --------------------------------

	PlayerInputs _inputs;
	public PlayerInputs inputs {
		get {
			if (_inputs == null) {
				return App.shared.inputs; //inputs are only assigned for SharedScreenPVP.  Use defaults otherwise.
			}
			else {
				return _inputs;
			}
		}

		set {
			Debug.Log(localNumber + ": " + "Assign Inputs");
			DestroyInputs();
			_inputs = value;
		}
	}

	public void DestroyInputs() {
		if (_inputs != null) {
			App.shared.Log("DestroyInputs", this);
			_inputs.Destroy();
			_inputs = null;
		}
	}
		

	public bool isTutorialMode = false;
	private GameObject firstTutorial = null;

	void TutorialStep() {
		if (playerNumber == 1 && firstTutorial == null && App.shared.cameraController.initComplete) {
			firstTutorial = GameObject.Find("TutorialStart");
			firstTutorial.GetComponent<TutorialPart>().Begin();
		}
	}


}
