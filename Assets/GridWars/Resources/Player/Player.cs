using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	public Battlefield battlefield;
	public Fortress fortress;

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

	public List<GameUnit> units {
		get {
			return new List<GameUnit>(FindObjectsOfType<GameUnit>()).FindAll(gameUnit => gameUnit.enabled && gameUnit.player == this);
		}
	}

	void Start() {
		gameObject.transform.parent = battlefield.transform;
		gameObject.transform.rotation = Quaternion.LookRotation(Vector3.forward * ((playerNumber % 2 == 0) ? -1 : 1), Vector3.up);
		gameObject.transform.localPosition = new Vector3(0f, 0f, -0.9f*gameObject.transform.forward.z*battlefield.bounds.z/2);

		gameObject.tag = "Player" + playerNumber;

		fortress = this.CreateChild<Fortress>();
		fortress.player = this;
		fortress.transform.localPosition = Vector3.zero;
		fortress.transform.localRotation = Quaternion.identity;
	}

	public void Paint(GameObject gameObject) {
		gameObject.Paint(color, "Unit");
	}

	public void PaintAsDisabled(GameObject gameObject) {
		var c = new Color();
		c.r = color.r/2;
		c.g = color.g/2;
		c.b = color.b/2;
		gameObject.Paint(c, "Unit");
	}

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
		return units.TrueForAll(u => u.isDestroyed);
	}
}
