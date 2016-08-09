using UnityEngine;
using System.Collections;

public class Player {
	public int playerNumber;

	public Color color {
		get {
			return colors[playerNumber - 1];
		}
	}

	public int xDirection {
		get {
			return playerNumber == 1 ? -1 : 1;
		}
	}

	public float baseEdgeZ = 50f;

	public PowerSource powerSource;

	public void Start() {
		powerSource = PowerSource.Instantiate();
		//powerSource.gameObject.transform.position = Vector3.zero;
		powerSource.gameObject.transform.position = new Vector3(0f, 0.1f, (baseEdgeZ - powerSource.trackWidth/2)*xDirection);
		powerSource.player = this;
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
