using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Player : MonoBehaviour {
	public static string primaryColorMaterialName = "PrimaryColor";
	public static string secondaryColorMaterialName = "SecondaryColor";

	public Battlefield battlefield;
	public Fortress fortress;
	public float separation = 0.9f;
	public Material primaryMaterial;
	public Material secondaryMaterial;

	//https://en.wikipedia.org/wiki/Federal_Standard_595_camouflage_colours

	/*
	Color[] colors = new Color[]{ 
		new Color(95f/255, 95f/255, 56f/255), 
		new Color(180f/255, 157f/255, 128f/255) 
	};
	*/

	Color[] primaryColors = new Color[]{ 
		//Color.red, 
		//Color.gray, 
		//Color.blue
		new Color(81f/255, 0f/255, 0f/255),
		new Color(22f/255, 22f/255, 191f/255)
	};

	Color[] secondaryColors = new Color[]{ 
		//new Color(120f/255, 120f/255, 120f/255),
		new Color(181f/255, 181f/255, 0f/255),
		new Color(181f/255, 181f/255, 0f/255)
	};

	//Color[] colors = new Color[]{ new Color(100f/255, 100f/255, 100f/255), new Color(150f/255, 150f/255, 150f/255) };

	//public List<GameObject> ownedObjects;

	public int playerNumber {
		get {
			return battlefield.players.IndexOf(this) + 1;
		}
	}

	public Color primaryColor {
		get {
			return primaryColors[playerNumber - 1];
		}
	}

	public Color secondaryColor {
		get {
			return secondaryColors[playerNumber - 1];
		}
	}

	public PowerSource powerSource {
		get {
			return fortress.powerSource;
		}
	}

	public List<GameUnit> units;

	public bool npcModeOn;

	public bool isLocal;

	public int localNumber {
		get {
			return App.shared.battlefield.localPlayers.IndexOf(this) + 1;
		}
	}

	void Start() {
		if (App.shared.testEndOfGameMode) {
			separation = 0.35f;
		}

		primaryMaterial = new Material(Resources.Load("Materials/" + primaryColorMaterialName) as Material);
		primaryMaterial.color = primaryColor;

		secondaryMaterial = new Material(Resources.Load("Materials/" + secondaryColorMaterialName) as Material);
		secondaryMaterial.color = secondaryColor;
		//primaryMaterial.SetFloat("_Glossiness", 0.35f);
		//ownedObjects = new List<GameObject>();

		gameObject.transform.parent = battlefield.transform;
		gameObject.transform.rotation = Quaternion.LookRotation(Vector3.forward * ((playerNumber % 2 == 0) ? -1 : 1), Vector3.up);
		gameObject.transform.localPosition = new Vector3(0f, 0f, -separation * gameObject.transform.forward.z * battlefield.bounds.z / 2f);

		//gameObject.tag = "Player" + playerNumber;
	}

	public void StartGame() {
		if (fortress == null) { //fortress will exist for rematches
			fortress = this.CreateChild<Fortress>();
			fortress.player = this;
			fortress.transform.localPosition = Vector3.zero;
			fortress.transform.localRotation = Quaternion.identity;
		}

		units = new List<GameUnit>();

		fortress.StartGame();
	}

	//Painting

	public void Paint(GameObject gameObject) {
		gameObject.EachRenderer(r => {
			if (r.material.name.StartsWith("PrimaryColor")) {
				r.material = primaryMaterial;
			}
			else if (r.material.name.StartsWith("SecondaryColor")) {
				r.material = secondaryMaterial;
			}
		});
	}

	string resourcesPath {
		get {
			return "Player/" + playerNumber + "/";
		}
	}

	string ResourcePath(string resourceName) {
		return resourcesPath + resourceName;
	}


	public virtual bool IsDead() {
		return units.Count == 0;
	}

	// --- Friend / Enemy ---------------------------------------

	public virtual bool IsFriendOf(Player otherPlayer) {
		if (otherPlayer == null) {
			return false;
		}
		return playerNumber == otherPlayer.playerNumber;
	}

	public virtual bool IsEnemyOf(Player otherPlayer) {
		if (otherPlayer == null) {
			return false;
		}
		return playerNumber != otherPlayer.playerNumber;
	}

	public List<Player> enemyPlayers {
		get {
			return Battlefield.current.players.FindAll(p => p.playerNumber != playerNumber);
		}
	}

	public Player Enemy() {
		return enemyPlayers[0];
	}

	public Player opponent {
		get {
			return enemyPlayers[0];
		}
	}

	public virtual List<GameObject> EnemyObjects() {
		var enemyObjects = new List<GameObject>();
		foreach(var enemyPlayer in enemyPlayers) {
			foreach (var unit in enemyPlayer.units) {
				enemyObjects.Add(unit.gameObject);
			}
		}
		return enemyObjects;
	}
		
	public virtual List<GameUnit> EnemyObjectsOfType(System.Type aType) {
		List <GameUnit> units = Enemy().units.Where(unit => unit.IsOfType(aType)).ToList<GameUnit>();
		return units;
	}

	public List <GameUnit> UnitsTargetingObj(GameObject targetObj) {
		var unitsTargeting = new List<GameUnit>();
		foreach(var unit in units) {
			if (unit.target == targetObj) {
				unitsTargeting.Add(unit);
			}
		}
		return unitsTargeting;
	}
		
	// --- Networking ---------------------------------------

	public void TakeControlOf(GameUnit gameUnit) {
		if (isLocal || (App.shared.network.connection == null)) {
			//take control as server
			gameUnit.entity.TakeControl();
		}
		else {
			//give control to client
			gameUnit.entity.AssignControl(App.shared.network.connection);
		}
	}

	public void FixedUpdate () {
		//base.ServerFixedUpdate(); 

		if (npcModeOn && BoltNetwork.isServer) {
			foreach (var tower in fortress.towers) {
				if (tower != null) {
					tower.NpcStep();
				}
			}
		}

		/*
		if (npcModeOn) {
			if (player.powerSource.PowerRatio() > .3) {

				float a = CountOfEnemyUnitsWeCanCounter();
				float b = CountOfEnemyUnitsThatCounterUs();

				//float desireToRelease = a * a / (1 + b);
				float cost = gameUnit.powerCost / player.powerSource.maxPower;
				float desireToRelease = 2f * (1.5f * a - b) / cost;

				if (Random.value < 0.001 * desireToRelease) {
					SendAttemptQueueUnit();
				} else if (player.powerSource.IsAtMax()) {
					if (Random.value < 0.001 * 2) {
						SendAttemptQueueUnit();
					}
				}
			} 
		}

		*/
	}
}
