using UnityEngine;
using System.Collections.Generic;

public class Fortress : MonoBehaviour {
	public Player player;
	public PowerSource powerSource;
	public List<Tower> towers;
	public float towerSpacing {
		get {
			return 0.7f*Tower.size.x;
		}
	}
	public float towerToPowerSpacing {
		get {
			return 0.25f*Tower.size.x;
		}
	}

	public Vector3 bounds {
		get {
			return new Vector3(unitTypes.Length*(Tower.size.x+towerSpacing) - towerSpacing,
				Tower.size.y,
				powerSourcePrefab.bounds.z + towerToPowerSpacing + Tower.size.z
			);
		}
	}

	static System.Type[] unitTypes = new System.Type[] {
		typeof(Chopper),
		typeof(Tanker),
		typeof(Tank),
		typeof(MobileSAM)
	};

	/*
	static System.Type[] unitTypes = new System.Type[] {
		typeof(Chopper),
		typeof(Tank),
		typeof(MobileSAM),
		typeof(Tanker)
	};
	*/


	private GameObject placement = null;

	void CreatePlacement() {
		//Bolt Entities must be root transforms.  Use this object to position things relative to Fortress / the powerSource
		placement = new GameObject();
		placement.transform.parent = transform;
		placement.transform.localPosition = new Vector3(0f, 0f, powerSourcePrefab.bounds.z/2);
		placement.transform.localRotation = Quaternion.identity;
	}

	// Use this for initialization
	public void StartGame () {
		if (App.shared.testEndOfGameMode) {
			//*
			unitTypes = new System.Type[] {
				typeof(Tank)
			};
			//*/
		}

		if (BoltNetwork.isServer) {
			CreatePlacement();
			PlacePowerSource();
			PlaceUnitTowers();
			Destroy(placement);
		}
	}

	void PlacePowerSource() {
		//client sets powerSource reference separately
		powerSource = GameUnit.Instantiate<PowerSource>();
		powerSource.player = player;
		powerSource.transform.position = transform.position;
		powerSource.transform.rotation = transform.rotation;

	}

	/*
	void setPlacementXYZ(float x, float y, float z) {
		placement.transform.localPosition = new Vector3(x, y, z);
		placement.transform.localRotation = Quaternion.identity;
	}
	*/

	void PlaceUnitTowers() {
		towers = new List<Tower>();
		var towerNum = 0;
		var z = placement.transform.localPosition.z;
		foreach (var unitType in unitTypes) {
			float tx = -bounds.x / 2 + Tower.size.x / 2 + towerNum * (Tower.size.x + towerSpacing);
			float tz = z + powerSourcePrefab.bounds.z / 2 + towerToPowerSpacing + Tower.size.z / 2;
			placement.transform.localPosition = new Vector3(tx, 0f, tz);
			placement.transform.localRotation = Quaternion.identity;

			var tower = GameUnit.Instantiate<Tower>();
			tower.player = player;
			tower.unitPrefabPath = App.shared.PrefabPathForUnitType(unitType);
			tower.transform.position = placement.transform.position;
			tower.transform.rotation = placement.transform.rotation;
			towers.Add(tower);
			towerNum ++;
		}
	}

	void PlacePointDefenseTowers() {
		var tower = GameUnit.Instantiate<PointDefenseTower>();
		tower.player = player;
		tower.unitPrefabPath = App.shared.PrefabPathForUnitType(typeof(PointDefense));
		placement.transform.localPosition = new Vector3(0f, 0f, 4f);
		tower.transform.position = placement.transform.position;
		tower.transform.rotation = placement.transform.rotation;

	}

	PowerSource powerSourcePrefab {
		get {
			return GameUnit.Load<PowerSource>().GetComponent<PowerSource>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		/*if (Input.GetKeyDown (KeyCode.E)) {
			var unitObject = Instantiate (Resources.Load<GameObject> ("GameUnit/Engineer/Engineer"));
			unitObject.transform.position = transform.position + new Vector3(0, 0.1f, 0);
			unitObject.transform.rotation = transform.rotation;
			var gameUnit = unitObject.GameUnit();
			gameUnit.player = player;
			gameUnit.tag = "Player" + player.playerNumber;
			gameUnit.GetComponent<Engineer> ().SwitchState (Engineer.State.Init);
		}*/
	}

	public void TowerDied(Tower tower) {
		towers.Remove(tower);
		player.powerSource.MakeMax();
	}
}
