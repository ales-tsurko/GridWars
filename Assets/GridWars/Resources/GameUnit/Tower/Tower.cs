using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tower : GameUnit {
	public static Vector3 bounds = new Vector3(5f, 1f, 5f);

	public GameObject unitPrefab;


	public override void Start () {
		isStaticUnit = true;

		base.Start();
		canAim = false;

		iconUnit = CreateUnit();
		iconUnit.transform.parent = transform;
		iconUnit.transform.localPosition = Vector3.zero;
		iconUnit.transform.localRotation = Quaternion.identity;
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

		var gameUnit = unitObject.GetComponent<GameUnit>();
		gameUnit.player = player;

		return gameUnit;
	}

}