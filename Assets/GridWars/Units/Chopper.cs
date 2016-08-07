using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chopper : GameUnit {
	public float cruiseHeight = 15f;

	public override void Start () {
		base.Start();
		thrust = 10;
		rotationThrust = 0.01f;
		isRunning = true;
	}
		
	GameObject mainRotor() {
		return transform.Find("Group003").gameObject;
	}

	GameObject tailRotor() {
		return transform.Find("Group006").gameObject;
	}

		
	public override void FixedUpdate () {
		//base.FixedUpdate();

		RemoveIfOutOfBounds ();

		if (isRunning) {
			Object_rotDY (mainRotor (), 20);
			Object_rotDY (tailRotor (), 20);

			float diff = cruiseHeight - y ();

			if (y () < cruiseHeight) {
				rigidBody ().AddForce (_t.up * 6 * Mathf.Sqrt(diff));
			} 
			steerTowardsNearestEnemy();

			if (y () > 4) {
				rigidBody().AddForce(_t.forward * thrust);
		}

		}

		/*
		if (diff > 0) {
			setRotZ ( diff * 2);
		}
		*/
		//steerTowardsNearestEnemy ();

	}

	void OnCollisionEnter(Collision collision) {
		if (collision.collider.name == "BattlefieldPlane") {
			if (collision.relativeVelocity.magnitude > 2) {
				//audio.Play ();
				print("collision.relativeVelocity.magnitude " + collision.relativeVelocity.magnitude);
				//Destroy (gameObject);
				isRunning = false;
			}
		}

	}

}
