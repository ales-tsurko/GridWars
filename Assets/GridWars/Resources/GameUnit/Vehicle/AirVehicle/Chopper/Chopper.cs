using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chopper : AirVehicle {
	public float cruiseHeight = 15f;
	public float thrustHeight = 4f;

	public GameObject mainRotor;
	public GameObject tailRotor;

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

	public override void Start () {
		base.Start();
		//thrust = 10;
		//rotationThrust = 0.01f;
		isRunning = true;

		mainRotor = _t.FindDeepChild("mainRotor").gameObject;
		tailRotor = _t.FindDeepChild("tailRotor").gameObject;
			
		missileLauncherLeft.owner = gameObject;
		missileLauncherRight.owner = gameObject;
	}

	/*
	public override void UpdatedTarget() {
		// we may want to have independent targets for multiple weapons...
		missileLauncherLeft.target  = target;
		missileLauncherRight.target = target;
	}
	*/

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
		if (y () > thrustHeight && !IsInStandoffRange()) {
			rigidBody().AddForce(_t.forward * thrust);
		}
	}

	public override void FixedUpdate () {
		if (isRunning) {
			//base.FixedUpdate();
			SpinRotors();		
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
				Disable();
			}
		}
	}

	public void Disable() {
		isRunning = false;
		missileLauncherLeft.isActive = false;
		missileLauncherRight.isActive = false;
	}
}
