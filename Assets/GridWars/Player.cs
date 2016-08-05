using UnityEngine;
using System.Collections;

public class Player {
	public int playerNumber;

	public Material enabledMaterial {
		get {
			return Resources.Load<Material>(ResourcePath("Enabled"));
		}
	}

	public Material disabledMaterial {
		get {
			return Resources.Load<Material>(ResourcePath("Disabled"));
		}
	}

	public Color color {
		get {
			return playerNumber == 1 ? Color.red : Color.blue;
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

	public void FixedUpdate() {
		
	}

	string resourcesPath {
		get {
			return "Players/" + playerNumber + "/";
		}
	}

	string ResourcePath(string resourceName) {
		return resourcesPath + resourceName;
	}
}
