using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AirVehicle : Vehicle {



	virtual public void OnCollisionEnter(Collision collision) {
		//base.OnCollisionEnter(Collision);

		// ignore collisions between air vehicles (for now)

		GameUnit unit = collision.gameObject.GetComponent<GameUnit>();

		if (unit && unit.GetType().IsSubclassOf(typeof(AirVehicle))) {
			Physics.IgnoreCollision(collision.collider, GetComponent<BoxCollider>());
		}
	}

}
