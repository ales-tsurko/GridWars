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

	PowerBar powerBar;
	float power;

	// Use this for initialization
	public void Start() {
		//Debug.Log(playerNumber);
		powerBar = PowerBar.New();
		powerBar.SetPosition(playerNumber == 1 ? PowerBarPlacement.Left : PowerBarPlacement.Right);
		powerBar.SetColor(playerNumber == 1 ? Color.red : Color.blue);
		powerBar.SetPower(0);
		//powerBar.minPower = 0f;
		//powerBar.maxPower = 600f;
		//powerBar.power = 0f;
		power = 0f;
	}
	
	// Update is called once per frame
	public void FixedUpdate () {
		power ++;

		float maxPower = 10f/Time.fixedDeltaTime;

		powerBar.SetPower((int)(100f*Mathf.Min(power, maxPower) / maxPower));
	}
}
