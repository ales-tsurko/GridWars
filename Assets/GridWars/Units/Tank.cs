using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tank : GameUnit {
	public override void Start () {
		base.Start();
		thrust = 200;
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

	public void aimTurret() {

		if (target) {
			Transform t = turret ().transform;
			var targetPos = target.transform.position;

			Vector3 targetDir = (targetPos - t.position).normalized;
			float angle = AngleSigned(t.forward, targetDir, t.up);

			if (true) {
				Debug.DrawLine(t.position, t.position + t.forward*10.0f, Color.blue); // forward blue
				Debug.DrawLine(t.position, t.position + targetDir*10.0f, Color.yellow); // targetDir yellow
			}
				
			float newAngle = -angle;
			float dy = Mathf.Sign(angle) * Mathf.Sqrt(Mathf.Abs(angle)) * 0.01f;


			var e = t.eulerAngles;
			float oldAngle = e.y;
			t.eulerAngles = new Vector3(e.x, e.y + dy, e.z);

		}
	}

}