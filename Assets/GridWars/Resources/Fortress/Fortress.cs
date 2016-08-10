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
				powerSource.bounds.z + towerToPowerSpacing + Tower.size.z
			);
		}
	}

	static System.Type[] unitTypes = new System.Type[]{ typeof(MobileSAM), typeof(LightTank), typeof(Chopper), typeof(Tank) };

	// Use this for initialization
	void Start () {
		powerSource = PowerSource.Create();
		powerSource.player = player;
		powerSource.bounds = new Vector3(bounds.x, powerSource.bounds.y, powerSource.bounds.z);
		powerSource.transform.parent = transform;
		powerSource.transform.localPosition = new Vector3(0f, 0f, powerSource.bounds.z/2);
		powerSource.transform.localRotation = Quaternion.identity;

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
				powerSource.transform.localPosition.z + powerSource.bounds.z/2 + towerToPowerSpacing + Tower.size.z/2
			);

			towerNum ++;
		}

		/*
		int maxTowers = unitTypes.Count;
		for (int towerNum = 0; towerNum < maxTowers; towerNum ++) {
			var tower = GameUnit.Instantiate<Tower>();

			if (playerNum == 0) {
				tower.unitPrefab = GameUnit.Load(unitTypes[towerNum]);
			} else {
				tower.unitPrefab = GameUnit.Load(unitTypes[maxTowers - 1 - towerNum]);
			}

			float x = 50*((((float)towerNum) / (float)maxTowers) - 0.5f);

			tower.setX (x);
			tower.setY (0.0f);
			tower.setZ (z);

			tower.setRotY (180*playerNum);
			//print("adding tower " + towerNum + " for player " + players [playerNum].playerNumber);
			tower.player = players[playerNum];

			//if ((playerNum == 0 && towerNum == 0) || (playerNum == 1 && towerNum == 2)) {
			//tower.ReleaseUnit ();
			//}

		}
		*/
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
