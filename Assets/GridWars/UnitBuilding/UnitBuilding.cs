using UnityEngine;
using System.Collections;

public class UnitBuilding : MonoBehaviour {
	public GameObject unitPrefab;
	public Player player;

	void ShowReadyToRelease() {
		GetComponent<MeshRenderer>().material = player.enabledMaterial;
	}

	void ShowNotReadyToRelease() {
		GetComponent<MeshRenderer>().material = player.disabledMaterial;
	}

	void ReleaseUnit() {
		var unitObject = Instantiate(unitPrefab);
		unitObject.transform.position = transform.position;
		unitObject.transform.rotation = transform.rotation;

		var gameUnit = unitObject.GetComponent<GameUnit>();
		gameUnit.player = player;
	}

	// Use this for initialization
	void Start() {
		player = new Player();
		player.playerNumber = 1;
	}
	
	// Update is called once per frame
	void Update() {
		if (Input.GetKey(KeyCode.S)) {
			ShowReadyToRelease();
		}

		if (Input.GetKeyDown(KeyCode.R)) {
			ReleaseUnit();
		}
	}

	public void OnMouseDown() {
		ReleaseUnit();
	}
}
