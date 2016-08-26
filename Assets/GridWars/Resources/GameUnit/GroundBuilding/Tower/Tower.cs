using UnityEngine;
using System.Collections.Generic;

public class Tower : GroundBuilding {

	public string activationKey;
	public Mesh theMesh;

	public static Vector3 size {
		get {
			/*
			Mesh mesh = GameUnit.Load<Tower>().gameObject.GetComponent<Mesh>();
			return mesh.bounds.size;
			*/

			/*
			BoxCollider bc = GameUnit.Load<Tower>().BoxCollider();
			if (bc != null ) {
				return bc.bounds.size;
			}
			*/

			return new Vector3(5f, 2f, 5f);
		}
	}
		
	public string unitPrefabPath {
		get {
			return (gameUnitState as TowerState).unitPrefabPath;
		}

		set {
			(gameUnitState as TowerState).unitPrefabPath = value;
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

	public override void SlaveStart() {
		base.SlaveStart();

		//*
		iconObject = CreateUnit().gameObject;

		iconObject.transform.SetParent(transform);
		iconObject.transform.localPosition = new Vector3(0f, size.y, 0f);
		iconObject.transform.localRotation = Quaternion.identity;
		iconObject.AddComponent<GameUnitIcon>().Enable();
		//*/

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

	GameUnit CreateUnit(Vector3 position = default(Vector3)) {
		var prefabUnit = unitPrefab.GetComponent<GameUnit>();

		var initialState = new GameUnitState(prefabUnit);
		initialState.player = player;

		var unit = prefabUnit.Instantiate(
			position == default(Vector3) ? transform.position + new Vector3(0, 0.1f, 0) : position,
			transform.rotation,
			initialState);
		unit.tag = "Player" + player.playerNumber;

		return unit;
	}

}