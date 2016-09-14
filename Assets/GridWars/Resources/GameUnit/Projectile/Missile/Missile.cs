using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Missile : Projectile {

	public bool isSeeking = true;

	public override void ServerJoinedGame () {
		thrust = 10f;
		base.ServerJoinedGame();
	}

	public override void ServerFixedUpdate () {
		if (isSeeking && target != null) {
			// control thrust vectoring
					//Vector3 tpos = TargetLeadPosition();
					Vector3 tpos = target.GameUnit().ColliderCenter();

			Vector3 targetDir = (tpos - _t.position).normalized;

			//rigidBody().AddForce(targetDir * thrust);

			// these angles are zero when missile is on (a stationary) target

			float upAngle    = AngleBetweenOnAxis(_t.forward, targetDir, _t.up);
			upAngle = Mathf.Clamp(upAngle, -3f, 3f);

			float rightAngle = AngleBetweenOnAxis(_t.forward, targetDir, _t.right);
			rightAngle = Mathf.Clamp(rightAngle, -3f, 3f);

			float aThrust = thrust / 100.0f;
			/*
			rigidBody().AddForce(transform.up * upAngle * aThrust);
			rigidBody().AddForce(transform.right * rightAngle * aThrust);
			rigidBody().AddForce(transform.forward * thrust);
			*/

			rigidBody().AddTorque(_t.up * (upAngle) * aThrust, ForceMode.Force);
			rigidBody().AddTorque(_t.right * (rightAngle) * aThrust, ForceMode.Force);
			rigidBody().AddForce(transform.forward * thrust*2f);

		} else {
			// simple thrust
			rigidBody().AddForce(transform.forward * thrust);
		}
			
	}

	Vector3 TargetVelocity() {
		Rigidbody rb = target.GetComponent<Rigidbody>();
		if (rb) {
			return rb.velocity;
		}
		return new Vector3(0, 0, 0);
	}
		
	public Vector3 TargetLeadPosition() {
		float d = DistanceToObj(target);
		//float accleration = thrust / rigidBody.mass;
		float speed = rigidBody().velocity.magnitude;

		float leadTime = d/speed;

		if (leadTime < 1.2f) {
			leadTime = 1.2f;
		}

		Vector3 pos = target.GameUnit().ColliderCenter();
		Rigidbody rb = target.GetComponent<Rigidbody>();

		if (rb) {
			return pos + TargetVelocity() * leadTime;
		}

		return pos;
	}
}
