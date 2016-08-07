using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Turret : GameUnit {

	public override void Start () {
		base.Start();
	}

	GameObject turret() {
		return _t.Find("headdus1").gameObject;
	}


	public override void FixedUpdate () {


		pickTarget ();

		if (target) {

			if (Mathf.Abs (angleToTarget) < 45) {
				base.FixedUpdate ();

			}

			steerTowardsNearestEnemy ();
			aimTurret ();
		}
	}

	public float turrentAngleToTarget() {
		if (target) {
			Transform t = turret ().transform;
			var targetPos = target.transform.position;

			Vector3 targetDir = (targetPos - t.position).normalized;
			float angle = AngleBetweenOnAxis (t.forward, targetDir, t.up);

			if (true) {
				Debug.DrawLine (t.position, t.position + t.forward * 10.0f, Color.blue); // forward blue
				Debug.DrawLine (t.position, t.position + targetDir * 10.0f, Color.yellow); // targetDir yellow
			}

			return angle;
		}

		return 0;
	}

	public void aimTurret() {
		if (target) {
			float angle = turrentAngleToTarget();
			float dy = Mathf.Sign(angle) * Mathf.Sqrt(Mathf.Abs(angle)) * 0.01f;

			Transform tt = turret ().transform;
			var e = tt.eulerAngles;
			tt.eulerAngles = new Vector3(e.x, e.y + dy, e.z);

		}
	}

	public void fireAtWill() {
		if (turrentAngleToTarget () < 0.0) {
			fire ();
		}
	}

	public void fire() {

	}

}