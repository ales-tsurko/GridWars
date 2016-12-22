using UnityEngine;
using System.Collections.Generic;

public class Fortress : MonoBehaviour {
	public Player player;
	public PowerSource powerSource;
	public PowerSource spellSource;
	public List<Tower> towers;

	public float towerSpacing {
		get {
			return 0.7f*Tower.size.x;
		}
	}
	public float towerToPowerSpacing {
		get {
			return 0.25f * Tower.size.x;
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

	public float DistanceToEnemyFortress() {
		foreach (var enemyPlayer in player.enemyPlayers) {
			return Vector3.Distance(enemyPlayer.fortress.transform.position, transform.position);
		}

		return -1; 
	}

	static System.Type[] unitTypes = new System.Type[] {
		typeof(Chopper),
		typeof(Tanker),
		//typeof(LightTank),
		typeof(Tank),
		typeof(MobileSAM)
//		typeof(Gunship)
	};

	/*
	static System.Type[] unitTypes = new System.Type[] {
		typeof(Chopper),
		typeof(Tank),
		typeof(MobileSAM),
		typeof(Tanker)
	};
	*/


	private GameObject fortressPlacement = null;

	void CreatePlacement() {
		//Bolt Entities must be root transforms.  Use this object to position things relative to Fortress / the powerSource
		fortressPlacement = new GameObject();
		fortressPlacement.transform.parent = transform;
		fortressPlacement.transform.localPosition = new Vector3(0f, 0f, powerSourcePrefab.bounds.z/2);
		fortressPlacement.transform.localRotation = Quaternion.identity;
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

		CreatePlacement();

		if (BoltNetwork.isServer) {
			PlacePowerSource();
			PlaceSpellSource();
			PlaceUnitTowers();
			Destroy(fortressPlacement);
		}
	}

	void PlacePowerSource() {
		//client sets powerSource reference separately
		powerSource = GameUnit.Instantiate<PowerSource>();
		powerSource.name = "PowerSource Player" + player.playerNumber;
		powerSource.player = player;
		powerSource.transform.position = transform.position;
		powerSource.transform.rotation = transform.rotation;

	}

	void PlaceSpellSource() {
		spellSource = GameUnit.Instantiate<SpellSource>();
		spellSource.name = "SpellSource Player" + player.playerNumber;
		spellSource.player = player;
		spellSource.transform.position = transform.position;
		spellSource.transform.rotation = transform.rotation;

		spellSource.transform.Translate(new Vector3(0, 0, -powerSource.bounds.z));
	}

	/*
	void setPlacementXYZ(float x, float y, float z) {
		placement.transform.localPosition = new Vector3(x, y, z);
		placement.transform.localRotation = Quaternion.identity;
	}
	*/

	List<GameObject> _towerPlacements;
	public List<GameObject>towerPlacements {
		get {
			if (_towerPlacements == null) {
				_towerPlacements = new List<GameObject>();
				var z = fortressPlacement.transform.localPosition.z;
				for (int  i = 0; i < unitTypes.Length; i++) {
					float tx = -bounds.x / 2 + Tower.size.x / 2 + i * (Tower.size.x + towerSpacing);
					float tz = z + powerSourcePrefab.bounds.z / 2 + towerToPowerSpacing + Tower.size.z / 2;
					var placement = new GameObject();
					placement.name = "TowerPlacement";
					placement.transform.parent = transform;
					placement.transform.localPosition = new Vector3(tx, 0f, tz);
					placement.transform.localRotation = Quaternion.identity;
					towerPlacements.Add(placement);
				}
			}

			return _towerPlacements;
		}
	}

	void PlaceUnitTowers() {
		towers = new List<Tower>();
		var i = 0;
		foreach (var placement in towerPlacements) {
			var tower = GameUnit.Instantiate<Tower>();
			tower.player = player;
			tower.unitPrefabPath = App.shared.PrefabPathForUnitType(unitTypes[i]);
			tower.transform.position = placement.transform.position;
			tower.transform.rotation = placement.transform.rotation;
			towers.Add(tower);
			i ++;
		}
	}

	public void Unhide() {
		float dt = 0f;
		foreach (var tower in towers.Shuffled()) {
			tower.UnhideIn(dt);
			dt += 0.15f;
		}
	}

	PowerSource powerSourcePrefab {
		get {
			return GameUnit.Load<PowerSource>().GetComponent<PowerSource>();
		}
	}

	public void TowerDied(Tower tower) {
		towers.Remove(tower);

		if (!player.isInGame) {
			return;
		}

		if (towers.Count == 0) {
			player.powerSource.ShutDown();
			player.spellSource.ShutDown();
		} else {
			player.powerSource.power += (player.powerSource.maxPower - player.powerSource.power) * 0.8f;
			player.powerSource.generationRate *= 1.05f;

			player.spellSource.power += (player.spellSource.maxPower - player.spellSource.power) * 0.8f;
			player.spellSource.generationRate *= 1.05f;
		}
	}

	public GameObject ClosestEnemyObject() {
		float closestDist = Mathf.Infinity;
		GameObject closestObj = null;

		foreach (var tower in towers) {
			var enemyObj = tower.ClosestEnemyObject();
			if (enemyObj == null) {
				continue;
			}
			float d = tower.DistanceToObj(enemyObj);
			if (d < closestDist) {
				closestObj = enemyObj;
			}
		}

		return closestObj;
	}

	public float DistanceRatioOfClosestEnemy() {
		GameObject obj = ClosestEnemyObject();
		if (obj != null && obj.GameUnit() != null) {
			return obj.GameUnit().RatioOfDistanceToEnemyFortress();
		}

		return 1f;
	}

	public float TowersNetDamageRatio() {
		float maxHitPoints = 0f;
		float hitPoints = 0f;

		foreach (var tower in towers) {
			maxHitPoints += tower.maxHitPoints;
			hitPoints += tower.hitPoints;
		}

		return hitPoints / maxHitPoints;
	}
}