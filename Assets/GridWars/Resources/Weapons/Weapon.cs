using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour {
	public Player player;
	public GameObject owner;
	public GameObject target;
	public GameObject prefabProjectile;
	public GameObject muzzleFlash;
	public Transform muzzleFlashPoint;

	public bool isActive = true;
	public int ammoCount = -1;
	public float reloadTimeInSeconds = 3.0f;
	public float range = -1;
	public float aimedAngle = 5.0f;
	public float chanceOfFire = 0.02f; // as fraction of 1

	public AudioClip fireClip;

	public GameObject turretObjX = null; // need to set this to the obj that X axis will rotate on to aim
	public float turretMinX = -180;
	public float turretMaxX = 180;

	public GameObject turretObjY = null; // need to set this to the obj that Y axis will rotate on to aim
	public float turretMinY = -180;
	public float turretMaxY = 180;

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

	public float XAngleToTarget() {
		if (target) {
			Transform t = turretObjX.transform;
			var targetPos = target.transform.position;

			Vector3 targetDir = (targetPos - t.position).normalized;
			float angle = AngleBetweenOnAxis (t.forward, targetDir, t.right);


			if (true) {
				var r = range == -1 ? 1000 : range;

				Debug.DrawLine (t.position, t.position + t.forward * r, Color.blue); // forward 
				Debug.DrawLine (t.position, t.position + targetDir * r, Color.blue); // targetDir 
			}

			return angle;
		}

		return 0;
	}

	public float YAngleToTarget() {
		if (target) {
			Transform t = turretObjY.transform;
			var targetPos = target.transform.position;

			Vector3 targetDir = (targetPos - t.position).normalized;
			float angle = AngleBetweenOnAxis (t.forward, targetDir, t.up);

			/*
			if (true) {
				var r = range == -1 ? 10 : range;

				Debug.DrawLine (t.position, t.position + t.forward * r, Color.yellow); // forward 
				Debug.DrawLine (t.position, t.position + targetDir * r, Color.yellow); // targetDir 
			}
			*/


			return angle;
		}

		return 0;
	}

	public void AimOnXAxis() {
		float angle = XAngleToTarget();
		float dx = Mathf.Sign(angle) * Mathf.Sqrt(Mathf.Abs(angle)) * 0.05f; // hack for now

		Transform tt = turretObjX.transform;
		var e = tt.eulerAngles;
		tt.eulerAngles = new Vector3(e.x +dx, e.y, e.z);
	}

	public void AimOnYAxis() {
		float angle = YAngleToTarget();
		float dy = Mathf.Sign(angle) * Mathf.Sqrt(Mathf.Abs(angle)) * 0.05f; // hack for now

		Transform tt = turretObjY.transform;
		var e = tt.eulerAngles;
		tt.eulerAngles = new Vector3(e.x, e.y + dy, e.z);
	}

	public bool canRotateX() {
		return turretObjX != null;
	}

	public bool canRotateY() {
		return turretObjY != null;
	}

	public string ownerType() {
		return owner.GetType().ToString();
		//return gameObject.root.GetType().ToString();
	}

	public bool AimIfAble() { 

		//print(ownerType() + " AimIfAble ");

		if (target) {
			
			if (canRotateX()) {
				AimOnXAxis();
			}

			if (canRotateY()) {
				AimOnYAxis();
			}

			return true;
		}
		return false;
	}

	// --- firing ------------------

	public bool chooseToFire() {
		return Random.value > chanceOfFire; 
	}

	public bool FireIfAppropriate() {
		//print("FireIfAppropriate");
		if (hasAmmo() && isLoaded () && isAimed () && targetInRange() && chooseToFire()) {
			Fire ();
			return true;
		}
		return false;
	}

	public float targetDistance() {
		return Vector3.Distance(owner.transform.position, target.transform.position);
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
		float diff = 0;

		if (turretObjY) {
			diff += Mathf.Abs(YAngleToTarget());
		}

		if (turretObjX) {
			diff += Mathf.Abs(XAngleToTarget());
		}

		return diff < aimedAngle;
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

	void CreateMuzzleFlash(){
		if (muzzleFlashPoint == null || muzzleFlash == null) {
			return;
		}
		Instantiate (muzzleFlash, muzzleFlashPoint.position, muzzleFlashPoint.rotation);
	}

	Projectile CreateProjectile() {

		//print("CreateProjectile");
		if (muzzleFlash != null) {
			CreateMuzzleFlash ();
		}
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