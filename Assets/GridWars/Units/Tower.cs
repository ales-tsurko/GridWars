using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tower : GameUnit {
	public GameObject prefabUnit;
	public int playerNumber;

	void Start () {


	}

	public void constructUnit () {
		GameObject unitGameObject = Instantiate(prefabUnit); 
		GameUnit unit = unitGameObject.GetComponent<GameUnit> ();
		unit.setX(transform.position.x);
		unit.setZ(transform.position.z);
		string tagString = "Player" + playerNumber;
		unit.tag = tagString;
		unit.setRotY (rotY());
	}

	public void OnMouseDown() {
			constructUnit();
	}

	public override void FixedUpdate () {


	}

}