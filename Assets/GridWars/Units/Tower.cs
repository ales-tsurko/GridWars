using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tower : GameUnit {
	public GameObject prefabUnit;

	public override void Start () {
		isStaticUnit = true;

		base.Start();
		canAim = false;

		iconUnit = CreateUnit();
		iconUnit.GetComponent<GameUnit>().enabled = false;
		iconUnit.GetComponent<GameUnitIcon>().enabled = true;
	}

	public void OnMouseDown() {
		if (canReleaseUnit) {
			ReleaseUnit();
		}
	}

	GameUnit iconUnit;

	public override void FixedUpdate () {
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

	public void ReleaseUnit() {
		CreateUnit();
	}

	GameUnit CreateUnit() {
		var unitObject = Instantiate(prefabUnit);
		unitObject.transform.position = transform.position + new Vector3(0, 0.1f, 0);
		unitObject.transform.rotation = transform.rotation;

		var gameUnit = unitObject.GetComponent<GameUnit>();
		gameUnit.player = player;

		return gameUnit;
	}

}