using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Vehicle : GameUnit  {

	[HideInInspector]
	public bool hasVehicleCollisionsOn;

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

	public bool IsHeavilyDamaged() {
		return ((hitPoints / maxHitPoints) < .5);
	}

	public float SmoothValue(float v) {
		return Mathf.Sign(v)*Mathf.Sqrt(Mathf.Abs(v));
	}
		
	public float ForwardSpeed() {
		var localVelocity = transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity);
		return localVelocity.z;
	}

	public float RightSpeed() {
		var localVelocity = transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity);
		return localVelocity.x;
	}

	public List<GameUnit> AllVehicles() {
		List <GameObject> objs = activeGameObjects();
		var results = new List<GameUnit>();

		foreach (GameObject obj in objs) {
			GameUnit unit = obj.GameUnit();

			if (unit && unit.IsOfType(typeof(Vehicle))) {
				results.Add(unit);
			}
		}

		return results;
	}
		
	/*
	public List<Vehicle> OwnVehilces() {
		var results = new List<Vehicle>();

		foreach (Vehicle vehicle in AllVehicles()) {
			if (vehicle.player == player) {
				results.Add(vehicle);
			}
		}

		return results;
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
			IgnoreCollisionsWithUnits(AllVehicles(), hasVehicleCollisionsOn);
		}
	}
		
	public void EnableVehicleCollisions() {
		if (!hasVehicleCollisionsOn) {
			hasVehicleCollisionsOn = true;
			IgnoreCollisionsWithUnits(AllVehicles(), hasVehicleCollisionsOn);
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

			foreach (var vehicle in AllVehicles()) {
				BoxCollider otherCollider = vehicle.BoxCollider();

				if (myCollider.bounds.Intersects(otherCollider.bounds)) {
					return true;
				}
			}
		}
		return false;
	}

	/*
	public override void MasterFixedUpdate() {
		base.MasterFixedUpdate();
		//EnableVehicleCollisionsIfClear();
	}
	*/
}
