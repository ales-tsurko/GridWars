using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	public Battlefield battlefield;
	public Fortress fortress;
	public float separation = 0.9f;
	public Material unitMaterial;

	public List<GameObject> ownedObjects;

	public BoltConnection connection { //TODO: set these as players connect via create game / start game separation
		get {
			return Network.shared.ConnectionForPlayer(this);
		}
	}

	public int playerNumber {
		get {
			return battlefield.players.IndexOf(this) + 1;
		}
	}

	public Color color {
		get {
			return colors[playerNumber - 1];
		}
	}

	public PowerSource powerSource {
		get {
			return fortress.powerSource;
		}
	}

	public List<GameUnit> units;

	public bool npcModeOn;

	void Awake() {
		units = new List<GameUnit>();
	}

	void Start() {
		unitMaterial = new Material(Resources.Load("Materials/Unit") as Material);
		unitMaterial.color = color;
			
		ownedObjects = new List<GameObject>();

		gameObject.transform.parent = battlefield.transform;
		gameObject.transform.rotation = Quaternion.LookRotation(Vector3.forward * ((playerNumber % 2 == 0) ? -1 : 1), Vector3.up);
		gameObject.transform.localPosition = new Vector3(0f, 0f, -separation * gameObject.transform.forward.z * battlefield.bounds.z / 2f);

		//gameObject.tag = "Player" + playerNumber;

		fortress = this.CreateChild<Fortress>();
		fortress.player = this;
		fortress.transform.localPosition = Vector3.zero;
		fortress.transform.localRotation = Quaternion.identity;
	}

	//Painting

	public void Paint(GameObject gameObject) {
		//gameObject.Paint(color, "Unit");
		gameObject.EachRenderer(r => {
			if (r.material.name.StartsWith("Unit")) {
				r.material = unitMaterial;
			}
		});
	}

	/*
	public void PaintAsDisabled(GameObject gameObject) {
		var c = new Color();
		c.r = color.r/2;
		c.g = color.g/2;
		c.b = color.b/2;
		gameObject.Paint(c, "Unit");
	}

	public void PaintAsHighlighted(GameObject gameObject, float level) {
		var c = Color.Lerp(color, Color.white, level);
		gameObject.Paint(c, "Unit");
	}
	*/

	string resourcesPath {
		get {
			return "Player/" + playerNumber + "/";
		}
	}

	string ResourcePath(string resourceName) {
		return resourcesPath + resourceName;
	}

	//https://en.wikipedia.org/wiki/Federal_Standard_595_camouflage_colours

	Color[] colors = new Color[]{ new Color(95f/255, 95f/255, 56f/255), new Color(180f/255, 157f/255, 128f/255) };

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

	public List<Player> enemies {
		get {
			return Battlefield.current.players.FindAll(p => p.playerNumber != playerNumber);
		}
	}
		
	public virtual List<GameObject> EnemyObjects() {
		var enemyObjects = new List<GameObject>();
		foreach(var enemy in enemies) {
			foreach (var unit in enemy.units) {
				enemyObjects.Add(unit.gameObject);
			}
		}
		return enemyObjects;
	}
		
	// --- Networking ---------------------------------------

	public void TakeControlOf(GameUnit gameUnit) {
		if (connection) {
			//give control to client
			gameUnit.entity.AssignControl(connection);
		}
		else {
			//take control as server
			gameUnit.entity.TakeControl();
		}
	}
}
