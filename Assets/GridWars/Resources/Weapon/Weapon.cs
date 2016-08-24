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

	public int clipAmmo = 4;
	public int clipMaxAmmo = 4;
	public float clipReloadTimeInSeconds = 3.0f;

	public float range = -1;
	public float aimedAngle = 5.0f;
	public float chanceOfFire = 0.02f; // as fraction of 1

	public AudioClip fireClip;
	public float fireClipVolume;

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

	public float targetLeadTime;


	// targeting

	public int priority = 0; // vehicle's uses target chosen by highest priority weapon
	public List<System.Type> targetableTypes;


	//Networking

	public void SimulateOwner() {
		if (isActive) {
			PickTarget();
			FireIfAppropriate();
			AimIfAble();
		} else {
			target = null;
		}
	}

	//MonoBehaviour

	//TODO: move code that isn't needed on client to Attached and check for isServer
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

	// --- target selection -------------------

	public virtual bool CanTargetClassOfUnit(GameUnit unit) {
		if (unit == null) {
			return false;
		}

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


	public static float AngleBetweenOnAxis(Vector3 v1, Vector3 v2, Vector3 n)
	{
		// Determine the signed angle between two vectors, 
		// with normal 'n' as the rotation axis.

		return Mathf.Atan2(
			Vector3.Dot(n, Vector3.Cross(v1, v2)),
			Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
	}

	public float AimDiff() {
		float diff = 0;
		Transform t = transform;
		var targetPos = TargetLeadPosition();

		Vector3 targetDir = (targetPos - t.position).normalized;
		//if (turretObjX) {
			float angleX = AngleBetweenOnAxis(t.forward, targetDir, t.right);
			diff += Mathf.Abs(angleX);
		//}

		//if (turretObjY) {
			float angleY = AngleBetweenOnAxis(t.forward, targetDir, t.forward);
			diff +=  Mathf.Abs(angleY);
		//}

		return diff;
	}

	public Vector3 TargetLeadPosition() {
		Rigidbody rb = target.GetComponent<Rigidbody>();
		Vector3 targetPos = target.GameUnit().ColliderCenter();

		if (rb) {
			return targetPos + rb.velocity * targetLeadTime;
		}

		return targetPos;
	}

	public float XAngleToTarget() {
		if (target) {
			Transform t = turretObjX.transform;
			var targetPos = TargetLeadPosition();

			Vector3 targetDir = (targetPos - t.position).normalized;
			float angle = AngleBetweenOnAxis (t.forward, targetDir, t.right);

			/*
			if (true) {
				var r = range == -1 ? 1000 : range;
				//Debug.DrawLine (t.position, t.position + t.forward * r, Color.red, 0, true); // forward 
				//Debug.DrawLine (t.position, t.position + targetDir * r, Color.red, 0, true); // targetDir 
			}
			*/

			return angle;
		}

		return 0;
	}

	public float YAngleToTarget() {
		if (target) {
//			/Transform t = turretObjY.transform;
			Transform t = transform;
			var targetPos = TargetLeadPosition();

			Vector3 targetDir = (targetPos - t.position).normalized;
			float angle = AngleBetweenOnAxis (t.forward, targetDir, t.up);

			if (true) {
				//var r = range == -1 ? 10 : range;

				//Debug.DrawLine (t.position, t.position + t.forward * r, Color.yellow); // forward 
				//Debug.DrawLine (t.position, t.position + targetDir * r, Color.yellow); // targetDir
			}


			return angle;
		}

		return 0;
	}

	float ClampAngle(float angle, float from, float to) {

		// adjust to -180 to 180
		if (angle > 180) {
			angle -= 360;
		}

		if (angle < from) {
			angle = from;
		}

		if (angle > to) {
			angle = to;
		}

		// adjust back to 0 to 360
		if (angle < 0) {
			angle += 360;
		}

		return angle;
	}

	public void ApplyAngleLimits() {

		/*
		if (turretObjX) {
			Vector3 e = turretObjX.transform.localEulerAngles;
			float newX = ClampAngle(e.x, turretMinX, turretMaxX);
			turretObjX.transform.localEulerAngles = new Vector3(newX, e.y, e.z);
		}

		if (turretObjY) {
			Vector3 e = turretObjY.transform.localEulerAngles;
			float newY = ClampAngle(e.y, turretMinY, turretMaxY);
			turretObjY.transform.localEulerAngles = new Vector3(e.x, newY, e.z);
		}
		*/
	}

	public void AimOnXAxis() {
		float angle = XAngleToTarget();
		float dx = Mathf.Sign(angle) * Mathf.Sqrt(Mathf.Abs(angle)) * aimRateX; // hack for now

		Transform tt = turretObjX.transform;
		var e = tt.eulerAngles;
		float newX = e.x + dx;
		tt.eulerAngles = new Vector3(newX, e.y, e.z);
	}

	public void AimOnYAxis() {
		float angle = YAngleToTarget();
		float dy = Mathf.Sign(angle) * Mathf.Sqrt(Mathf.Abs(angle)) * aimRateY; // hack for now

		Transform tt = turretObjY.transform;
		var e = tt.eulerAngles;
		float newY = e.y + dy;

		tt.eulerAngles = new Vector3(e.x, newY, e.z);
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

			ApplyAngleLimits();

			ShowDebugAimLine();
			ShowDebugTargetLine();

			return true;
		}
		return false;
	}

	// --- firing ------------------

	public bool ChooseToFire() {
		float r = Random.value;
		return r < chanceOfFire; 
	}

	public bool ShouldFire() {
		// easier to debug with separate ifs
		if (target) {
			if (hasAmmo()) {
				if (isLoaded()) {
					if (IsAimed()) {
						if (TargetInRange()) {
							if (ChooseToFire()) {
								ShowDebugAimLine();
								IsAimed();
								//if ((!usesRayCastAimCheck) || RayCastHitsEnemy()) {
								//if (!RayCastHitsNonEnemy()) {
									return true;
								//}
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
		return Vector3.Distance(owner.transform.position, TargetLeadPosition());
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

	public bool IsAimed() {
		// need to deal with both:
		// chopper missiles aiming at targets (can't ignore X or Y)
		// tanks aiming at buildings (need to ignore X)

		/*
		if (turretObjX == null && turretObjY != null) {
			// having a y turret but no x turrent usually means 
			// it's a ground vehicle aiming on ground plane
			// so this test works
			bool isHit = RayCastHitsEnemy();
			if (isHit) {
				return true;
			}
		}
		*/

		float diff = AimDiff();
		bool angleDiffOk = diff < aimedAngle;

		return angleDiffOk;
	}

	public void FillClip() {
		while ((clipAmmo < clipMaxAmmo) && (ammoCount > 0)) {
			ammoCount--;
			clipAmmo++;
		}
	}

	public void Reload() {
		if (hasAmmo()) {
			if (clipAmmo == 0) {
				FillClip();
				isReloadedAfterTime = Time.time + clipReloadTimeInSeconds;
			} else {
				clipAmmo--;
				isReloadedAfterTime = Time.time + reloadTimeInSeconds;
			}
		}
	}

	public void Fire() {
		Projectile proj =  CreateProjectile();
		if (proj != null) {
			if (fireClip != null) {
				GetComponent<AudioSource>().PlayOneShot(fireClip, fireClipVolume);
			}
			Reload();
		}
	}

	public float barrelLength() {
		// we put the weapon component at the tip of the barrel 
		// and turn off collisions between the projectile and the firing object
		// so this isn't needed anymore

		return 0.0f;
		/*
		Collider ownerCollider = owner.GetComponent<Collider>();
		float maxZ = ownerCollider.bounds.size.z;
		return maxZ * 1f; // put it outside
		*/
	}

	void CreateMuzzleFlash(){
		if (muzzleFlashPoint == null || muzzleFlash == null) {
			return;
		}
		Instantiate (muzzleFlash, muzzleFlashPoint.position, muzzleFlashPoint.rotation);
	}

	Projectile CreateProjectile() {

		if (owner.GetComponent<Rigidbody>() == null) {
			return null;
		}

		if (muzzleFlash != null) {
			CreateMuzzleFlash ();
		}
			
		var initialState = new InitialGameUnitState();
		initialState.position = transform.position + (transform.forward * barrelLength());
		initialState.rotation = transform.rotation;
		initialState.player = player;

		var projUnit = (Projectile) prefabProjectile.GetComponent<Projectile>().Instantiate(initialState);
		projUnit.copyVelocityFrom(owner);

		projUnit.IgnoreCollisionsWith(owner);

		return projUnit;
	}

	// --- ray cast ---

	public virtual bool RayCastHitsEnemy() {
		GameObject obj = RayCastHitObject();

		if (obj) {
			GameUnit ownerUnit = owner.GetComponent<GameUnit>();
			GameUnit objUnit = obj.GetComponent<GameUnit>();
			bool isEnemy = ownerUnit.isEnemyOf(objUnit);
			return isEnemy;
		}

		return false;
	}

	public virtual bool RayCastHitsNonEnemy() {
		GameObject obj = RayCastHitObject();
		return obj && !owner.GetComponent<GameUnit>().isEnemyOf(obj.GetComponent<GameUnit>());
	}

	public virtual GameObject RayCastHitObject() {
		RaycastHit hit;

		if (Physics.Raycast(transform.position, transform.forward, out hit, range)) {
			//print("Found an object - distance: " + hit.distance);
			//Debug.DrawLine(transform.position, transform.position + transform.forward * range, Color.black, 0, true); // forward 
			return hit.collider.gameObject;
		} else {
			//Debug.DrawLine (transform.position, transform.position + transform.forward * range, Color.red, 0, true); // forward 
		}
		
		return null;
	}

	float Convert360to180(float angle) {
		if (angle > 180) {
			angle -= 360;
		}
		return angle;
	}

	float Convert180to360(float angle) {
		if (angle < 0) {
			angle += 360;
		}
		return angle;
	}

	#if UNITY_EDITOR
	void OnDrawGizmos() {
		UnityEditor.Handles.color = Color.white;
		if (turretObjY) {
			//UnityEditor.Handles.Label(transform.position, "y" + Mathf.Floor(Convert360to180(turretObjY.transform.localEulerAngles.y)));
			UnityEditor.Handles.Label(transform.position, "x" + Mathf.Floor(Convert360to180(turretObjY.transform.localEulerAngles.x)));
		}
		//UnityEditor.Handles.Label (transform.position, "ey " + Mathf.Floor(transform.eulerAngles.y));
	}
	#endif

	public void ShowDebugAimLine() {
		RaycastHit hit;
		if (Physics.Raycast(transform.position, transform.forward, out hit, range) && RayCastHitsEnemy()) {
			
			Debug.DrawLine(transform.position, hit.point, Color.yellow, 0, true); // hit point
		} else {
			Debug.DrawLine(transform.position, transform.position + transform.forward * 1000f, Color.red, 0, true);
		}

//		Debug.DrawLine(transform.position, transform.position + transform.forward * 1000f, Color.yellow, 0, true);
	}
		
	public void ShowDebugTargetLine() {
		if (target) {
			Debug.DrawLine(transform.position, target.gameObject.transform.position, Color.black, 0, true); 
		} 
	}
}