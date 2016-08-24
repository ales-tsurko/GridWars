using UnityEngine;
using System.Collections.Generic;

public class InitialTowerState : InitialGameUnitState {
	public System.Type unitType;
}

public class Tower : GroundBuilding {

	public string activationKey;

	public static Vector3 size {
		get {
			return GameUnit.Load<Tower>().GetComponent<BoxCollider>().size;
		}
	}
		
	public string unitPrefabPath {
		get {
			return towerState.unitPrefabPath;
		}

		set {
			towerState.unitPrefabPath = value;
		}
	}

	GameObject _unitPrefab;
	public GameObject unitPrefab {
		get {
			if (_unitPrefab == null) {
				_unitPrefab = Resources.Load<GameObject>(unitPrefabPath);
			}
			return _unitPrefab;
		}
	}

	public override void MasterStart() {
		base.MasterStart();

		isStaticUnit = true;
		canAim = false;

		releaseZones = new List<ReleaseZone>();
		var concurrency = Mathf.Floor(player.powerSource.maxPower / unitPrefab.GetComponent<GameUnit>().powerCost);
		var unitSize = unitPrefab.GetComponent<BoxCollider>().size;
		var unitWidth = unitSize.x;
		var unitLength = unitSize.z;
		var unitSpacing = unitWidth/4;
		var launchZoneWidth = concurrency*(unitWidth + unitSpacing) - unitSpacing;

		for (var i = 0; i < concurrency; i ++) {
			var releaseZone = this.CreateChild<ReleaseZone>();
			var collider = releaseZone.gameObject.AddComponent<BoxCollider>();
			collider.size = unitSize;
			collider.center = new Vector3(0f, collider.size.y/2, 0f);
			collider.isTrigger = true;
			releaseZone.transform.localPosition = new Vector3(-launchZoneWidth/2 + unitWidth/2 + i*(unitWidth+unitSpacing), 0.1f, 3 + size.z/2 + unitLength/2 + unitSpacing);
			releaseZones.Add(releaseZone);
		}

		tag = "Player" + player.playerNumber;
	}

	public override void ApplyInitialState() {
		unitPrefabPath = GameUnit.PrefabPathForUnitType((initialState as InitialTowerState).unitType);
		base.ApplyInitialState(); //do this second as it resets initialState
	}

	public override void SlaveStart() {
		base.SlaveStart();

		var boltEntity = unitPrefab.GetComponent<BoltEntity>();
		if (boltEntity != null) {
			boltEntity.enabled = false;
		}

		iconObject = CreateUnit().gameObject;

		if (boltEntity != null) {
			boltEntity.enabled = true;
		}

		iconObject.transform.SetParent(transform);
		iconObject.transform.localPosition = new Vector3(0f, size.y, 0f);
		iconObject.transform.localRotation = Quaternion.identity;
		iconObject.AddComponent<GameUnitIcon>().Enable();

		if (CameraController.instance != null) {
			CameraController.instance.InitCamera (transform);
		}
	}

	public void OnMouseDown() {
		if (canQueueUnit) {
			QueueUnit();
		}
	}

	GameObject iconObject;
	float lastProductionTime = 0f;
	//int releaseLocationIndex = 0;
	int queueSize = 0;
	List<ReleaseZone> releaseZones;

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
			return player.powerSource.power >= gameUnit.powerCost;
		}
	}

	GameUnit gameUnit {
		get {
			return unitPrefab.GetComponent<GameUnit>();
		}
	}

	KeyCode keyCode {
		get {
			if (player.playerNumber <= gameUnit.buildKeyCodeForPlayers.Length) {
				return gameUnit.buildKeyCodeForPlayers[player.playerNumber - 1];
			}
			else {
				return KeyCode.None;
			}
		}
	}


	//TODO move input logic to slaves
	public override void MasterFixedUpdate () {
		base.MasterFixedUpdate();
		if (canQueueUnit) {
			if (Input.GetKeyDown(keyCode)) {
				QueueUnit();
			}
		}
			
		if (queueSize > 0) {
			ReleaseUnits();
		}
	}

	public override void SlaveFixedUpdate() {
		base.SlaveFixedUpdate();

		if (canQueueUnit) {
			player.Paint(gameObject);
			player.Paint(iconObject);
		}
		else {
			player.PaintAsDisabled(gameObject);
			player.PaintAsDisabled(iconObject);
		}
	}

	void QueueUnit() {
		queueSize ++;
		player.powerSource.power -= gameUnit.powerCost;
		lastProductionTime = Time.time;
	}

	void ReleaseUnits() {
		while (queueSize > 0 && unobstructedReleaseZone != null) {
			var releaseZone = unobstructedReleaseZone;
			var unit = CreateUnit(releaseZone.transform.position);
			releaseZone.AddObstruction(unit.GetComponent<Collider>());
			queueSize --;
		}
	}

	ReleaseZone unobstructedReleaseZone {
		get {
			foreach (var rz in releaseZones) {
				if (!rz.isObstructed) {
					return rz;
				}
			}
			return null;
		}
	}



	ITowerState towerState {
		get {
			return boltEntity.GetState<ITowerState>();
		}
	}

	GameUnit CreateUnit(Vector3 position = default(Vector3)) {
		var initialState = new InitialGameUnitState();
		if (position == default(Vector3)) {
			initialState.position = transform.position + new Vector3(0, 0.1f, 0);
		}
		else {
			initialState.position = position;
		}
		initialState.rotation = transform.rotation;
		initialState.player = player;

		var unit = unitPrefab.GetComponent<GameUnit>().Instantiate(initialState);
		unit.tag = "Player" + player.playerNumber;

		return unit;
	}

}