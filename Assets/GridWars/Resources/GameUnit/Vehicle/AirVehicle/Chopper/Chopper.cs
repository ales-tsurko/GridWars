using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chopper : AirVehicle {
	public float cruiseHeight = 12f;
	public float thrustHeight = 4f;

	public GameObject mainRotor;
	public GameObject tailRotor;

	Transform mainRotorTransform; // set in start


	//public transform mainRotorThrustPointFront;
	//public transform mainRotorThrustPointBack;

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

		mainRotorTransform = _t.FindDeepChild("mainRotorCenter");
	}

	/*
	public virtual void SpinRotors (float v) {
		Object_rotDY (mainRotor, v);
		Object_rotDY (tailRotor, v);
	}

	public void ApplyForces() {
		ApplyCruisingAltitudeForce();
		ApplyThrustIfAppropriate();
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
	*/

	public float UpDesire() { // 0.0 to 1.0
		float diff = cruiseHeight - y ();

		//if (y () < cruiseHeight) {
			return Mathf.Clamp(Smooth(diff)/2, 0f, 1f);
		//}

		return 0f;
	}

	public float ForwardDesire() { // 0.0 to 1.0 
		if (!target) {
			return 0f;
		}

		if (y() < thrustHeight) {
			return 0f;
		}

		if (!IsInStandoffRange()) {
			float angleDiff = Mathf.Abs(AngleToTarget());
			if (angleDiff < 30) {
				float diff = targetDistance() - standOffDistance;
				return Mathf.Clamp(diff, 0f, 1f);
			}
		}

		return 0f;
	}

	public float ForwardSpeed() {
		var localVelocity = transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity);
		return localVelocity.z;
	}

	public float Smooth(float v) {
		return Mathf.Sign(v)*Mathf.Sqrt(Mathf.Abs(v));
	}

	public void  ApplyRotorThrust() {
		// points around top rotor to apply force
		// a difference between the force applied to these causes chopper to tilt and then move forward or back

		float upThrust = 16f * UpDesire();

		float offset = 1.0f;
		Vector3 mainRotorThrustPointBack  = mainRotorTransform.position + mainRotorTransform.forward * offset;
		Vector3 mainRotorThrustPointFront = mainRotorTransform.position - mainRotorTransform.forward * offset;

		/*
		float u = UpDesire();
		float frontThrust = u * (1 + f) * thrustLevel;
		float rearThrust  = u * (1 + f) * thrustLevel;

		rigidBody().AddForceAtPosition(mainRotor.transform.forward * frontThrust, mainRotorThrustPointFront);
		rigidBody().AddForceAtPosition(mainRotor.transform.forward * rearThrust,  mainRotorThrustPointBack);
		*/

		Vector3 rotorUp = mainRotorTransform.up;
		float speed = ForwardSpeed();
		float desiredSpeed = ForwardDesire()*4;
		float speedDiff = desiredSpeed - speed;
		float f = Mathf.Clamp(speedDiff/10, -4, 4);


		Vector3 frontForce = rotorUp * ((upThrust + f) / 2);
		Vector3 backForce  = rotorUp * ((upThrust - f) / 2);

		rigidBody().AddForceAtPosition(frontForce, mainRotorThrustPointFront);
		rigidBody().AddForceAtPosition(backForce,  mainRotorThrustPointBack);

		Debug.DrawLine(mainRotorThrustPointFront, mainRotorThrustPointFront + frontForce * 2.0f, Color.yellow); 
		Debug.DrawLine(mainRotorThrustPointBack,  mainRotorThrustPointBack  + backForce  * 2.0f, Color.blue); 


		//rigidBody().AddForceAtPosition(mainRotorTransform.up * thrustLevel,  mainRotorTransform.position);

		Object_rotDY (mainRotor, 40f);
		Object_rotDY (tailRotor, 40f);
	}

	public override void FixedUpdate () {
		if (isRunning) {
			//base.FixedUpdate();
			//SpinRotors();		
			PickTarget();
			SteerTowardsTarget(); // picks target

			ApplyRotorThrust();
			//ApplyForces();
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
