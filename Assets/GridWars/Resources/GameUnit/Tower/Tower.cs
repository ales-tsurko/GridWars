using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tower : GameUnit {
	public GameObject unitPrefab;

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

	public void ReleaseUnit() {
		CreateUnit();
	}

	GameUnit iconUnit;

	void Update () {
		if (canReleaseUnit) {
			player.Paint(gameObject);
		}
		else {
			player.PaintAsDisabled(gameObject);
		}
	}

	bool canReleaseUnit {
		get {
			return player.powerSource.power >= unitPrefab.GetComponent<GameUnit>().powerCost;
		}
	}

	GameUnit CreateUnit() {
		var unitObject = Instantiate(unitPrefab);
		unitObject.transform.position = transform.position + new Vector3(0, 0.1f, 0);
		unitObject.transform.rotation = transform.rotation;

		var gameUnit = unitObject.GetComponent<GameUnit>();
		gameUnit.player = player;

		return gameUnit;
	}

}