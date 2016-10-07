using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Chopper : AirVehicle {
	public float cruiseHeight;
	public float thrustHeight = 2f;

	public GameObject mainRotorFixed; // used to apply thrust - normal rotor spins
	public GameObject mainRotor;
	public GameObject tailRotor;

	public GameObject leftJet;
	public GameObject rightJet;
	public GameObject rearJet;

	public float maxForwardSpeed;

	[HideInInspector]
	//public bool usesSoundtrack = false;
	float damageRotation;

	/*
	public override void ServerAndClientJoinedGame() {
		base.ServerAndClientJoinedGame();
		//UpdateSoundtrack();
		//gameObject.TurnOffShadows(); // apply to prefab?
	}
	*/

	public override void ServerJoinedGame () {
		base.ServerJoinedGame();
		isRunning = true;

		//cruiseHeight = 9f + Random.Range(-1.0f, 1.0f);

		damageRotation = (Random.value - 0.5f) * 10f;
		SetAllowFriendlyFire(false);

		if (Minigun()) {
			Minigun().damageAdjustments.Add(typeof(MobileSAM), 0.6f);
			Minigun().damageAdjustments.Add(typeof(Tower), 0.25f);
		}
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

		float diff = ( ch - y() ) / ch; 

		//diff = SmoothValue(diff);

		return Mathf.Clamp(diff, -1f, 1f);
	}

	public float ForwardDesire() { // 0.0 to 1.0 
		if (!target) {
			return 0f;
		} 

		float s = ForwardSpeed();
		//float angleDiff = Mathf.Abs(YAngleToTarget());
		float diff = (TargetDistance() - (standOffDistance + s))/standOffDistance;
		float v = diff;
		//v *= 1f - (Mathf.Clamp(angleDiff, 0f, 30f) / 30f);
		v = SmoothValue(v);
		v = Mathf.Clamp(v, 0f, 1f);


		//Debug.DrawLine(_t.position, _t.position + _t.up * v * 10f, Color.yellow); 
		//Debug.DrawLine(_t.position, _t.position + (target.transform.position - _t.position).normalized * (TargetDistance() - standOffDistance), Color.yellow); 

		return v;

	}

	public float TiltRightDesire() { // -1.0 to 1.0
		Vector3 worldUp = new Vector3(0, 1, 0);
		float a = AngleBetweenOnAxis(_t.up, worldUp, _t.forward); // left is positive angle
		//return Mathf.Clamp(a/10.0f, -1.0f, 1.0f)/5f;
		return Mathf.Clamp(a/10.0f, -1.0f, 1.0f)/3f;
	}

	public float TotalUpThrust() {
		float upThrust = thrust * UpDesire(); 

		if (IsHeavilyDamaged()) {
			upThrust *= Random.value;
		}

		return upThrust;
	}

	public void PositionJets() {
		float speed = ForwardSpeed();

		float xr = (- 90f) + Mathf.Clamp(speed, 0f, 90f) * 10f;
		xr = Convert180to360(xr);
		Object_setRotX(leftJet, xr);
		Object_setRotX(rightJet, xr);
	}


	public void ApplyJetThrustNew() {

		float thrust = TotalUpThrust();
		Vector3 ud = _t.transform.up * UpDesire();
		Vector3 fd = _t.transform.forward *ForwardDesire();

		Vector3 dir = (ud + fd).normalized;
		float angle = AngleBetweenOnAxis(_t.forward, dir, _t.right);

		//angle = angle;
		//angle = -90f;
		Object_setRotX(leftJet, angle);
		Object_setRotX(rightJet, angle);

		Vector3 mainForce = dir * thrust;
		rigidBody().AddForceAtPosition(mainForce, _t.transform.position);


		Debug.DrawLine(leftJet.transform.position, leftJet.transform.position + leftJet.transform.forward * thrust, Color.yellow); 
		Debug.DrawLine(leftJet.transform.position, leftJet.transform.position + leftJet.transform.forward * thrust, Color.yellow); 
	}

	public void ApplyJetThrust() {
/*
		Vector3 leftForce  = leftJet.transform.forward  * (thrust / 2f);
		Vector3 rightForce = rightJet.transform.forward * (thrust / 2f);
			
		rigidBody().AddForceAtPosition(leftForce,  leftJet.transform.position);
		rigidBody().AddForceAtPosition(rightForce, rightJet.transform.position);
		*/
		rigidBody().AddForce(_t.up * TotalUpThrust());
		rigidBody().AddForce(_t.forward * ForwardDesire() * thrust);

		PositionJets();
	}

	public override void ServerFixedUpdate () {
		base.ServerFixedUpdate();
		if (isRunning) {
			PickTarget();
			SteerTowardsTarget();
			ApplyJetThrust();
			DieIfOverAccelerated();
		}
			
		RemoveIfOutOfBounds();
	}

	private void DieIfOverAccelerated() {
		if (rigidBody().velocity.magnitude > 40f) {
			Die();
		}
	}
		
	/*
	public override void ServerAndClientFixedUpdate () {
		base.ServerAndClientFixedUpdate();
		PositionJets();
	}
	*/

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

	/*
	override public void ServerAndClientLeftGame(){
		base.ServerAndClientLeftGame();
		UpdateSoundtrack();
	}
	*/

	public int PlayerChopperCount() {
		var choppers = player.units.Where(unit => unit.IsOfType(typeof(Chopper))).ToList<GameUnit>();
		return choppers.Count;
	}

	public Soundtrack Soundtrack() {
		return App.shared.SoundtrackNamed("Wagner_Ride_of_the_Valkyries");
	}
}
