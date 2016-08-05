using UnityEngine;
using System.Collections;

public class UnitBuilding : MonoBehaviour {
	public GameObject unitPrefab;
	public Player player;

	public void Disable() {
		GetComponent<MeshRenderer>().material = player.disabledMaterial;
	}

	public void Enable() {
		GetComponent<MeshRenderer>().material = player.enabledMaterial;
	}

	public void SpawnUnit() {
		var unit = Instantiate(unitPrefab);
		unit.transform.position = transform.position;
		unit.transform.rotation = transform.rotation;
	}

	// Use this for initialization
	void Start () {
		player = new Player();
		player.playerNumber = 1;

		Disable();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.E)) {
			Enable();
		}

		if (Input.GetKey(KeyCode.S)) {
			SpawnUnit();
		}
	}
}
