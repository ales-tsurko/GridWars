using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ChopperConventional : AirVehicle {
	public float cruiseHeight;
	public float thrustHeight = 2f;

	public GameObject mainRotorFixed; // used to apply thrust - normal rotor spins
	public GameObject mainRotor;
	public GameObject tailRotor;

	[HideInInspector]
	public bool usesSoundtrack = true;
	public float damageRotation;

	public override void ServerAndClientJoinedGame() {
		base.ServerAndClientJoinedGame();
		UpdateSoundtrack();
		gameObject.TurnOffShadows(); // apply to prefab?
	}

	public override void ServerJoinedGame () {
		base.ServerJoinedGame();
		isRunning = true;

		cruiseHeight = 9f + Random.Range(-1.0f, 1.0f);

		damageRotation = (Random.value - 0.5f) * 10f;
		SetAllowFriendlyFire(false);

		AddStartingBoost();

		if (Minigun()) {
			Minigun().damageAdjustments.Add(typeof(MobileSAM), 0.75f);
		}
	}

	private void AddStartingBoost() {
		//rigidBody().velocity = new Vector3(0, 4f, 0f);
		_t.position = new Vector3(_t.position.x, _t.position.y*2, _t.position.z);
		//_t.position = new Vector3(_t.position.x, cruiseHeight*0.7f, _t.position.z);

		Vector3 e = _t.eulerAngles;
		e.x = 15f;
		_t.eulerAngles = e; //new Vector3(e.x, e.y, e.z);
	}

	public Weapon Minigun() {
		foreach (Weapon w in Weapons()) {
			if (w.name == "Minigun") {
				return w;
			}
		}
		return null;
	}

	public float TargetXZDistance() {
		Vector3 p1 = transform.position;
		p1.y = 0f;

		Vector3 p2 = target.transform.position;
		p2.y = 0f;

		return Vector3.Distance(p1, p2);
	}

	public float UpDesire() { // 0.0 to 1.0
		float ch = cruiseHeight;
		if (target == null) {
			ch = 0f;
		}

		float diff = ch - y ();
		return Mathf.Clamp(SmoothValue(diff)/2, 0f, 1f);
	}

	public float ForwardDesire() { // 0.0 to 1.0 
		if (!target) {
			return 0f;
		}
			
		if (!IsInStandoffRange()) {
			// don't tilt forward until we're roughly facing the target
			float angleDiff = Mathf.Abs(YAngleToTarget());
			if (angleDiff < 30) {
				float diff = TargetDistance() - standOffDistance;
				return Mathf.Clamp(diff, 0f, 1f);
			}
		}

		return 0f;
	}

	public float TiltRightDesire() { // -1.0 to 1.0
		Vector3 worldUp = new Vector3(0, 1, 0);
		float a = AngleBetweenOnAxis(_t.up, worldUp, _t.forward); // left is positive angle
		return Mathf.Clamp(a/10.0f, -1.0f, 1.0f)/5f;
	}

	public void  ApplyRotorLRThrust() { // z tilt control ------------------------------------------------------
		float upThrust = TotalUpThrust()/2f;

		float offset = 1f;
		Vector3 thrustPointLeft  = mainRotorFixed.transform.position - mainRotorFixed.transform.right * offset;
		Vector3 thrustPointRight = mainRotorFixed.transform.position + mainRotorFixed.transform.right * offset;

		float f = TiltRightDesire();

		Vector3 rotorUp = mainRotorFixed.transform.up;
		Vector3 leftForce  = rotorUp * ((upThrust - f) / 2);
		Vector3 rightForce = rotorUp * ((upThrust + f) / 2);

		rigidBody().AddForceAtPosition(leftForce,  thrustPointLeft);
		rigidBody().AddForceAtPosition(rightForce, thrustPointRight);

		//Debug.DrawLine(mainRotorFixed.transform.position, mainRotorFixed.transform.position + (mainRotorFixed.transform.up * transform.rotation.eulerAngles.z / 10f), Color.blue); 
		//Debug.DrawLine(thrustPointLeft, thrustPointLeft + leftForce , Color.black); 
		//Debug.DrawLine(thrustPointRight, thrustPointRight + rightForce , Color.black); 
	}
				
	public float TotalUpThrust() {
		float upThrust = thrust * UpDesire(); // * 1.5f;

		if (IsHeavilyDamaged()) {
			upThrust *= Random.value;
		}

		return upThrust;
	}

	public void  ApplyRotorThrust() {
		// points around top rotor to apply force
		// a difference between the force applied to these 
		// causes chopper to tilt and then move forward or back

		float upThrust = TotalUpThrust();

		ApplyRotorLRThrust();

		// forward/backward control ---------------------------------------------------

		float offset = 1f;
		Vector3 mainRotorThrustPointBack  = mainRotorFixed.transform.position + mainRotorFixed.transform.forward * offset;
		Vector3 mainRotorThrustPointFront = mainRotorFixed.transform.position - mainRotorFixed.transform.forward * offset;

		Vector3 rotorUp = mainRotorFixed.transform.up;
		float speed = ForwardSpeed();
		float desiredSpeed = ForwardDesire() * 4f;
		float speedDiff = desiredSpeed - speed;
		float f = Mathf.Clamp(speedDiff, -upThrust, upThrust) * 1f;

		Vector3 frontForce = rotorUp * ((upThrust + f) / 2);
		Vector3 backForce  = rotorUp * ((upThrust - f) / 2);

		rigidBody().AddForceAtPosition(frontForce, mainRotorThrustPointFront);
		rigidBody().AddForceAtPosition(backForce,  mainRotorThrustPointBack);

	
		//Debug.DrawLine(mainRotorThrustPointFront, mainRotorThrustPointFront + frontForce * 2.0f, Color.yellow); 
		//Debug.DrawLine(mainRotorThrustPointBack,  mainRotorThrustPointBack  + backForce  * 2.0f, Color.blue); 
	}

	public void SpinRotors() {
		// rotors don't look right except at certain speeds, so hard wire this
		float r = Random.value;
		float t = TotalUpThrust();

		if (mainRotor != null) {
			Object_rotDY(mainRotor, 20f + t*r); //Mathf.Abs(upThrust*5.0f) + 20f);
		}

		if (tailRotor != null) {
			Object_rotDX(tailRotor, 20f + 20f * r);
		}
	}

	public override void ServerFixedUpdate () {
		base.ServerFixedUpdate();
		if (isRunning) {
			PickTarget();
			SteerTowardsTarget();
			ApplyRotorThrust();
		}

		DieIfOverAccelerated();

		RemoveIfOutOfBounds();
	}

	private void DieIfOverAccelerated() {
		if (rigidBody().velocity.magnitude > 20f) {
			Die();
		}
	}

	public override void ServerAndClientFixedUpdate () {
		base.ServerAndClientFixedUpdate();
		SpinRotors();
	}

	public override void OnCollisionEnter(Collision collision) {
		base.OnCollisionEnter(collision);

		// destroy on ground collision
		if (collision.collider.name == "BattlefieldPlane") {
			if (collision.relativeVelocity.magnitude > 2) {
				Die();
			}
		}
	}

	override public void ApplyDamage(float damage) {
		base.ApplyDamage(damage);
	}

	// Soundtrack ----------------------------------------

	override public void ServerAndClientLeftGame(){
		base.ServerAndClientLeftGame();
		UpdateSoundtrack();
	}

	public int PlayerChopperCount() {
		var choppers = player.units.Where(unit => unit.IsOfType(typeof(Chopper))).ToList<GameUnit>();
		return choppers.Count;
	}

	public Soundtrack Soundtrack() {
		return App.shared.SoundtrackNamed("Wagner_Ride_of_the_Valkyries");
	}

	public void UpdateSoundtrack() {
		if (usesSoundtrack) {
			float count = (float)PlayerChopperCount();

			float threshold = 4f;

			if (count > 4) {
				Soundtrack().Play();
				Soundtrack().SetTargetVolume(1f);
			}

			if (count < 4) {
				Soundtrack().SetTargetVolume(count / threshold);
			}
		}
	}
}
