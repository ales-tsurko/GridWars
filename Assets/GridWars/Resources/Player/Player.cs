using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public int playerNumber {
		get {
			return battlefield.players.IndexOf(this) + 1;
		}
	}

	public Battlefield battlefield;
	public Fortress fortress;

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

	void Start() {
		gameObject.transform.parent = battlefield.transform;
		gameObject.transform.rotation = Quaternion.LookRotation(Vector3.forward * ((playerNumber % 2 == 0) ? -1 : 1), Vector3.up);
		gameObject.transform.localPosition = new Vector3(0f, 0f, -0.9f*gameObject.transform.forward.z*battlefield.bounds.z/2);

		fortress = this.CreateChild<Fortress>();
		fortress.player = this;
		fortress.transform.localPosition = Vector3.zero;
		fortress.transform.localRotation = Quaternion.identity;
	}

	public void Paint(GameObject gameObject) {
		gameObject.EachMaterial(m => {
			m.SetColor("_Color", color);
		});
	}

	public void PaintAsDisabled(GameObject gameObject) {
		gameObject.EachMaterial(m => {
			var c = new Color();
			c.r = color.r/2;
			c.g = color.g/2;
			c.b = color.b/2;
			m.SetColor("_Color", c);
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

	//https://en.wikipedia.org/wiki/Federal_Standard_595_camouflage_colours

	Color[] colors = new Color[]{ new Color(78f/255, 84f/255, 68f/255), new Color(180f/255, 157f/255, 128f/255) };
}
