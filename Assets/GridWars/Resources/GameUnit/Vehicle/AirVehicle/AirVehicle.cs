using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AirVehicle : Vehicle {

	public override void MasterStart() {
		base.MasterStart();
		IgnoreAirVehicleCollisions();
	}

	public void IgnoreAirVehicleCollisions() {
		AirVehicle[] units = (AirVehicle[])UnityEngine.Object.FindObjectsOfType(typeof(AirVehicle));

		foreach (var unit in units) {
			if (unit != this) {
				Physics.IgnoreCollision(unit.BoxCollider(), BoxCollider(), true);
			}
		}
	}

	virtual public void OnCollisionEnter(Collision collision) {
		//base.OnCollisionEnter(Collision);
		// ignore collisions between air vehicles (for now)
		/*
		GameObject obj = collision.gameObject;
		GameUnit unit = obj.GetComponent<GameUnit>();

		if (unit && unit.GetType().IsSubclassOf(typeof(AirVehicle))) {
			Physics.IgnoreCollision(collision.collider, GetComponent<BoxCollider>());
		}
		*/
	}

}
