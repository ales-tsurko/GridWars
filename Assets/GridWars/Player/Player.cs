using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	PowerBar powerBar;
	Color color;
	int playerNumber;

	// Use this for initialization
	void Start () {
		powerBar = PowerBar.New();
		powerBar.SetColor(color);
		powerBar.SetPosition(playerNumber == 1 ? PowerBarPlacement.Left : PowerBarPlacement.Right);
		powerBar.minPower = 0f;
		powerBar.maxPower = 600f;
		powerBar.power = 0f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		powerBar.power += 1;
	}
}
