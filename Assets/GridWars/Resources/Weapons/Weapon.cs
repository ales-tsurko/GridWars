using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour {
	public Player player;
	public GameObject owner;
	public GameObject target;
	public GameObject prefabProjectile;

	public bool isActive = false;
	public bool isFixed = true;
	public int ammoCount = -1;
	public float reloadTimeInSeconds = 3.0f;
	public Quaternion rotOffset;
	public float range = -1;
	public float aimedAngle = 5.0f;

	public AudioClip fireClip;

	[HideInInspector]
	float isReloadedAfterTime = 2;

	//public Vector3 angularSpread; // accuracy in euler angles

	public void Start () {
		//base.Start();
		Reload();

		if (fireClip != null) {
			gameObject.AddComponent<AudioSource>();
		}
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
		//print("AimIfAble1");

		if (target && !isFixed) {
			//print("AimIfAble2");

			// assumes we can only rotate weapon about Y axis

			float angle = AngleToTarget();
			float dy = Mathf.Sign(angle) * Mathf.Sqrt(Mathf.Abs(angle)) * 0.05f; // hack for now

			Transform tt = transform;
			var e = tt.eulerAngles;
			tt.eulerAngles = new Vector3(e.x, e.y + dy, e.z);
			return true;
		}
		return false;
	}

	// --- firing ------------------

	public bool FireIfAppropriate() {
		if (hasAmmo() && isLoaded () && isAimed () && targetInRange()) {
			Fire ();
			return true;
		}
		return false;
	}

	public float targetDistance() {
		return Vector3.Distance(owner.transform.position, target.transform.position);
		//return owner.transform.position.Distance(target.transform.position);
	}
	
	public bool targetInRange() {
		return (range == -1) || (targetDistance() < range);
	}

	public bool hasAmmo() {
		return (ammoCount == -1) || (ammoCount > 0);
	}

	public bool isLoaded() {
		return Time.time > isReloadedAfterTime;
	}

	public bool isAimed() {
		//return true;
		return Mathf.Abs(AngleToTarget ()) < aimedAngle;
	}

	public void Reload() {
		if (hasAmmo()) {
			if (ammoCount > 0) {
				ammoCount--;
			}
			isReloadedAfterTime = Time.time + reloadTimeInSeconds;
		}
	}

	public void Fire() {
		CreateProjectile();
		if (fireClip != null) {
			GetComponent<AudioSource>().PlayOneShot(fireClip);
		}
		Reload();
	}

	public float barrelLength() {
		Collider ownerCollider = owner.GetComponent<Collider>();
		float maxZ = ownerCollider.bounds.size.z;
		return maxZ * 0.5f * 2.1f; // put it outside
	}

	Projectile CreateProjectile() {

		//print("CreateProjectile");

		var obj = Instantiate(prefabProjectile);
		obj.transform.position = transform.position + (transform.forward * barrelLength());
		obj.transform.rotation = transform.rotation;

		//Transform t = obj.transform;
		//obj.transform.eulerAngles = t.eulerAngles + rotOffset.eulerAngles;

		var unit = obj.GetComponent<Projectile>();
		unit.copyVelocityFrom(owner);
		unit.player = player;

		return unit;
	}
}