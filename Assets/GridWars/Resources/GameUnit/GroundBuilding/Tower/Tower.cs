using UnityEngine;
using System.Collections.Generic;

public class Tower : GroundBuilding {

	public string activationKey;

	public static Vector3 size {
		get {
			return GameUnit.Load<Tower>().GetComponent<BoxCollider>().size;
		}
	}
		
	public string unitPrefabPath {
		get {
			return ((TowerProtocolToken)entity.attachToken).unitPrefabPath;
		}
	}

	public GameObject unitPrefab;

	public override void Attached() {
		base.Attached();

		Setup();
	}

	public void Setup() {
		unitPrefab = Resources.Load<GameObject>(unitPrefabPath);

		var boltEntity = unitPrefab.GetComponent<BoltEntity>();
		if (boltEntity != null) {
			boltEntity.enabled = false;
		}

		try {
			iconUnit = CreateUnit();
		}
		finally {
			if (boltEntity != null) {
				boltEntity.enabled = true;
			}
		}

		iconUnit.transform.SetParent(transform);
		iconUnit.transform.localPosition = new Vector3(0f, size.y, 0f);
		iconUnit.transform.localRotation = Quaternion.identity;
		iconUnit.gameObject.AddComponent<GameUnitIcon>().Enable();

		canAim = false;

		if (BoltNetwork.isServer) {
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

		if (CameraController.instance != null) {
			CameraController.instance.InitCamera (transform);
		}
	}

	public void OnMouseDown() {
		if (canQueueUnit) {
			QueueUnit();
		}
	}

	GameUnit iconUnit;
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

	void Update () {
		if (canQueueUnit) {
			player.Paint(gameObject);
			player.Paint(iconUnit.gameObject);
			if (Input.GetKeyDown(keyCode)) {
				QueueUnit();
			}
		}
		else {
			player.PaintAsDisabled(gameObject);
			player.PaintAsDisabled(iconUnit.gameObject);
		}
			
		if (queueSize > 0) {
			ReleaseUnits();
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
		var unit = unitPrefab.GetComponent<GameUnit>().Instantiate<GameUnit>(prototype => {
			if (position == default(Vector3)) {
				prototype.transform.position = transform.position + new Vector3(0, 0.1f, 0);
			}
			else {
				prototype.transform.position = position;
			}

			prototype.transform.rotation = transform.rotation;
			prototype.player = player;
		});

		unit.tag = "Player" + player.playerNumber;

		return unit;
	}

}