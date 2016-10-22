using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Vehicle : GameUnit  {

	public float maxSpeed = 10f;
	//public float timeToMaxSpeed = 1f;

	public float damageThrustAdjustment = 0.1f;
	public float maxRotYDegPerSec = 20f; 
	public float lastRotY = -1; 
	public float rotDY = 0; 

	[HideInInspector]

	bool disableCollisionsOnLaunch = true;
	bool hasVehicleCollisionsOn;
	GameObject nearestVehicle;

	// some code to avoid vehicle collisions on launch from tower
	// on start, we disable collisions with all extant same player vehicles
	// each frame we test for vehicle box collision,
	// when it fails we enable vehicle collisions

	/*
	public virtual void SetThrustForMaxSpeed(float speed) { // world units per second
		Rigidbody rb = GetComponent<Rigidbody>();
		thrust = speed / rb.mass; // need to adjust for drag
	}
	*/


	virtual public float AvailableThrust() {
		/*
		if (ForwardSpeed() < maxSpeed) {
			float r = damageThrustAdjustment;
			float entropy = (1 - UnityEngine.Random.value * 0.1f);
			return thrust * ((1.0f - r) + (hpRatio * r)) * entropy;
			return thrust;
		}

		return 0f;
		*/

		float r = damageThrustAdjustment;
		float entropy = (1 - UnityEngine.Random.value * 0.1f);
		return thrust * ((1.0f - r) + (hpRatio * r)) * entropy;
	}


	virtual public float AvailableRotationThrust() {
		float r = damageThrustAdjustment;
		float entropy = (1 - UnityEngine.Random.value * 0.1f);
		return rotationThrust *  ((1.0f - r) + (hpRatio * r)) * entropy;
	}

	// NetworkObject ----------------------------------------

	public override void ServerAndClientInit() {
		base.ServerAndClientInit();
		if (disableCollisionsOnLaunch) {
			DisableVehicleCollisions();
		}
	}

	public override void ServerAndClientJoinedGame() {
		base.ServerAndClientJoinedGame();

		App.shared.stepCache.AddVehicle(this);
	}

	public override void ServerAndClientLeftGame() {
		base.ServerAndClientLeftGame();

		App.shared.stepCache.RemoveVehicle(this);
	}

	public override void ServerFixedUpdate() {
		base.ServerFixedUpdate();
		/*
		if (disableCollisionsOnLaunch) {
			EnableVehicleCollisionsIfClear();
		}
		*/
		//EnforceMaxRotYDegPerSec();
	}

	/*
	public void EnforceMaxRotYDegPerSec() {
		Vector3 av = rigidBody().angularVelocity;

		if (Mathf.Abs(av.y) > maxRotYDegPerSec * 0.0174533f) {
			av.y = Mathf.Sign(av.y) * maxRotYDegPerSec;
			rigidBody().angularVelocity = av;
		}
	}
	*/

	public virtual void SteerTowardsTarget() {
		if (target != null) {
			RotateTowardTarget();
		}
		RotateAwayFromNearestObsticle();
	}

	public override void Think() {
		base.Think();
		UpdateNearestObstacle();
	}

	private GameObject nearestObsticle;
	private float avoidObsticleDistance = 4f;

	virtual public void UpdateNearestObstacle() {
		// find nearest object that's 
		// - a vehicle
		// - not our target
		// - not us

		List <GameObject> vehicles = new List<GameObject>(App.shared.stepCache.AllVehicleObjects());
		//vehicles.AddRange(App.shared.stepCache.AllWreckageObjects()); // wreckage no longer has collisions with units
		vehicles.Remove(gameObject);

		nearestObsticle = ClosestOfObjects(vehicles);

		if (nearestObsticle != null) {
			if (DistanceToObj(nearestObsticle) > avoidObsticleDistance) {
				nearestObsticle = null;
			}
		}

		/*
		if (nearestObsticle != null) {
			Vector3 p1 = _t.position;
			p1.y += 0.5f;

			Vector3 p2 = nearestObsticle.transform.position;
			p2.y += 0.5f;

			DebugLine.DrawLine (p1, p2, Color.white, 1, 3);  
		}
		*/
	}

	public virtual void RotateAwayFromNearestObsticle() {
		if (nearestObsticle) {
			float r = (avoidObsticleDistance - DistanceToObj(nearestObsticle)) / avoidObsticleDistance;
			float desire =  SmoothValue(1f - r*r);
			desire = SmoothValue(desire);
			desire = SmoothValue(desire);

			var otherPos = nearestObsticle.transform.position;
			Vector3 dir = (otherPos - _t.position).normalized;
			float angleToTarget = AngleBetweenOnAxis(_t.forward, dir, _t.up);

			if (Mathf.Abs(angleToTarget) < 90f) {
				float rotdy = rigidBody().angularVelocity.y * 57.2958f; // degrees per second

				bool isTurningAway = Mathf.Sign(rotdy) == -Mathf.Sign(angleToTarget);

				if (!isTurningAway || Math.Abs(rotdy) < maxRotYDegPerSec) {
					float f = 1f; //0.4f; 
					rigidBody().AddTorque(_t.up * (-angleToTarget) * f * desire * AvailableRotationThrust(), ForceMode.Force);
					
					//Debug.DrawLine(_t.position, _t.position + _t.forward * 10.0f, Color.blue); // forward blue
					//Debug.DrawLine(_t.position, _t.position + dir * 10.0f, Color.yellow); // targetDir yellow
					//Debug.DrawLine(_t.position, _t.position + dir * AvailableRotationThrust, Color.red); // targetDir red
				}
			}
		}
	}

	#if UNITY_EDITOR

	public override void OnDrawGizmos() {
		base.OnDrawGizmos();
		if (nearestObsticle != null) {
			//Debug.DrawLine (_t.position, nearestObsticle.transform.position, Color.red, 1, false);  
		}
	}

	#endif

	public Vector3 TargetMovePos() {
		if (target != null) {

			/*
			if (nearestObsticle != null) {
				var otherPos = nearestObsticle.transform.position;
			}
			*/
				
			return target.transform.position;
		}
		return new Vector3();
	}

	public float YAngleToTargetMovePos() {
		Vector3 targetDir = (TargetMovePos() - _t.position).normalized;
		return AngleBetweenOnAxis(_t.forward, targetDir, _t.up);
	}

	public virtual void RotateTowardTarget() {
		float ya = YAngleToTargetMovePos();

		//Debug.DrawLine(_t.position, _t.position + _t.forward*10.0f, Color.blue); // forward blue
		//Debug.DrawLine(_t.position, _t.position + targetDir*10.0f, Color.yellow); // targetDir yellow
		//Debug.DrawLine(_t.position, _t.position + targetDir*AvailableRotationThrust(), Color.red); // targetDir red

		rigidBody().AddTorque( _t.up * ya * AvailableRotationThrust(), ForceMode.Force);
	}

	// --- Utility methods -----------------------------------------

	public bool IsHeavilyDamaged() {
		return (hpRatio < .25);
	}

	public float SmoothValue(float v) {
		return Mathf.Sign(v) * Mathf.Sqrt(Mathf.Abs(v));
	}

	public float SmoothValue2(float v) {
		return Mathf.Sign(v) * Mathf.Pow(Mathf.Abs(v), -2);
	}

	public float SmoothValue3(float v) {
		return Mathf.Sign(v) * Mathf.Pow(Mathf.Abs(v), -3);
	}

	public float SmoothValue4(float v) {
		return Mathf.Sign(v) * Mathf.Pow(Mathf.Abs(v), -4);
	}


	public float SharpenValue(float v) {
		return Mathf.Sign(v) * v * v;
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


	public IEnumerable<Vehicle> AllVehicleUnits() {
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

	public void IgnoreCollisionsWithUnits(IEnumerable<GameUnit> units, bool ignore) {
		foreach (var unit in units) {
			if (unit != this) {
				Physics.IgnoreCollision(unit.BoxCollider(), BoxCollider(), ignore);
			}
		}
	}

	public void DisableVehicleCollisions() {
		if (hasVehicleCollisionsOn) {
			hasVehicleCollisionsOn = false;
			IgnoreCollisionsWithUnits(AllVehicleUnits().Cast<GameUnit>(), hasVehicleCollisionsOn);
		}
	}
		
	public void EnableVehicleCollisions() {

		if (!hasVehicleCollisionsOn) {
			hasVehicleCollisionsOn = true;
			IgnoreCollisionsWithUnits(AllVehicleUnits().Cast<GameUnit>(), hasVehicleCollisionsOn);
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
