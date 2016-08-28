using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Vehicle : GameUnit  {

	[HideInInspector]
	public bool hasVehicleCollisionsOn;

	GameObject nearestVehicle;

	// some code to avoid vehicle collisions on launch from tower
	// on start, we disable collisions with all extant same player vehicles
	// each frame we test for vehicle box collision,
	// when it fails we enable vehicle collisions

	public override void Start() {
		base.Start();
		//DisableVehicleCollisions();
	}

	/*
	public virtual void SetThrustForMaxSpeed(float speed) { // world units per second
		Rigidbody rb = GetComponent<Rigidbody>();
		thrust = speed / rb.mass; // need to adjust for drag
	}
	*/

	public override void MasterFixedUpdate() {
		base.MasterFixedUpdate();
		//EnableVehicleCollisionsIfClear();
	}

	public virtual void SteerTowardsTarget() {
		if (target != null) {
			RotateTowardObject (target);
		}
		RotateAwayFromNearestObsticle();
	}

	public override void Think() {
		base.Think();
		UpdateNearestObsticle();
	}

	private GameObject nearestObsticle;
	private float avoidObsticleDistance = 6f;

	public void UpdateNearestObsticle() {
		// find nearest object that's 
		// - a vehicle
		// - not our target
		// - not us

		List <GameObject> vehicles = App.shared.stepCache.AllVehicleObjects();
		vehicles.Remove(gameObject);

		nearestObsticle = ClosestOfObjects(vehicles);

		if (nearestObsticle != null) {
			if (DistanceToObj(nearestObsticle) > avoidObsticleDistance) {
				nearestObsticle = null;
			}
		}
	}

	public virtual void RotateAwayFromNearestObsticle() {
		if (nearestObsticle) {
			float r = (avoidObsticleDistance - DistanceToObj(nearestObsticle)) / avoidObsticleDistance;
			float desire =  1f - r*r;
			//float desire = 1f;

			var otherPos = nearestObsticle.transform.position;
			Vector3 dir = (otherPos - _t.position).normalized;
			float angleToTarget = AngleBetweenOnAxis(_t.forward, dir, _t.up);

			Debug.DrawLine(_t.position, _t.position + _t.forward*10.0f, Color.blue); // forward blue
			Debug.DrawLine(_t.position, _t.position + dir*10.0f, Color.yellow); // targetDir yellow
			Debug.DrawLine(_t.position, _t.position + dir*rotationThrust, Color.red); // targetDir red

			rigidBody().AddTorque(_t.up * (-angleToTarget) * .4f * desire * rotationThrust, ForceMode.Force);
		}
	}


	public virtual void RotateTowardObject(GameObject obj) {
		/*
		var targetPos = obj.transform.position;

		Vector3 targetDir = (targetPos - _t.position).normalized;
		float angleToTarget = AngleBetweenOnAxis(_t.forward, targetDir, _t.up);
		*/

		float ya = YAngleToTarget();

		/*
		Debug.DrawLine(_t.position, _t.position + _t.forward*10.0f, Color.blue); // forward blue
		Debug.DrawLine(_t.position, _t.position + targetDir*10.0f, Color.yellow); // targetDir yellow
		Debug.DrawLine(_t.position, _t.position + targetDir*rotationThrust, Color.red); // targetDir red
		*/

		rigidBody().AddTorque( _t.up * ya * rotationThrust, ForceMode.Force);
	}

	// --- Utility methods -----------------------------------------

	public bool IsHeavilyDamaged() {
		return (hpRatio < .5);
	}

	public float SmoothValue(float v) {
		return Mathf.Sign(v) * Mathf.Sqrt(Mathf.Abs(v));
	}
		
	public Vector3 LocalVelocity() {
		return transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity);
	}

	public float ForwardSpeed() {
		return LocalVelocity().z;
	}

	public float RightSpeed() {
		return LocalVelocity().x;
	}


	public List<GameUnit> AllVehicleUnits() {
		return App.shared.stepCache.AllVehicleUnits();
	}
		
	/*
	public void IgnoreCollisionsWithUnitsOfType(System.Type unitType, bool ignore) {
		GameObject[] objs = (GameObject[])UnityEngine.Object.FindObjectsOfType(unitType) ;
		foreach (var obj in objs) {
			GameUnit unit = obj.GameUnit();
			if (unit != this) {
				Physics.IgnoreCollision(unit.BoxCollider(), BoxCollider(), ignore);
			}
		}
	}
	*/

	public void IgnoreCollisionsWithUnits(List <GameUnit> units, bool ignore) {
		foreach (var unit in units) {
			if (unit != this) {
				Physics.IgnoreCollision(unit.BoxCollider(), BoxCollider(), ignore);
			}
		}
	}

	public void DisableVehicleCollisions() {
		if (hasVehicleCollisionsOn) {
			hasVehicleCollisionsOn = false;
			IgnoreCollisionsWithUnits(AllVehicleUnits(), hasVehicleCollisionsOn);
		}
	}
		
	public void EnableVehicleCollisions() {
		if (!hasVehicleCollisionsOn) {
			hasVehicleCollisionsOn = true;
			IgnoreCollisionsWithUnits(AllVehicleUnits(), hasVehicleCollisionsOn);
		}
	}


	public void EnableVehicleCollisionsIfClear() {
		if (hasVehicleCollisionsOn == false) {
			if (IsIntersectingWithVehicle() == false) {
				EnableVehicleCollisions();
			}
		}
	}

	public bool IsIntersectingWithVehicle() {
		if (!hasVehicleCollisionsOn) {
			BoxCollider myCollider = BoxCollider();

			foreach (var vehicle in AllVehicleUnits()) {
				BoxCollider otherCollider = vehicle.BoxCollider();

				if (myCollider.bounds.Intersects(otherCollider.bounds)) {
					return true;
				}
			}
		}
		return false;
	}

}
