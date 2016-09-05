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
		typeof(MobileSAM),
		typeof(Tanker),
		typeof(Tank)//,
		typeof(Chopper)
	};

	// Use this for initialization
	void Start () {
		if (BoltNetwork.isServer) {
			//Bolt Entities must be root transforms.  Use this object to position things relative to Fortress / the powerSource
			var placement = new GameObject();
			placement.transform.parent = transform;
			placement.transform.localPosition = new Vector3(0f, 0f, powerSourcePrefab.bounds.z/2);
			placement.transform.localRotation = Quaternion.identity;

			//client sets powerSource reference separately
			powerSource = GameUnit.Instantiate<PowerSource>();
			powerSource.player = player;
			powerSource.transform.position = transform.position;
			powerSource.transform.rotation = transform.rotation;

			//*
			towers = new List<Tower>();
			var towerNum = 0;
			var z = placement.transform.localPosition.z;
			foreach (var unitType in unitTypes) {
				placement.transform.localPosition = new Vector3(-bounds.x/2 + Tower.size.x/2 + towerNum*(Tower.size.x + towerSpacing),
					0f,
					z + powerSourcePrefab.bounds.z/2 + towerToPowerSpacing + Tower.size.z/2
				);
				placement.transform.localRotation = Quaternion.identity;

				var tower = GameUnit.Instantiate<Tower>();
				tower.player = player;
				tower.unitPrefabPath = App.shared.PrefabPathForUnitType(unitType);
				tower.transform.position = placement.transform.position;
				tower.transform.rotation = placement.transform.rotation;

				towerNum ++;
			}
			//*/

			Destroy(placement);
		}
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
}
