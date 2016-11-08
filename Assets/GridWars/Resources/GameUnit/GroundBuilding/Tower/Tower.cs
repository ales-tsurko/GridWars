using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using InControl;

public class Tower : GroundBuilding, CameraControllerDelegate, KeyDelegate {

	public GameObject iconPlacement;

	//public Mesh theMesh;
	[HideInInspector]

	// for death from tanker hit explosion fx
	public bool dieWithBlockify {
		get {
			return entity.GetState<ITowerState>().dieWithBlockify;
		}

		set {
			entity.GetState<ITowerState>().dieWithBlockify = value;
		}
	}

    // for warp in fx
	public bool isWarpedIn {
		get {
			return entity.GetState<ITowerState>().isWarpedIn;
		}

		set {
			entity.GetState<ITowerState>().isWarpedIn = value;
		}
	}

	public GameObject cube;

	public bool npcModeOn {
		get {
			return player.npcModeOn;
		}
	}

	//public GameObject topComponent;
	//public GameObject baseComponent;
	private GameUnit iconUnit;

	public static Vector3 size {
		get {
			/*
			Mesh mesh = GameUnit.Load<Tower>().gameObject.GetComponent<Mesh>();
			return mesh.bounds.size;
			*/

			//float y = GameUnit.Load<Tower>().GetComponent<Collider>().bounds.size.y;

			return new Vector3(5f, 1.5f, 5f);
		}
	}
		
	public string unitPrefabPath {
		get {
			return entity.GetState<ITowerState>().unitPrefabPath;
		}

		set {
			entity.GetState<ITowerState>().unitPrefabPath = value;
		}
	}

	GameObject _unitPrefab;
	public GameObject unitPrefab {
		get {
			if (_unitPrefab == null) {
				_unitPrefab = App.shared.LoadGameObject(unitPrefabPath);
				_unitPrefab.GetComponent<GameUnit>().Awake();
			}
			return _unitPrefab;
		}
	}

	// NetworkedGameUnit

	public override void ServerInit() {
		maxHitPoints = 50f;
		base.ServerInit();

		aiStyle = UnityEngine.Random.value;
		unitWithVeterancyQueue = new List<int>();
	}


	public override void ClientInit() {
		shouldDestroyColliderOnClient = false;
		base.ClientInit();
	}

	public override void ServerAndClientInit() {
		base.ServerAndClientInit();

		prefs = App.shared.prefs; //perf opt

		if (App.shared.testEndOfGameMode) {
			hitPoints = 1f;
		}

		keyIcon.SetActive(false);

		App.shared.cameraController.cameraControllerDelegates.Add(this);

		gameUnitState.AddCallback("isWarpedIn", IsWarpedInChanged);

	}

	public override void ServerJoinedGame() {
		base.ServerJoinedGame();

		isStaticUnit = true;

		releaseZones = new List<ReleaseZone>();
		var concurrency = Mathf.Ceil(player.powerSource.maxPower / unitPrefab.GameUnit().PowerCost(0));
		var unitSize = unitPrefab.GetComponent<BoxCollider>().size;
		var unitWidth = unitSize.x;
		var unitLength = unitSize.z;
		var unitSpacing = unitWidth/6;
		var launchZoneWidth = concurrency*(unitWidth + unitSpacing) - unitSpacing;

		for (var i = 0; i < concurrency; i ++) {
			var releaseZone = this.CreateChild<ReleaseZone>();
			releaseZone.size = unitSize;
			releaseZone.transform.localPosition = new Vector3(-launchZoneWidth/2 + unitWidth/2 + i*(unitWidth+unitSpacing), 0.05f, 0f);

			releaseZone.transform.Translate(Vector3.Scale(
				new Vector3(0f, size.y/2 + unitSize.y/2 + unitSpacing, size.z/2 + unitLength/2 + unitSpacing * 2f),
				unitPrefab.GetComponent<GameUnit>().launchDirection
			));

			releaseZones.Add(releaseZone);
		}

		tag = "Player" + player.playerNumber;

		entity.AddEventCallback<AttemptQueueUnitEvent>(ReceiveAttemptQueueUnit);
	}

	public override void ServerAndClientJoinedGame() {
		base.ServerAndClientJoinedGame();

		iconUnit = Instantiate(unitPrefab).GetComponent<GameUnit>();
		iconUnit.BecomeIcon();

		iconObject = iconUnit.gameObject;
		iconObject.transform.SetParent(transform);
		iconObject.transform.localPosition = new Vector3(0f, iconPlacement.transform.position.y, 0f);
		iconObject.transform.localRotation = Quaternion.identity;
		player.Paint(iconObject);

		keyIcon.EachRenderer(r => r.enabled = true);

        keyIcon.SetActive(player.isLocal && prefs.keyIconsVisible);

        gameObject.name = "Tower Player" + player.playerNumber + " " + unitPrefab.name;
        if (player.inputs.LastInputType == BindingSourceType.None) {
            player.inputs.LastInputType = BindingSourceType.KeyBindingSource;
        }
        WriteHotKeysBasedOnInput();

		HideMesh();
	}

	// hide / unhide for warp in

	public void IsWarpedInChanged() {
		if (isWarpedIn) {
			UnhideMesh();
		}
	}

	public void UnhideIn(float dt) {
		App.shared.timerCenter.NewTimer().SetTimeout(dt).SetTarget(this).SetMethod("Unhide").Start();
	}

	public void Unhide() {
		isWarpedIn = true;
		// bolt will now call IsWarpedInChanged() on server and client
	}

	// hide / unhide mesh

	public void HideMesh() {
		cube.GetComponent<MeshRenderer>().enabled = false;
	}

	public void UnhideMesh() {
		MeshRenderer mr = cube.GetComponent<MeshRenderer>();
		if (mr.enabled == false) {
			mr.enabled = true;
			var fader = cube.AddComponent<BrightFadeInGeneric>();
			fader.period = 0.35f;
			App.shared.PlayAppSoundNamedAtVolume("TowerBirth", 0.3f);
			fader.OnEnable();
		}
	}

	// ---------------------------

	public override void ServerFixedUpdate () {
		//base.ServerFixedUpdate(); TODO: extract another class from GameUnit so we don't have to perform this perf opt.

		//NpcStep();

		if (unitWithVeterancyQueue.Count > 0) {
			ReleaseUnits();
		}

	}

	public void LaunchWithChance(float chance) { // chance out of 1
		if (Random.value < chance) {
			SendAttemptQueueUnit();
		}
	}

	BindingSourceType lastInputType = BindingSourceType.None;

	public override void ServerAndClientUpdate() {
		base.ServerAndClientUpdate();
        if (player.inputs.LastInputType != lastInputType) {
            WriteHotKeysBasedOnInput();
        }

        iconObject.SetActive(CanQueueUnit(0));
		
	}


	public override void ServerLeftGame() {
		base.ServerLeftGame();

		if (player != null && player.fortress != null) {
			player.fortress.TowerDied(this);
		}
	}

	public override void ServerAndClientLeftGame() {
		base.ServerAndClientLeftGame();

		App.shared.cameraController.cameraControllerDelegates.Remove(this);
	}

	// HUD

	GameObject iconObject;
	public GameObject keyIcon;
	Prefs prefs;

	// CameraController

	public void CameraControllerBeganTransition() {
		keyIcon.transform.rotation = Quaternion.Euler(App.shared.cameraController.keyIconRotation.rotation);
	}

	public override void Think() {
		// doesn't need to pick targets
	}

	public void ReceiveAttemptQueueUnit(AttemptQueueUnitEvent e) {
		if (CanQueueUnit(e.veteranLevel)) {
			QueueUnit(e.veteranLevel);
		}
	}

	public float lastProductionTime = 0f;
	//int releaseLocationIndex = 0;
	List<int>unitWithVeterancyQueue; //value is veterancy
	List<ReleaseZone> releaseZones;
	int nextUnitVeteranLevel; //used for power calculations

	bool CanQueueUnit(int level) {
		return hasCooledDown && HasEnoughPower(level);
	}

	bool hasCooledDown {
		get {
			return Time.time >= lastProductionTime + gameUnit.cooldownSeconds;
		}
	}

	bool HasEnoughPower(int level) {
		return player.powerSource.power >= gameUnit.PowerCost(level);
	}

	GameUnit gameUnit {
		get {
			return unitPrefab.GetComponent<GameUnit>();
		}
	}

	// Input


	float longPressDuration = 0.5f;
	float releaseUnitDownTime;

	bool passedLongPress {
		get {
			return Time.time - releaseUnitDownTime >= longPressDuration;
		}
	}

	void ReleaseUnitDown() {
		if (!npcModeOn) {
			releaseUnitDownTime = Time.time;
		}
	}

	void ReleaseUnitUp() {
		if (!npcModeOn && releaseUnitDownTime != 0f && !passedLongPress) {
			SendAttemptQueueUnit(0);
		}
		releaseUnitDownTime = 0f;
	}

	void ReleaseUnitUpdate() {
		if (releaseUnitDownTime > 0f && passedLongPress && HasEnoughPower(1)) {
			releaseUnitDownTime = 0f;
			SendAttemptQueueUnit(1);
		}
	}


	//TODO: don't allow keyboard player to release units for other side?
	void OnMouseDown() {
		ReleaseUnitDown();
	}

	void OnMouseUp() {
		ReleaseUnitUp();
	}

	PlayerAction releaseAction {
		get {
			return player.inputs.GetPlayerActionByName(unitPrefab.name);
		}
	}

	public override void QueuePlayerCommands() {
		base.QueuePlayerCommands();

		
			if (releaseAction.WasPressed) {
				ReleaseUnitDown();
			}

			if (releaseAction.WasReleased) {
				ReleaseUnitUp();
			}
		

		ReleaseUnitUpdate();
	}

	public void SendAttemptQueueUnit(int veteranLevel = 0) {
		if (isInGame && entity.hasControl && CanQueueUnit(veteranLevel)) {
			var queueEvent = AttemptQueueUnitEvent.Create(entity);
			queueEvent.veteranLevel = veteranLevel;
			queueEvent.Send();
		}
	}

	void QueueUnit(int veteranLevel) {
		if (!BoltNetwork.isServer) {
			throw new System.Exception("Use SendAttemptQueueUnit from the client");
		}
		unitWithVeterancyQueue.Add(veteranLevel);
		player.powerSource.power -= gameUnit.PowerCost(veteranLevel);
		lastProductionTime = Time.time;
	}

	void ReleaseUnits() {
		while (unitWithVeterancyQueue.Count > 0 && unobstructedReleaseZone != null) {
			var releaseZone = unobstructedReleaseZone;

			var unit = unitPrefab.GameUnit().Instantiate();

			unit.player = player;

			releaseZone.hiddenUnit = unit;
			unit.releaseZone = releaseZone;

			unit.transform.position = releaseZone.transform.position;
			unit.transform.rotation = transform.rotation;

			if (unitWithVeterancyQueue[0] == 1) {
				unit.UpgradeVeterancy();
			}
			unitWithVeterancyQueue.RemoveAt(0);
		}
	}

	ReleaseZone unobstructedReleaseZone {
		get {
			var shuffledZones = releaseZones.OrderBy(a => UnityEngine.Random.value);

			foreach (var rz in shuffledZones) {
				if (!rz.isObstructed) {
					return rz;
				}
			}
			return null;
		}
	}

	// --- AI ------------------------------


	// count effectiveness

	public int CountOfTowerUnits() {
		return player.FriendlyUnitsOfType(iconUnit.GetType()).Count;
	}


	public int CountOfEnemyUnitsWeCanCounter() {
		int total = 0;

		foreach(var counterType in iconUnit.CountersTypes()) {
			int count = player.EnemyUnitsOfType(counterType).Count;
			total += count;
		}

		return total;
	}

	public int CountOfEnemyUnitsThatCounterUs() {
		int total = 0;

		foreach(GameUnit unit in EnemyUnits()) {
			if (unit != null && unit.CountersTypes().Contains(iconUnit.GetType())) {
				total += 1;
			}
		}

		return total;
	}

	// cost effectiveness

	public float CostOfTowerUnits() {
		float cost = 0;
		foreach (var unit in player.FriendlyUnitsOfType(iconUnit.GetType())) {
			cost += unit.PowerCost(unit.veteranLevel) * unit.hpRatio;
		}

		return cost;
	}
	public float CostOfEnemyUnitsWeCanCounter() {
		float cost = 0;

		foreach(var counterType in iconUnit.CountersTypes()) {
			foreach (var unit in player.EnemyUnitsOfType(counterType)) {
				//float dr = Mathf.Sqrt(1f - unit.RatioOfDistanceToEnemyFortress()*0.5f);
				//float dr = 1f/(1f + unit.RatioOfDistanceToEnemyFortress());
				float dr = 1f;
				cost += unit.PowerCost(unit.veteranLevel) * unit.hpRatio * dr;
			}
		}

		return cost;
	}
		
	public float CostOfEnemyUnitsThatCounterUs() {
		float cost = 0;

		foreach(GameUnit unit in EnemyUnits()) {
			if (unit != null && unit.CountersTypes().Contains(iconUnit.GetType())) {
				//float dr = Mathf.Sqrt(1f - unit.RatioOfDistanceToEnemyFortress()*0.5f);
				//float dr = 1f/(1f + unit.RatioOfDistanceToEnemyFortress());
				float dr = 1f;
				cost += unit.PowerCost(unit.veteranLevel) * unit.hpRatio * dr;
			}
		}

		return cost;
	}

	public float aiStyle = 0;

	public float Effectiveness() {
		float wc = 0;
		float cu = 0;
		float c  = 0;
		float e = 0;
		float unitCost = gameUnit.PowerCost(gameUnit.veteranLevel) / player.powerSource.maxPower;

		if (player.playerNumber == 1) {
			wc = CountOfEnemyUnitsWeCanCounter();
			cu = CountOfEnemyUnitsThatCounterUs();
			//c  = CountOfTowerUnits();
			//float e = ( (wc - c) / (1 + cu) ) / unitCost;
			e = ( (wc) / (1f + cu) ) / unitCost;
		} else {
			wc = CostOfEnemyUnitsWeCanCounter();
			cu = CostOfEnemyUnitsThatCounterUs();
			c  = CostOfTowerUnits();
			e = ( (wc - c/2f) / (1f + cu) ) / unitCost;
		}

		return e;
	}

	// KeyDelegate

	bool inGameMenuIsFocused {
		get {
			if (App.shared.state.inheritsFrom(typeof(PlayingGameState))) {
				return (App.shared.state as PlayingGameState).InGameMenuForPlayer(player).hasFocus;
			}
			else {
				return false;
			}
		}
	}

	public void KeyPressed() {
		if (!npcModeOn) {
			SendAttemptQueueUnit();
		}
	}

	public void KeyLongPressed() {
		if (!npcModeOn) {
			SendAttemptQueueUnit(1);
		}
	}


	public void DieWithBlockify() {
		dieWithBlockify = true;
	}

	public override GameObject ShowFxExplosion() {
		var explosion = base.ShowFxExplosion();

		if (dieWithBlockify) {
			Blockify b = explosion.GetComponent<Blockify>();
			b.enabled = true;
		}

		return explosion;
	}

	public void NpcStep () {

		if (npcModeOn) {
			if (player.powerSource.PowerRatio() > 0.3f) {
				if (Random.value < 0.001f * Effectiveness()) {
					SendAttemptQueueUnit();
				} else if (player.powerSource.IsAtMax()) {
					LaunchWithChance(0.002f);
				}
			} 
		}
	}

    public void UpdateHotKeys(){
        keyIcon.SetActive(player.isLocal && prefs.keyIconsVisible);
    }

    public void WriteHotKeysBasedOnInput() {
        lastInputType = player.inputs.LastInputType;
        foreach (var binding in releaseAction.Bindings) {
            if (binding.BindingSourceType == lastInputType) {
                var textMesh = keyIcon.GetComponentInChildren<TextMesh>();
                textMesh.text = binding.HotkeyDescription();
                if (textMesh.text == "△") {
                    textMesh.transform.localScale = new Vector3(0.065f, 0.065f, 0.065f);
                    textMesh.transform.localPosition = new Vector3(0.054f, 0f, -0.18f);
                }
                else if (textMesh.text == "▢") {
                    textMesh.transform.localScale = new Vector3(0.065f, 0.065f, 0.065f);
                    textMesh.transform.localPosition = new Vector3(0.032f, 0f, -0.18f);
                }
                else if (textMesh.text == "◯") {
                    textMesh.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                    textMesh.transform.localPosition = new Vector3(0.028f, 0f, -0.18f);
                }
                else {
                    textMesh.transform.localScale = new Vector3(0.038f, 0.038f, 0.038f);
                    textMesh.transform.localPosition = new Vector3(0f, 0f, -0.18f);
                }
            }
        }
    }

}