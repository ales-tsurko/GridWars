using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

	// Thinking

	int thinkFrequency = 20;

	int thinkBucket {
		get {
			return (int)((uint)GetHashCode() % (uint)thinkFrequency);
		}
	}

	public bool IsThinkStep() {
		return (App.shared.timeCounter % thinkFrequency == 0);
	}

	public void Think() {
		PickTarget();
	}

	// Networking

	public void SimulateOwner() {
		if (isActive) {
			if (IsThinkStep()) {
				Think();
			}
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

	public virtual void ConsiderTarget(GameObject obj) {
		if (CanTargetClassOfUnit(obj.GameUnit())) {
			target = obj;
		}
	}

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
		if (target != null && target.IsDestroyed()) {
			target = null;
		}

		// unless we can shoot now at our current target,
		// see if there's one that's closer to target

		// prioritize targets with weapons
		// prioritize targets with weapons - especially those that can target us

		if (!CanTargetObj(target)|| !TargetInRange()) {
			//bool isTargetable = owner.GameUnit().isTargetable;
			//var oldTarget = target;

			GameObject newTarget = ClosestTargetableEnemyObjectWithWeapon();

			if (newTarget == null) {
				newTarget = ClosestTargetableEnemyObject();
			}

			if (target != newTarget) {
				target = newTarget;
				UpdatedTarget();
			}
		}
	}

	public virtual void UpdatedTarget() {
	}

	public float DistanceToObj(GameObject obj) {
		// Please do not change this to sqrMagnitude
		return Vector3.Distance(transform.position, obj.transform.position);
	}

	/*
	public virtual float EvalTarget(GameObject obj) {
		float distance = DistanceToObj(obj);


	}

	public virtual GameObject EvaledTargets() {
		Tuple <GameObject, float> tuple =
			new Tuple<GameObject, float>(1, "cat", true);

		var ownerUnit = owner.GameUnit();
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
	*/


	public virtual IEnumerable<GameObject> TargetableEnemyObjects() { 
		var ownerUnit = owner.GameUnit();
		var enemyObjs = ownerUnit.EnemyObjects();
		return enemyObjs.Where(obj => this.CanTargetObj(obj));
	}
		
	public virtual GameObject ClosestTargetableEnemyObject() {
		return ClosestOfObjects(TargetableEnemyObjects());
	}

	public virtual IEnumerable <GameObject> TargetableEnemyObjectWithWeapons() {
		return TargetableEnemyObjects().Where(obj => obj.GameUnit() != null && obj.GameUnit().HasWeapons());
	}


	public virtual GameObject ClosestTargetableEnemyObjectWithWeapon() {
		return ClosestOfObjects(TargetableEnemyObjectWithWeapons());
	}

	public virtual GameObject ClosestTargetableEnemyVehicles() {
		return ClosestOfObjects(App.shared.stepCache.AllVehicleObjects());
	}

	public virtual GameObject ClosestOfObjects(IEnumerable<GameObject> objs) {
		GameObject closest = null;
		float distance = Mathf.Infinity;
		var ownerUnit = owner.GameUnit();
		Vector3 position = ownerUnit.transform.position;
		foreach (GameObject obj in objs) {
			Vector3 diff = obj.transform.position - position;
			float curDistance = diff.sqrMagnitude;
			if (curDistance < distance) {
				closest = obj;
				distance = curDistance;
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
			//Transform t = turretObjX.transform;
			Transform t = transform;
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
			//Transform t = turretObjY.transform;
			Transform t = transform;
			var targetPos = TargetLeadPosition();

			Vector3 targetDir = (targetPos - t.position).normalized;
			float angle = AngleBetweenOnAxis (t.forward, targetDir, t.up);

			if (true) {
				var r = range == -1 ? 10 : range;
				Debug.DrawLine (t.position, t.position + t.forward * r, Color.yellow); // forward 
				//Debug.DrawLine (t.position, t.position + targetDir * r, Color.yellow); // targetDir
			}


			return angle;
		}

		return 0;
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
	}

	// --- Aiming and deciding to fire ------------------

	public bool AimIfAble() { 
		
		if (target) {
			
			if (canRotateX()) {
				AimOnXAxis();
			}

			if (canRotateY()) {
				AimOnYAxis();
			}

			ApplyAngleLimits();

			ShowDebugAimLine();
			/*
			ShowDebugTargetLine();
			*/

			return true;
		}

		return false;
	}

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
								//ShowDebugAimLine();
								IsAimed();
								//if ((!usesRayCastAimCheck) || RayCastHitsEnemy()) {
								if (!RayCastHitsFriend()) {
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
		return Vector3.Distance(owner.transform.position, TargetLeadPosition());
	}
	
	public bool TargetInRange() {
		return (range == -1) || (targetDistance() < range);
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

	// --- Ammo and Loading -----------------------

	public bool hasAmmo() {
		return (ammoCount == -1) || (ammoCount > 0);
	}

	public void DecrementAmmo() {
		if (ammoCount > 0) {
			ammoCount--;
		}
	}

	public bool isLoaded() {
		return Time.time > isReloadedAfterTime;
	}



	public void FillClip() {
		while ((clipAmmo < clipMaxAmmo+1) && hasAmmo()) {
			DecrementAmmo();
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

	// --- Firing -----------------------------------------

	public void TellTargetItsBeingFiredOn() {
		if(target && !target.IsDestroyed()) {
			var unit = target.GameUnit();
			if (unit) {
				unit.WasFiredOnByWeapon(this);
			}
		}
	}

	public void Fire() {
		TellTargetItsBeingFiredOn();
		Projectile proj =  CreateProjectile();
		if (proj != null) {
			if (fireClip != null) {
				GetComponent<AudioSource>().PlayOneShot(fireClip, fireClipVolume);
			}
			Reload();
		}
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
			
		var projectile = prefabProjectile.GetComponent<Projectile>().Instantiate() as Projectile;
		projectile.player = player;
		projectile.transform.position = transform.position;
		projectile.transform.rotation = transform.rotation;
		projectile.copyVelocityFrom(owner);
		projectile.IgnoreCollisionsWith(owner);

		return projectile;
	}

	// --- Ray Casting ----------------------------------------

	public virtual bool RayCastHitsEnemy() {
		GameObject obj = RayCastHitObject();

		if (obj) {
			GameUnit ownerUnit = owner.GetComponent<GameUnit>();
			GameUnit objUnit = obj.GetComponent<GameUnit>();
			bool isEnemy = ownerUnit.IsEnemyOf(objUnit);
			return isEnemy;
		}

		return false;
	}

	public virtual bool RayCastHitsNonEnemy() {
		GameObject obj = RayCastHitObject();
		return obj && !owner.GetComponent<GameUnit>().IsEnemyOf(obj.GetComponent<GameUnit>());
	}

	public virtual bool RayCastHitsFriend() {
		GameObject obj = RayCastHitObject();
		return obj && owner.GetComponent<GameUnit>().IsFriendOf(obj.GetComponent<GameUnit>());
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

	// --- Utilities -----------------------------------

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

	// --- Debugging --------------------------------------

	#if UNITY_EDITOR
	void OnDrawGizmos() {
		/*

		string msg = ""

		if (turretObjX) {
			msg += "x" + Mathf.Floor(Convert360to180(turretObjX.transform.localEulerAngles.x)) +  " ";
		}
		
		if (turretObjY) {
			msg += "y" + Mathf.Floor(Convert360to180(turretObjY.transform.localEulerAngles.y));
		}

		UnityEditor.Handles.color = Color.white;
		UnityEditor.Handles.Label(msg);
		*/
	}
	#endif

	public void ShowDebugAimLine() {
		//Debug.DrawLine(transform.position, transform.position + transform.forward * 1000f, Color.red, 0, true);
		/*
		RaycastHit hit;
		if (Physics.Raycast(transform.position, transform.forward, out hit, range) && RayCastHitsEnemy()) {
			
			//Debug.DrawLine(transform.position, hit.point, Color.yellow, 0, true); // hit point
		} else {
			//Debug.DrawLine(transform.position, transform.position + transform.forward * 1000f, Color.red, 0, true);
		}

		//Debug.DrawLine(transform.position, transform.position + transform.forward * 1000f, Color.yellow, 0, true);
	    */
	}
		
	public void ShowDebugTargetLine() {
		if (target) {
			//Debug.DrawLine(transform.position, target.gameObject.transform.position, Color.black, 0, true); 
		} 
	}
}