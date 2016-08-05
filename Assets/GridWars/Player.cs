using UnityEngine;
using System.Collections;

public class Player {
	public int playerNumber;

	public Material enabledMaterial {
		get {
			return Resources.Load("Players/" + playerNumber + "/Enabled") as Material;
		}
	}

	public Material disabledMaterial {
		get {
			return Resources.Load("Players/" + playerNumber + "/Enabled") as Material;
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

	public float baseEdgeX = 50f;

	public PowerSource powerSource;

	public void Start() {
		powerSource = PowerSource.Instantiate();
		//powerSource.gameObject.transform.position = Vector3.zero;
		powerSource.gameObject.transform.position = new Vector3((baseEdgeX - powerSource.trackWidth/2)*xDirection, 0.1f, 0f);
	}

	public void FixedUpdate() {
		
	}
}
