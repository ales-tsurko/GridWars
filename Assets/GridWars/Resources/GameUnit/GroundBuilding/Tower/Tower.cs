﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class Tower : GroundBuilding, CameraControllerDelegate, KeyDelegate {

	public GameObject iconPlacement;

	//public Mesh theMesh;
	[HideInInspector]
	public KeyCode attemptQueueUnitKeyCode = KeyCode.None;
    public string unitKeyMap = "None";
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
				_unitPrefab = Resources.Load<GameObject>(unitPrefabPath);
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
	}

	public override void ServerJoinedGame() {
		base.ServerJoinedGame();

		isStaticUnit = true;

		releaseZones = new List<ReleaseZone>();
		var concurrency = Mathf.Ceil(player.powerSource.maxPower / unitPrefab.GameUnit().powerCost);
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
				new Vector3(0f, size.y/2 + unitSize.y/2 + unitSpacing, size.z/2 + unitLength/2 + unitSpacing),
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

        unitKeyMap = iconUnit.GetComponent<GameUnit>().GetType().ToString() + player.localNumber;
        attemptQueueUnitKeyCode = unitKeyMap.GetKey();
		//Keys.data.TryGetValue(iconUnit.GetComponent<GameUnit>().GetType().ToString() + player.localNumber, out attemptQueueUnitKeyCode); //assigns KeyCode from string - dictionary is editable for remapping keys

        keyIcon.GetComponentInChildren<TextMesh>().text = attemptQueueUnitKeyCode.ToString().FormatForKeyboard();

		keyIcon.EachRenderer(r => r.enabled = true);//GameUnit.SetVisibileAndEnabled(true) isn't working for some reason.

		//Debug.Log(player.playerNumber + ": " + gameUnit.GetType() + ": " + attemptQueueUnitKeyCode.ToString());

		App.shared.keys.AddKeyDelegate(unitKeyMap, this);
	}

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

	public override void ServerAndClientUpdate() {
		base.ServerAndClientUpdate();

		iconObject.SetActive(canQueueUnit);
		keyIcon.SetActive(attemptQueueUnitKeyCode != KeyCode.None && player.isLocal && prefs.keyIconsVisible);
	}


	public override void ServerLeftGame() {
		base.ServerLeftGame();

		if (player != null) {
			player.fortress.TowerDied(this);
		}
	}

	public override void ServerAndClientLeftGame() {
		base.ServerAndClientLeftGame();

		App.shared.cameraController.cameraControllerDelegates.Remove(this);
	}

	public override void RemoveFromGame() {
		base.RemoveFromGame();

		App.shared.keys.RemoveKeyDelegate(unitKeyMap, this);
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
		try {
			nextUnitVeteranLevel = e.veteranLevel;
			if (canQueueUnit) {
				QueueUnit();
			}
		}
		finally {
			nextUnitVeteranLevel = 0;
		}
	}

	public float lastProductionTime = 0f;
	//int releaseLocationIndex = 0;
	List<int>unitWithVeterancyQueue; //value is veterancy
	List<ReleaseZone> releaseZones;
	int nextUnitVeteranLevel; //used for power calculations

	bool canQueueUnit {
		get {
			return hasCooledDown && hasEnoughPower;
		}
	}

	bool hasCooledDown {
		get {
			return Time.time >= lastProductionTime + gameUnit.cooldownSeconds;
		}
	}

	bool hasEnoughPower {
		get {
			float powerCost = float.MaxValue;
			if (nextUnitVeteranLevel == 0) {
				powerCost = gameUnit.powerCost;
			}
			else if (nextUnitVeteranLevel == 1) {
				powerCost = player.powerSource.maxPower; //TODO: move this to GameUnit
			}

			return player.powerSource.power >= powerCost;
		}
	}

	GameUnit gameUnit {
		get {
			return unitPrefab.GetComponent<GameUnit>();
		}
	}

	float mouseDownStart;

	void OnMouseDown() {
		mouseDownStart = Time.time;
	}

	void OnMouseUp() {
		if (Time.time - mouseDownStart >= App.shared.keys.longPressDuration) {
			if (!npcModeOn) {
				SendAttemptQueueUnit(1);
			}
		}
	}

	public override void QueuePlayerCommands() {
		base.QueuePlayerCommands();
	}

	public void SendAttemptQueueUnit(int veteranLevel = 0) {
		if (entity.hasControl) {
			var queueEvent = AttemptQueueUnitEvent.Create(entity);
			queueEvent.veteranLevel = veteranLevel;
			queueEvent.Send();
		}
	}

	void QueueUnit() {
		if (!BoltNetwork.isServer) {
			throw new System.Exception("Use SendAttemptQueueUnit from the client");
		}
		unitWithVeterancyQueue.Add(nextUnitVeteranLevel);
		player.powerSource.power -= gameUnit.powerCost;
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

	public float aiStyle = 0;

	public float Effectiveness() {
		float a = CountOfEnemyUnitsWeCanCounter();
		float b = CountOfEnemyUnitsThatCounterUs();
		float c = CountOfTowerUnits();

		float cost = gameUnit.powerCost / player.powerSource.maxPower;

		float e = 0;
		//if (aiStyle > .5) {
		if (true) {
			e = ( (a - c) / (1 + b) ) / cost;
		}/* else {
			e = (1.5f * a - b) / cost;
		}*/

		return e;
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

	// KeyDelegate

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
}