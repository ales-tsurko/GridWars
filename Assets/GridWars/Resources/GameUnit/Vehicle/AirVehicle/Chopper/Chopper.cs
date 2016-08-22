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
	public float damageRotation;


	public override void Start () {
		base.Start();
		isRunning = true;

		mainRotor = _t.FindDeepChild("mainRotor").gameObject;
		tailRotor = _t.FindDeepChild("tailRotor").gameObject;

		cruiseHeight = defaultCruiseHeight + Mathf.Floor(Random.Range(-4.0f, 0.0f)) * 1.0f;

		mainRotorTransform = _t.FindDeepChild("mainRotorCenter");

		damageRotation = (Random.value - 0.5f)*10f;
	}

	public float UpDesire() { // 0.0 to 1.0
		float diff = cruiseHeight - y ();
		return Mathf.Clamp(Smooth(diff)/2, 0f, 1f);
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

	public float RightSpeed() {
		var localVelocity = transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity);
		return localVelocity.x;
	}

	public float Smooth(float v) {
		return Mathf.Sign(v)*Mathf.Sqrt(Mathf.Abs(v));
	}

	public bool IsHeavilyDamaged() {
		return ((hitPoints / maxHitPoints) < .5);
	}

	public float TiltDesire() { // -1.0 to 1.0
		// find tilt angle in world coordinates
		return Mathf.Clamp(transform.rotation.eulerAngles.z/10.0f, -1.0f, 1.0f);
	}

	public void  ApplyRotorLRThrust() {
		// z tilt control ------------------------------------------------------
		float upThrust = TotalUpThrust()/1f;

		float offset = 1.0f;
		Vector3 thrustPointLeft  = mainRotorTransform.position - mainRotorTransform.right * offset;
		Vector3 thrustPointRight = mainRotorTransform.position + mainRotorTransform.right * offset;

		float f = TiltDesire();

		Vector3 rotorUp = mainRotorTransform.up;
		Vector3 leftForce = rotorUp * ((upThrust * f) / 2);
		Vector3 rightForce  = rotorUp * ((upThrust * f) / 2);

		rigidBody().AddForceAtPosition(leftForce, thrustPointLeft);
		rigidBody().AddForceAtPosition(rightForce,  thrustPointRight);

	}
				
	public float TotalUpThrust() {
		float upThrust = 16f * UpDesire();

		if (IsHeavilyDamaged()) {
			upThrust *= Random.value;
		}

		return upThrust;
	}

	public void  ApplyRotorThrust() {
		// points around top rotor to apply force
		// a difference between the force applied to these causes chopper to tilt and then move forward or back

		float upThrust = TotalUpThrust();

		//ApplyRotorLRThrust();

		// forward/backward control ---------------------------------------------------

		float offset = 1.0f;
		Vector3 mainRotorThrustPointBack  = mainRotorTransform.position + mainRotorTransform.forward * offset;
		Vector3 mainRotorThrustPointFront = mainRotorTransform.position - mainRotorTransform.forward * offset;

		Vector3 rotorUp = mainRotorTransform.up;
		float speed = ForwardSpeed();
		float desiredSpeed = ForwardDesire()*4;
		float speedDiff = desiredSpeed - speed;
		float f = Mathf.Clamp(speedDiff/10, -4, 4);


		Vector3 frontForce = rotorUp * ((upThrust + f) / 2);
		Vector3 backForce  = rotorUp * ((upThrust - f) / 2);

		rigidBody().AddForceAtPosition(frontForce, mainRotorThrustPointFront);
		rigidBody().AddForceAtPosition(backForce,  mainRotorThrustPointBack);

	
		/*
		Debug.DrawLine(mainRotorThrustPointFront, mainRotorThrustPointFront + frontForce * 2.0f, Color.yellow); 
		Debug.DrawLine(mainRotorThrustPointBack,  mainRotorThrustPointBack  + backForce  * 2.0f, Color.blue); 
		*/

		Object_rotDY(mainRotor, 40f); //Mathf.Abs(upThrust*5.0f) + 20f);
		Object_rotDY (tailRotor, 40f);
	}

	public override void FixedUpdate () {
		if (isRunning) {
			PickTarget();
			SteerTowardsTarget();
			ApplyRotorThrust();
		}
		RemoveIfOutOfBounds();
	}

	public override void OnCollisionEnter(Collision collision) {
		base.OnCollisionEnter(collision);

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
