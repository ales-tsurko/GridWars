using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tower : GameUnit {
	public GameObject prefabUnit;

	public void OnMouseDown() {
		if (canReleaseUnit) {
			ReleaseUnit();
		}
	}

	GameUnit iconUnit;

	void Start() {
		iconUnit = CreateUnit();
		iconUnit.GetComponent<GameUnit>().enabled = false;
		iconUnit.GetComponent<GameUnitIcon>().enabled = true;
	}

	void FixedUpdate () {
		GetComponent<MeshRenderer>().material = readyMaterial;
	}

	Material readyMaterial {
		get {
			if (canReleaseUnit) {
				return player.enabledMaterial;
			}
			else {
				return player.disabledMaterial;
			}
		}
	}

	bool canReleaseUnit {
		get {
			return player.powerSource.power >= prefabUnit.GetComponent<GameUnit>().powerCost;
		}
	}

	void ReleaseUnit() {
		CreateUnit();
	}

	GameUnit CreateUnit() {
		var unitObject = Instantiate(prefabUnit);
		unitObject.transform.position = transform.position + new Vector3(0, 1, 0);
		unitObject.transform.rotation = transform.rotation;

		var gameUnit = unitObject.GetComponent<GameUnit>();
		gameUnit.player = player;

		return gameUnit;
	}

}