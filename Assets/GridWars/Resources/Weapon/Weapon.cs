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
	public int maxAmmoCount = 0;
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

	public float aimRateX;
	public float aimRateY;

	public bool canTargetGround = true;
	public bool canTargetAir = true;

	//public bool usesRayCastAimCheck = false;

	[HideInInspector]
	float isReloadedAfterTime = 2;



	// targeting

	public int priority = 0; // vehicle's uses target chosen by highest priority weapon
	public List<System.Type> targetableTypes;


	public void Start () {
		//base.Start();
		Reload();

		targetableTypes = new List<System.Type>();

		if (canTargetGround) {
			targetableTypes.Add(typeof(GroundVehicle));
			targetableTypes.Add(typeof(GroundBuilding));
		}

		if (canTargetAir) {
			targetableTypes.Add(typeof(AirVehicle));
		}

		//targetableTypes = new List<System.Type>(){ typeof(GroundVehicle), typeof(GroundBuilding), typeof(AirVehicle) };

		if (fireClip != null) {
			gameObject.AddComponent<AudioSource>();
		}
	}

	public void FixedUpdate () {
		if (isActive) {
			PickTarget();
			FireIfAppropriate();
			AimIfAble();
		} else {
			target = null;
		}
	}

	// --- target selection -------------------

	public virtual bool CanTargetClassOfUnit(GameUnit unit) {
		System.Type unitType = unit.GetType();

		foreach (System.Type targetableClass in targetableTypes) {
			if ((unitType == targetableClass) || unitType.IsSubclassOf(targetableClass)) {
				return true;
			}
		}

		return false;
	}

	public virtual bool CanTargetObj(GameObject obj) {
		if (obj == null) {
			return false;
		}

		GameUnit aUnit = obj.GetComponent<GameUnit>();

		if (aUnit.isTargetable && CanTargetClassOfUnit(aUnit)) {
			/*
			 can't do this since we need to be able to move towards target...
			if (DistanceToObj(obj) <= range) {
				return true;
			}
			*/
			return true;
		}

		return false;
	}

	public virtual void PickTarget() {
		if (target && target.IsDestroyed()) {
			target = null;
		}

		if (!CanTargetObj(target)|| !TargetInRange()) {
			GameObject newTarget = ClosestTargetableEnemyObject();
			if (target != newTarget) {
				target = newTarget;
				UpdatedTarget();
			}
		}
	}

	public virtual void UpdatedTarget() {

	}

	public float DistanceToObj(GameObject obj) {
		// please do not change this to sqrMagnitude
		return Vector3.Distance(transform.position, obj.transform.position);
	}

	public virtual GameObject ClosestTargetableEnemyObject() {
		var ownerUnit = owner.GetComponent<GameUnit>();
		var enemyObjs = ownerUnit.EnemyObjects();
		GameObject closest = null;
		float distance = Mathf.Infinity;
		foreach (GameObject obj in enemyObjs) {
			if (CanTargetObj(obj)) {
				float curDistance = DistanceToObj(obj);
				if (curDistance < distance) {
					closest = obj;
					distance = curDistance;
				}
			}
		}
		return closest;
	}

	// --- aiming ------------------

	public virtual void ApplyAngleLimits() {
		if (turretObjX) {
			// todo
		}

		if (turretObjY) {
			// todo
		}
	}

	public static float AngleBetweenOnAxis(Vector3 v1, Vector3 v2, Vector3 n)
	{
		// Determine the signed angle between two vectors, 
		// with normal 'n' as the rotation axis.

		return Mathf.Atan2(
			Vector3.Dot(n, Vector3.Cross(v1, v2)),
			Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
	}

	public float AimDiff() {
		Transform t = transform;
		var targetPos = target.transform.position;

		Vector3 targetDir = (targetPos - t.position).normalized;
		float xAngle = AngleBetweenOnAxis (t.forward, targetDir, t.right);
		float yAngle = AngleBetweenOnAxis (t.forward, targetDir, t.forward);

		return xAngle + yAngle;
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

			if (true) {
				var r = range == -1 ? 10 : range;

				Debug.DrawLine (t.position, t.position + t.forward * r, Color.yellow); // forward 
				Debug.DrawLine (t.position, t.position + targetDir * r, Color.yellow); // targetDir 
			}


			return angle;
		}

		return 0;
	}

	public void AimOnXAxis() {
		float angle = XAngleToTarget();
		float dx = Mathf.Sign(angle) * Mathf.Sqrt(Mathf.Abs(angle)) * aimRateX; // hack for now

		Transform tt = turretObjX.transform;
		var e = tt.eulerAngles;
		tt.eulerAngles = new Vector3(e.x + dx, e.y, e.z);
	}

	public void AimOnYAxis() {
		float angle = YAngleToTarget();
		float dy = Mathf.Sign(angle) * Mathf.Sqrt(Mathf.Abs(angle)) * aimRateY; // hack for now

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


	public bool ShouldFire() {
		// easier to debug with separate ifs
		if (target) {
			if (hasAmmo()) {
				if (isLoaded()) {
					if (isAimed()) {
						if (TargetInRange()) {
							if (chooseToFire()) {
								//if ((!usesRayCastAimCheck) || RayCastHitsEnemy()) {
								if (!RayCastHitsNonEnemy()) {
									return true;
								}
							}
						}
					}
				}
			}
		}
		return false;
	}

	public void FireIfAppropriate() {
		if (ShouldFire()) {
			Fire();
		}
	}

	public float targetDistance() {
		return Vector3.Distance(owner.transform.position, target.transform.position);
	}
	
	public bool TargetInRange() {
		return (range == -1) || (targetDistance() < range);
	}

	public bool hasAmmo() {
		return (ammoCount == -1) || (ammoCount > 0);
	}

	public bool isLoaded() {
		return Time.time > isReloadedAfterTime;
	}

	public bool isAimed() {
		float diff = AimDiff();

		/*
		if (turretObjY) {
			diff += Mathf.Abs(YAngleToTarget());
		}

		if (turretObjX) {
			diff += Mathf.Abs(XAngleToTarget());
		}
*/
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
		Debug.Log(fireClip);
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

	// --- ray cast ---

	public virtual bool RayCastHitsEnemy() {
		GameObject obj = RayCastHitObject();
		return obj && owner.GetComponent<GameUnit>().isEnemyOf(obj.GetComponent<GameUnit>());
	}

	public virtual bool RayCastHitsNonEnemy() {
		GameObject obj = RayCastHitObject();
		return obj && !owner.GetComponent<GameUnit>().isEnemyOf(obj.GetComponent<GameUnit>());
	}
		

	public virtual GameObject RayCastHitObject() {
		RaycastHit hit;

		if (Physics.Raycast(transform.position, transform.forward, out hit, range)) {
			//print("Found an object - distance: " + hit.distance);
			return hit.collider.gameObject;
		}
		
		return null;
	}
}