using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	public static string primaryColorMaterialName = "PrimaryColor";
	public static string secondaryColorMaterialName = "SecondaryColor";

	public Battlefield battlefield;
	public Fortress fortress;
	public float separation = 0.9f;
	//public float separation = 0.35f;
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
		Color.red, 
		Color.blue
	};

	Color[] secondaryColors = new Color[]{ 
		new Color(120f/255, 120f/255, 120f/255),
		new Color(200f/255, 200f/255, 200f/255)
	};

	//Color[] colors = new Color[]{ new Color(100f/255, 100f/255, 100f/255), new Color(150f/255, 150f/255, 150f/255) };

	//public List<GameObject> ownedObjects;

	public BoltConnection connection;

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

	void Awake() {
		units = new List<GameUnit>();
	}

	void Start() {
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
		fortress = this.CreateChild<Fortress>();
		fortress.player = this;
		fortress.transform.localPosition = Vector3.zero;
		fortress.transform.localRotation = Quaternion.identity;
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

	public virtual List<GameObject> EnemyObjects() {
		var enemyObjects = new List<GameObject>();
		foreach(var enemyPlayer in enemyPlayers) {
			foreach (var unit in enemyPlayer.units) {
				enemyObjects.Add(unit.gameObject);
			}
		}
		return enemyObjects;
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
}
