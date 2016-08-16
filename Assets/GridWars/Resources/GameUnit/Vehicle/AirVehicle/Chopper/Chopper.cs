using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chopper : AirVehicle {
	public float cruiseHeight = 12f;
	public float thrustHeight = 4f;

	public GameObject mainRotor;
	public GameObject tailRotor;

	[HideInInspector]
	public float defaultCruiseHeight = 15f;
	/*
	Weapon _missileLauncherLeft;
	public Weapon missileLauncherLeft {
		get {
			if (_missileLauncherLeft == null) {
				_missileLauncherLeft = _t.FindDeepChild("MissileLauncherLeft").gameObject.GetComponent<Weapon>();
			}
			return _missileLauncherLeft;
		}
	}

	Weapon _missileLauncherRight;
	public Weapon missileLauncherRight {
		get {
			if (_missileLauncherRight == null) {
				_missileLauncherRight = _t.FindDeepChild("MissileLauncherRight").gameObject.GetComponent<Weapon>();
			}
			return _missileLauncherRight;
		}
	}
	*/

	public override void Start () {
		base.Start();
		isRunning = true;

		mainRotor = _t.FindDeepChild("mainRotor").gameObject;
		tailRotor = _t.FindDeepChild("tailRotor").gameObject;

		cruiseHeight = defaultCruiseHeight + Mathf.Floor(Random.Range(-4.0f, 0.0f)) * 1.0f;
	}

	public virtual void SpinRotors () {
		Object_rotDY (mainRotor, 20);
		Object_rotDY (tailRotor, 20);
	}

	public void ApplyCruisingAltitudeForce() {
		float diff = cruiseHeight - y ();

		if (y () < cruiseHeight) {
			rigidBody ().AddForce (_t.up * 6 * Mathf.Sqrt(diff));
		} 
	}

	public void ApplyThrustIfAppropriate() {
		if (target && y () > thrustHeight && !IsInStandoffRange()) {
			rigidBody().AddForce(_t.forward * thrust);
		}
	}

	public override void FixedUpdate () {
		if (isRunning) {
			//base.FixedUpdate();
			SpinRotors();		
			PickTarget();
			SteerTowardsTarget(); // picks target
			ApplyCruisingAltitudeForce();
			ApplyThrustIfAppropriate();
		}
	}

	void OnCollisionEnter(Collision collision) {
		// destroy on ground collision
		if (collision.collider.name == "BattlefieldPlane") {
			if (collision.relativeVelocity.magnitude > 2) {
				//audio.Play ();
				//print("collision.relativeVelocity.magnitude " + collision.relativeVelocity.magnitude);
				//Destroy (gameObject);
				//DeactivateWeapons();
				OnDead();
			}
		}
	}
}
