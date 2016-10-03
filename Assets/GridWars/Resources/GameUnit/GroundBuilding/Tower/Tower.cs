using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class Tower : GroundBuilding {

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

	public override void ClientInit() {
		shouldDestroyColliderOnClient = false;
		base.ClientInit();
	}

	public override void ServerAndClientInit() {
		base.ServerAndClientInit();

		prefs = App.shared.prefs; //perf opt
		//hitPoints = 1f;
	}

	public override void ServerJoinedGame() {
		base.ServerJoinedGame();

		isStaticUnit = true;

		releaseZones = new List<ReleaseZone>();
		var concurrency = Mathf.Ceil(player.powerSource.maxPower / unitPrefab.GameUnit().powerCost);
		var unitSize = unitPrefab.GetComponent<BoxCollider>().size;
		var unitWidth = unitSize.x;
		var unitLength = unitSize.z;
		var unitSpacing = unitWidth/4;
		var launchZoneWidth = concurrency*(unitWidth + unitSpacing) - unitSpacing;

		for (var i = 0; i < concurrency; i ++) {
			var releaseZone = this.CreateChild<ReleaseZone>();
			releaseZone.size = unitSize;
			releaseZone.transform.localPosition = new Vector3(-launchZoneWidth/2 + unitWidth/2 + i*(unitWidth+unitSpacing), 0.1f, 0f);

			releaseZone.transform.Translate(Vector3.Scale(
				new Vector3(0f, size.y/2 + unitSize.y/2 + unitSpacing, size.z/2 + unitLength/2 + unitSpacing),
				unitPrefab.GetComponent<GameUnit>().launchDirection
			));

			releaseZones.Add(releaseZone);
		}

		tag = "Player" + player.playerNumber;

		entity.AddEventCallback<AttemptQueueUnitEvent>(AttemptQueueUnit);
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

		if (playerNumber == 2) {
			keyIcon.transform.Rotate(new Vector3(0, 0, 180));
		}

		//Debug.Log(player.playerNumber + ": " + gameUnit.GetType() + ": " + attemptQueueUnitKeyCode.ToString());
	}

	public override void ServerFixedUpdate () {
		//base.ServerFixedUpdate(); TODO: extract another class from GameUnit so we don't have to perform this perf opt.

		if (queueSize > 0) {
			ReleaseUnits();
		}
	}

	public override void ServerAndClientUpdate() {
		base.ServerAndClientUpdate();

		if (canQueueUnit) {
			//Paint();
			ShowHud(true);
			//keyIcon.SetActive((attemptQueueUnitKeyCode != KeyCode.None) && App.shared.prefs.keyIconsVisible);

		}
		else {
			//PaintAsDisabled();
            ShowHud(false);
		}

		keyIcon.SetActive(attemptQueueUnitKeyCode != KeyCode.None && player.isLocal && prefs.keyIconsVisible);
	}

	// HUD

	GameObject iconObject;
	public GameObject keyIcon;
	Prefs prefs;

    public void ShowHud(bool b = true) {
        foreach (Renderer renderer in iconObject.GetComponentsInChildren<Renderer>()) {
            renderer.enabled = b;
        }
	}

	/*public void HideHud() {
		if (!hudIsHidden) {
			keyIcon.SetActive(false);
			foreach (Renderer renderer in iconObject.GetComponentsInChildren<Renderer>()) {
				renderer.enabled = false;
			}
			hudIsHidden = true;
		}
	}*/


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

	public void OnMouseDown() {
		if (!npcModeOn) {
			SendAttemptQueueUnit();
		}
	}

	public override void QueuePlayerCommands() {
		base.QueuePlayerCommands();

		if (npcModeOn) {
			if (Random.value < 0.001*4) {
				SendAttemptQueueUnit();
			}
		}
        if (unitKeyMap == "None") {
            return;
        }
        if (!npcModeOn && unitKeyMap.Pressed()) {
			SendAttemptQueueUnit();
		}
	}

	void SendAttemptQueueUnit() {
		if (entity.hasControl) {
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

			queueSize --;
		}
	}

	ReleaseZone unobstructedReleaseZone {
		get {
			var shuffledZones = releaseZones.OrderBy(a => UnityEngine.Random.value);

			/*
			for (int i = 0; i < releaseZones.Count; i ++) {
				var rz = releaseZones.PickRandom();

				if (!rz.isObstructed) {
					return rz;
				}
			}
			*/

			foreach (var rz in shuffledZones) {
				if (!rz.isObstructed) {
					return rz;
				}
			}
			return null;
		}
	}
}