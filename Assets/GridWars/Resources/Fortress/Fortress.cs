using UnityEngine;
using System.Collections.Generic;

public class Fortress : MonoBehaviour {
	public Player player;
	public PowerSource powerSource;
	public List<Tower> towers;
	public float towerSpacing {
		get {
			return 1.0f*Tower.size.x;
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
		typeof(MobileSAM),
		typeof(AirStrike),
		typeof(Jeep),
		typeof(Tank),
		typeof(Chopper)
	};

	// Use this for initialization
	void Start () {
		if (BoltNetwork.isServer) {
			//Bolt Entities must be root transforms.  Use this object to position things relative to Fortress / the powerSource
			var powerSourcePlacement = new GameObject();
			powerSourcePlacement.transform.parent = transform;
			powerSourcePlacement.transform.localPosition = new Vector3(0f, 0f, powerSourcePrefab.bounds.z/2);
			powerSourcePlacement.transform.localRotation = Quaternion.identity;

			powerSource = BoltNetwork.Instantiate(BoltPrefabs.PowerSource, powerSourcePlacement.transform.position, powerSourcePlacement.transform.rotation).GetComponent<PowerSource>();
			powerSource.player = player;
			powerSource.Setup();

			towers = new List<Tower>();
			var towerNum = 0;
			foreach (var unitType in unitTypes) {
				var tower = GameUnit.Instantiate<Tower>();
				tower.transform.parent = transform;
				tower.player = player;
				tower.unitPrefab = GameUnit.Load(unitType);
				tower.tag = "Player" + player.playerNumber;


				tower.transform.localRotation = Quaternion.identity;
				tower.transform.localPosition = new Vector3(-bounds.x/2 + Tower.size.x/2 + towerNum*(Tower.size.x + towerSpacing),
					0f,
					powerSourcePlacement.transform.localPosition.z + powerSourcePrefab.bounds.z/2 + towerToPowerSpacing + Tower.size.z/2
				);

				towerNum ++;
			}

			Destroy(powerSourcePlacement);
		}
	}

	PowerSource powerSourcePrefab {
		get {
			return Resources.Load<GameObject>("PowerSource/PowerSource").GetComponent<PowerSource>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.E)) {
			var unitObject = Instantiate (Resources.Load<GameObject> ("GameUnit/Engineer/Engineer"));
			unitObject.transform.position = transform.position + new Vector3(0, 0.1f, 0);
			unitObject.transform.rotation = transform.rotation;
			var gameUnit = unitObject.GetComponent<GameUnit>();
			gameUnit.player = player;
			gameUnit.tag = "Player" + player.playerNumber;
			gameUnit.GetComponent<Engineer> ().SwitchState (Engineer.State.Init);
		}
	}
}
