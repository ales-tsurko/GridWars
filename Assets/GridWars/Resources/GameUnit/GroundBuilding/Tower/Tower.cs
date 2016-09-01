using UnityEngine;
using System.Collections.Generic;

public class Tower : GroundBuilding {

	public string activationKey;
	//public Mesh theMesh;

	bool npcModeOn = false;
	public GameObject topComponent;
	public GameObject baseComponent;

	public static Vector3 size {
		get {
			/*
			Mesh mesh = GameUnit.Load<Tower>().gameObject.GetComponent<Mesh>();
			return mesh.bounds.size;
			*/

			//float y = GameUnit.Load<Tower>().GetComponent<Collider>().bounds.size.y;

			return new Vector3(5f, 2f, 5f);
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
			}
			return _unitPrefab;
		}
	}

	public override void ServerAndClientInit() {
		base.ServerAndClientInit();
	}

	public override void ServerAndClientJoinedGame() {
		base.ServerAndClientJoinedGame();

		var iconUnit = Instantiate(unitPrefab).GetComponent<GameUnit>();
		iconUnit.BecomeIcon();

		iconObject = iconUnit.gameObject;
		iconObject.transform.SetParent(transform);
		iconObject.transform.localPosition = new Vector3(0f, size.y, 0f);
		iconObject.transform.localRotation = Quaternion.identity;

		if (CameraController.instance != null) {
			CameraController.instance.InitCamera (transform);
		}

		StartPaint();
	}

	public override void ServerJoinedGame() {
		base.ServerJoinedGame();

		isStaticUnit = true;

		releaseZones = new List<ReleaseZone>();
		var concurrency = Mathf.Floor(player.powerSource.maxPower / unitPrefab.GameUnit().powerCost);
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

		entity.AddEventCallback<AttemptQueueUnitEvent>(AttemptQueueUnit);
	}

	public override void ClientInit() {
		shouldDestroyColliderOnClient = false;
		base.ClientInit();
	}

	public override void Think() {
		// doesn't need to pick targets
	}

	public void AttemptQueueUnit(AttemptQueueUnitEvent e) {
		AttemptQueueUnit();
	}

	public void AttemptQueueUnit() {
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

	KeyCode attemptQueueUnitKeyCode {
		get {
			if (player.playerNumber <= gameUnit.buildKeyCodeForPlayers.Length) {
				return gameUnit.buildKeyCodeForPlayers[player.playerNumber - 1];
			}
			else {
				return KeyCode.None;
			}
		}
	}


	//TODO move input logic to slavesfkey
	public override void ServerFixedUpdate () {
		base.ServerFixedUpdate();
			
		if (queueSize > 0) {
			ReleaseUnits();
		}

		if (npcModeOn) {
			if (Random.value < 0.001) {
				AttemptQueueUnit();
			}
		}
	}
		
	int paintMode = 0;

	public override void ServerAndClientFixedUpdate() {
		base.ServerAndClientFixedUpdate();

		if (canQueueUnit) {
			Paint();
		}
		else {
			PaintAsDisabled();
		}
	}

	public void StartPaint() {
		player.Paint(topComponent);
		player.Paint(baseComponent);
		player.Paint(iconObject);
	}

	public void Paint() {
		if (paintMode != 1) {
			paintMode = 1;
			/*
			player.Paint(gameObject);
			player.Paint(iconObject);
			*/
			player.PaintAsHighlighted(topComponent);

		}
	}

	public void PaintAsDisabled() {
		if (paintMode != 2) {
			paintMode = 2;
			/*
			player.PaintAsDisabled(gameObject);
			player.PaintAsDisabled(iconObject);
			*/
			player.Paint(topComponent);

		}
	}

	public void OnMouseDown() {
		if (entity.hasControl) {
			AttemptQueueUnitEvent.Create(entity).Send();
		}
	}

	public override void QueuePlayerCommands() {
		base.QueuePlayerCommands();

		if (Input.GetKeyDown(attemptQueueUnitKeyCode)) {
			AttemptQueueUnitEvent.Create(entity).Send();
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

			var unit = unitPrefab.GameUnit().Instantiate();
			unit.player = player;
			unit.transform.position = releaseZone.transform.position;
			unit.transform.rotation = transform.rotation;

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
}