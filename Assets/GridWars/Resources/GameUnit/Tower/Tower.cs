using UnityEngine;
using System.Collections.Generic;

public class Tower : GameUnit {
	public static Vector3 size {
		get {
			return GameUnit.Load<Tower>().GetComponent<BoxCollider>().size;
		}
	}

	public GameObject unitPrefab;


	public override void Start () {
		isStaticUnit = true;

		base.Start();
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
			releaseZone.transform.localPosition = new Vector3(-launchZoneWidth/2 + unitWidth/2 + i*(unitWidth+unitSpacing), 0.1f, size.z/2 + unitLength/2 + unitSpacing);
			releaseZones.Add(releaseZone);
		}

		iconUnit = CreateUnit();
		iconUnit.transform.parent = transform;
		iconUnit.transform.localPosition = new Vector3(0f, size.y, 0f);
		iconUnit.transform.localRotation = Quaternion.identity;
		iconUnit.GetComponent<GameUnit>().enabled = false;
		iconUnit.GetComponent<GameUnitIcon>().enabled = true;
	}

	public void OnMouseDown() {
		if (canQueueUnit) {
			QueueUnit();
		}
	}

	GameUnit iconUnit;
	float lastProductionTime = 0f;
	int releaseLocationIndex = 0;
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

	void Update () {
		if (canQueueUnit) {
			player.Paint(gameObject);
			player.Paint(iconUnit.gameObject);
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
			var unit = CreateUnit();
			var releaseZone = unobstructedReleaseZone;
			unit.transform.position = releaseZone.transform.position;
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

	GameUnit CreateUnit() {
		var unitObject = Instantiate(unitPrefab);
		unitObject.transform.position = transform.position + new Vector3(0, 0.1f, 0);
		unitObject.transform.rotation = transform.rotation;

		var gameUnit = unitObject.GetComponent<GameUnit>();
		gameUnit.player = player;
		gameUnit.tag = "Player" + player.playerNumber;
		return gameUnit;
	}

}