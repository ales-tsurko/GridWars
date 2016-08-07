using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour {
	public GameObject target;

	public bool isActive = true;
	public bool isFixed = true;
	public int ammoCount = -1;
	public float reloadTimeInSeconds = 2.0f;
	public float isReloadedAfterTime = 0;

	public void Start () {
		//base.Start();
	}

	public void FixedUpdate () {

		if (isActive) {
			FireIfAppropriate ();
			AimIfAble ();
		}

	}

	// --- aiming ------------------

	public static float AngleBetweenOnAxis(Vector3 v1, Vector3 v2, Vector3 n)
	{
		// Determine the signed angle between two vectors, 
		// with normal 'n' as the rotation axis.

		return Mathf.Atan2(
			Vector3.Dot(n, Vector3.Cross(v1, v2)),
			Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
	}

	public float AngleToTarget() {
		if (target) {
			Transform t = transform;
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

	public bool AimIfAble() { 
		if (target && !isFixed) {
			// assumes we can only rotate weapon about Y axis

			float angle = AngleToTarget();
			float dy = Mathf.Sign(angle) * Mathf.Sqrt(Mathf.Abs(angle)) * 0.01f; // hack for now

			Transform tt = transform;
			var e = tt.eulerAngles;
			tt.eulerAngles = new Vector3(e.x, e.y + dy, e.z);
			return true;
		}
		return false;
	}

	// --- firing ------------------

	public bool FireIfAppropriate() {
		if (hasAmmo() && isLoaded () && isAimed ()) {
			Fire ();
			return true;
		}
		return false;
	}

	public bool hasAmmo() {
		return (ammoCount == -1) || (ammoCount > 0);
	}

	public bool isLoaded() {
		return Time.time > isReloadedAfterTime;
	}

	public bool isAimed() {
		return AngleToTarget () < 0.0;
	}
		
	public void Fire() {
		isReloadedAfterTime = Time.time + reloadTimeInSeconds;
	}
}