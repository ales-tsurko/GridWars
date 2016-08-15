﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameUnit : MonoBehaviour {
	public float thrust;
	public float rotationThrust;
	public  Player player;
	public bool canAim = true;

	// Damagable
	public float hitPoints;
	public float maxHitPoints;

	public bool isTargetable = true;

	[HideInInspector]
	public Transform _t;
	public bool isRunning = true;

	public GameObject target = null;
	//public float angleToTarget = 0;

	public bool isStaticUnit = false;

	//tower
	public float powerCost = 4f;
	public float cooldownSeconds = 1f;
	public float standOffDistance = 20f;
	public KeyCode[] buildKeyCodeForPlayers = new KeyCode[2];



	public float hpRatio {
		get {
			return hitPoints/maxHitPoints;
		}
	}

	AudioSource _audioSource;
	protected AudioSource audioSource {
		get {
			if (_audioSource == null) {
				_audioSource = gameObject.AddComponent<AudioSource>();
			}
			return _audioSource;
		}
	}

	public AudioClip birthSound {
		get {
			string path = ResourcePathForUnitType(GetType());
			string soundPath = path + "/Sounds/birth";

			return Resources.Load<AudioClip>(soundPath);
		}
	}

	public GameObject deathExplosionPrefab;

	void Awake () {
		_t = transform;
	}

	public static GameObject Load<T>() where T: GameUnit {
		return Load(typeof(T));
	}

	public static string ResourcePathForUnitType(System.Type type) {
		List <string> pathComponents = new List<string>();

		while (type != typeof(GameUnit)) {
			pathComponents.Add(type.Name);
			type = type.BaseType;
		}

		pathComponents.Add(type.Name); // add GameUnit

		pathComponents.Reverse();

		return string.Join("/", pathComponents.ToArray());
	}

	public static GameObject Load(System.Type type) {

		string path = ResourcePathForUnitType(type);
		string prefabPath = path + "/Prefabs/" + type.Name;

		GameObject obj = (GameObject) Resources.Load(prefabPath);

		if (obj == null) {
			print("missing prefabPath " + prefabPath);
			throw new System.Exception("missing prefabPath " + prefabPath);
		} else {
			//print("found prefabPath " + prefabPath);
		}

		return obj;
	}

	public static T Instantiate<T>() where T: GameUnit {
		return (T) GameUnit.Instantiate(typeof(T));
	}

	public static GameUnit Instantiate(System.Type type) {
		return (GameUnit) Instantiate<GameObject>(Load(type)).GetComponent(type);
	}

	public virtual void Start () {
		hitPoints = maxHitPoints;

		SetupWeapons();
		SetupSmokeDamage ();
		gameObject.CloneMaterials();

		if (player != null) {
			player.Paint(gameObject);
			player.tag = "Player" + player.playerNumber;
		}

		PlayBirthSound();
	}

	protected void PlayBirthSound() {
		if (birthSound != null) {
			audioSource.PlayOneShot(birthSound);
		}
	}

	public virtual Rigidbody rigidBody() {
		if (body == null) {
			body = GetComponent<Rigidbody> ();
		}
		return body;
	}
	Rigidbody body;

	// -----------------------

	public bool IsInStandoffRange() {
		if (standOffDistance == -1) {
			return true;
		}

		return target && (targetDistance() < standOffDistance);
	}
		
	public virtual List<GameObject> activeGameObjects() {
		GameObject[] objs = (GameObject[])UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
		var results = new List<GameObject>();
		foreach (GameObject obj in objs) {
			if (obj.activeInHierarchy) {
				results.Add(obj);
			}
		}
		return results;
	}

	public virtual bool isEnemyOf(GameUnit otherUnit) {
		if (otherUnit == null) {
			return false;
		}

		if (player == null) {
			//print ("null player " + this);
			return false;
		}

		if (otherUnit == null) {
			//print ("null otherUnit " + otherUnit);
			return false;
		}

		if (otherUnit.player == null) {
			//print ("null otherUnit.player " + otherUnit.player);
			return false;
		}

		return (player.playerNumber != otherUnit.player.playerNumber);
		//return (player != null) && (otherUnit.player != null) && (player != otherUnit.player);
	}

	public float DistanceToObj(GameObject obj) {
		// please do not change this to sqrMagnitude
		return Vector3.Distance(obj.transform.position, transform.position);
	}

	public virtual List<GameObject> EnemyObjects() {

		GameUnit[] gameUnits = FindObjectsOfType<GameUnit>();
		var results = new List<GameObject>();
		foreach (GameUnit gameUnit in gameUnits) {
			if (gameUnit.player && (gameUnit.player != player) && CanTargetUnit(gameUnit)) {
				results.Add (gameUnit.gameObject);
			}
		}
		return results;

		/*var objs = activeGameObjects();
		foreach (GameObject obj in objs) {
			GameUnit unit = obj.GetComponent<GameUnit> ();
			//if (obj.tag.Contains("Player") && !obj.tag.Equals(this.tag)) {
			if ((obj.tag != null) && (unit != null && isEnemyOf(unit))) {
				results.Add(obj);
			}
		}

		return results;*/
	}

	// --- targeting -------------------

	public virtual bool CanTargetUnit(GameUnit unit) {

		if (unit.isTargetable) {
			return true;
		}

		return false;
	}

	public virtual Weapon HighestPriorityWeaponWithTarget() {
		Weapon[] weapons = Weapons();

		Weapon result = null;
		int highestPriority = -1;
		foreach (Weapon weapon in weapons) {
			if (weapon.target && highestPriority < weapon.priority) {
				result = weapon;
				highestPriority = weapon.priority;
			}
		}

		return result;
	}

	public virtual void PickTarget() {
		Weapon targetingWeapon = HighestPriorityWeaponWithTarget ();

		if (target && target.IsDestroyed()) {
			target = null;
		}

		if (targetingWeapon) {
			GameObject newTarget = targetingWeapon.target;
			//GameObject newTarget = ClosestEnemyObject ();

			if (target != newTarget) {
				target = newTarget;
				//UpdatedTarget();
			}
		}
	}

	public virtual GameObject ClosestEnemyObject() {
		var objs = EnemyObjects();
		GameObject closest = null;
		float distance = Mathf.Infinity;
		Vector3 position = _t.position;
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

	// -- set x, y, z -----------------------

	public virtual void setX(float x) {
		_t.position = new Vector3 (x, _t.position.y, _t.position.z);
	}
		
	public virtual void setY(float y) {
		_t.position = new Vector3 (_t.position.x, y, _t.position.z);
	}

	public virtual void setZ(float z) {
		_t.position = new Vector3 (_t.position.x, _t.position.y, z);
	}

	// --- get x, y, z -----------------------

	public virtual float x() {
		return _t.position.x;
	}

	public virtual float y() {
		return _t.position.y;
	}
		
	public virtual float z() {
		return _t.position.z;
	}

	// --- get/set rotations -----------------------

	public virtual float rotX() {
		return _t.eulerAngles.x;
	}

	public virtual void setRotX(float v) {
		var e = _t.eulerAngles;
		_t.eulerAngles = new Vector3(v, e.y, e.z);
	}


	public virtual void Object_rotDY(GameObject obj, float dy) {
		var e = obj.transform.eulerAngles;
		obj.transform.eulerAngles = new Vector3(e.x, e.y + dy, e.z);
	}


	public virtual float rotY() {
		return _t.eulerAngles.y;
	}

	public virtual void setRotY(float v) {
		var e = _t.eulerAngles;
		_t.eulerAngles = new Vector3(e.x, v, e.z);
	}

	public virtual float rotZ() {
		return _t.eulerAngles.z;
	}

	public virtual void setRotZ(float v) {
		var e = _t.eulerAngles;
		_t.eulerAngles = new Vector3(e.x, e.y, v);
	}

	// -----------------------

	public virtual bool isOutOfBounds () {
		return (
			(y() < -3) || (y() > 50) ||
			(x() > 50) || (x() > 50) ||
			(z() > 50) || (z() > 50) 
		);
	}
		
	public virtual void RemoveIfOutOfBounds () {
		if (isOutOfBounds() ) {
			Destroy (gameObject);
		}
	}

	public virtual void FixedUpdate () {

		if (player == null) {
			print ("FixedUpdate null player on " + this);
		}

		RemoveIfOutOfBounds ();
	}

	// -------------------


	public static float AngleBetweenOnAxis(Vector3 v1, Vector3 v2, Vector3 n)
	{
		// Determine the signed angle between two vectors, 
		// with normal 'n' as the rotation axis.

		return Mathf.Atan2(
			Vector3.Dot(n, Vector3.Cross(v1, v2)),
			Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
	}

	// --- aiming --------------------

	public virtual void SteerTowardsTarget() {
		if (target != null) {
			RotateTowardObject (target);
		}
	}

	/*
	public virtual void steerTowardsNearestEnemy() {
		var obj = ClosestEnemyObject ();
		if (obj != null) {
			RotateTowardObject(obj);
		}
	}
	*/

	public virtual void RotateTowardObject(GameObject obj) {
		var targetPos = obj.transform.position;

		Vector3 targetDir = (targetPos - _t.position).normalized;
		float angle = AngleBetweenOnAxis(_t.forward, targetDir, _t.up);
		//angleToTarget = angle;

		//Debug.DrawLine(_t.position, _t.position + _t.forward*10.0f, Color.blue); // forward blue
		//Debug.DrawLine(_t.position, _t.position + targetDir*10.0f, Color.yellow); // targetDir yellow
		//Debug.DrawLine(_t.position, _t.position + targetDir*rotationThrust, Color.red); // targetDir red

		rigidBody().AddTorque( _t.up * angle * rotationThrust, ForceMode.Force);
	}

	public float targetDistance() {
		return Vector3.Distance(transform.position, target.transform.position);
	}

	void OnCollisionEnter(Collision collision) {
		/*
		if (collision.collider.name == "BattlefieldPlane") {
			return;
		}

		GameUnit otherUnit = collision.gameObject.GetComponent<GameUnit> ();

		//print(this.player.playerNumber + " collision " + otherUnit.player.playerNumber);

		if (isEnemyOf (otherUnit)) {
			//print(this.player.playerNumber + " collision " + otherUnit.player.playerNumber);
			//Destroy (gameObject);
		}

		foreach (ContactPoint contact in collision.contacts) {
			Debug.DrawRay (contact.point, contact.normal, Color.white);
		}
		*/

		/*
		if (collision.relativeVelocity.magnitude > 2) {
			//audio.Play ();
			//print("collision");
			Destroy (gameObject);
		}
		*/
	}

	// --- icons --------------------


	void OnDrawGizmos() {
		if (target != null) {
			Gizmos.color = Color.green;
			Gizmos.DrawLine (_t.position, target.transform.position);
		}
	}

	// Damage

	public void ApplyDamage(float damage) {
		hitPoints -= damage;
		if (smokeDamage !=null) {
			smokeDamage.maxParticles = Mathf.Clamp (1000 - (int)((hitPoints / maxHitPoints) * 1000), 250, 1000);
		}
		if (hitPoints <= 0) {
			OnDead();
		}
	}

	public void OnDead() {
		ShowExplosion();
		Destroy(gameObject);
	}

	public void ShowExplosion() {
		if (deathExplosionPrefab != null) {
			var obj = Instantiate(deathExplosionPrefab);
			obj.transform.position = transform.position;
			obj.transform.rotation = UnityEngine.Random.rotation;
			obj.transform.localScale *= 15;
		}

		Destroy(gameObject);
	}

	//Particles for displaying damage amount to units
	ParticleSystem smokeDamage;
	void SetupSmokeDamage () {
		Transform smokeDamageT = _t.FindChild ("SmokeDamage");
		if (smokeDamageT != null) {
			smokeDamage = smokeDamageT.GetComponentInChildren<ParticleSystem> ();
			smokeDamage.maxParticles = 0;
		}
	}

	Weapon[] Weapons() {
		Weapon[] weapons = GetComponentsInChildren<Weapon>();
		return weapons;
	}

	void SetupWeapons() {
		Weapon[] weapons = Weapons();

		foreach (Weapon weapon in weapons) {
			weapon.owner = gameObject;
			weapon.enabled = true;
		}
	}

	/*
	public virtual void UpdatedTarget() {
		// by default we set all weapons to have this target, 
		// override this method if you want them to choose different targets

		Weapon[] weapons = GetComponentsInChildren<Weapon>();

		foreach (Weapon weapon in weapons) {
			weapon.target = target;
		}
	}
	*/

}
