﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chopper : GameUnit {
	public float cruiseHeight = 15f;
	public GameObject mainRotor;
	public GameObject tailRotor;

	public Weapon missileLauncherLeft {
		get {
			return _t.Find("MissileLauncherLeft").gameObject.GetComponent<Weapon>();
		}
	}

	public Weapon missileLauncherRight {
		get {
			return _t.Find("MissileLauncherRight").gameObject.GetComponent<Weapon>();
		}
	}

	public override void Start () {
		base.Start();
		thrust = 10;
		rotationThrust = 0.01f;
		isRunning = true;

		mainRotor = _t.Find("Group003").gameObject;
		tailRotor = _t.Find("Group006").gameObject;
			
		missileLauncherLeft.owner = gameObject;
		missileLauncherRight.owner = gameObject;
	}

	public override void pickTarget () {
		base.pickTarget();
		// we may want to have independent targets for multiple weapons...
		missileLauncherLeft.target = target;
		missileLauncherRight.target = target;
	}

	public override void FixedUpdate () {
		//base.FixedUpdate();
		missileLauncherLeft.player = player;
		missileLauncherRight.player = player;

		pickTarget();
		RemoveIfOutOfBounds ();

		if (isRunning) {
			Object_rotDY (mainRotor, 20);
			Object_rotDY (tailRotor, 20);

			float diff = cruiseHeight - y ();

			if (y () < cruiseHeight) {
				rigidBody ().AddForce (_t.up * 6 * Mathf.Sqrt(diff));
			} 

			steerTowardsTarget();

			if (y () > 4) {
				rigidBody().AddForce(_t.forward * thrust);
			}
		}
	}

	void OnCollisionEnter(Collision collision) {
		// destroy on ground collision
		if (collision.collider.name == "BattlefieldPlane") {
			if (collision.relativeVelocity.magnitude > 2) {
				//audio.Play ();
				//print("collision.relativeVelocity.magnitude " + collision.relativeVelocity.magnitude);
				//Destroy (gameObject);
				Disable();
			}
		}
	}

	public void Disable() {
		isRunning = false;
		missileLauncherLeft.isActive = false;
		missileLauncherRight.isActive = false;
	}

	void OnDisable() {
		missileLauncherLeft.enabled = false;
		missileLauncherRight.enabled = false;
	}

}
