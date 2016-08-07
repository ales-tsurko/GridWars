using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameUnit : MonoBehaviour {
	public float thrust;
	public float rotationThrust;
	public  Player player;
	public float powerCost = 4f;
	public bool canAim = true;
	[HideInInspector]
	public Transform _t;
	public bool isRunning = true;

	public GameObject target = null;
	public float angleToTarget = 0;

	public Vector3 lastUpTorque;
	public bool isStaticUnit = false;

	void Awake () {
		_t = transform;
	}

	public virtual void Start () {
		thrust = 0.0f;

		if (!isStaticUnit) {
			this.EachRenderer(r => {
				r.material = new Material(r.material);
			});
			GetComponent<Collider>().enabled = true;
			GetComponent<Rigidbody>().useGravity = true;
			this.EachMaterial(m => {
				m.SetColor("_V_WIRE_Color", new Color(0, 0, 0, 0));
				if (player == null) {
					m.SetColor("_Color", Color.white);
				}
				else {
					m.SetColor("_Color", player.color);
				}

			});
		}

		rotationThrust = 1.0f;
	}

	public virtual Rigidbody rigidBody() {
		return GetComponent<Rigidbody> ();
	}

	// -----------------------

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
		if (player == null) {
			print ("null player " + this);
		}
		return (player.playerNumber != otherUnit.player.playerNumber);
		//return (player != null) && (otherUnit.player != null) && (player != otherUnit.player);
	}

	public virtual List<GameObject> enemyObjects() {
		var objs = activeGameObjects();
		var results = new List<GameObject>();

		foreach (GameObject obj in objs) {
			GameUnit unit = obj.GetComponent<GameUnit> ();
			//if (obj.tag.Contains("Player") && !obj.tag.Equals(this.tag)) {
			if ((obj.tag != null) && (unit != null && isEnemyOf(unit))) {
				results.Add(obj);
			}
		}

		return results;
	}

	public virtual void pickTarget() {
		target = closestEnemyObject ();
	}

	public virtual GameObject closestEnemyObject() {
		var objs = enemyObjects();
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

	public virtual void steerTowardsNearestEnemy() {
		var obj = closestEnemyObject ();
		if (obj != null) {
			rotateTowardObject (obj);
		}
	}

	public virtual void rotateTowardObject(GameObject obj) {
		var targetPos = obj.transform.position;

		Vector3 targetDir = (targetPos - _t.position).normalized;
		float angle = AngleBetweenOnAxis(_t.forward, targetDir, _t.up);
		angleToTarget = angle;

		/*
		if (false) {
			Debug.DrawLine(_t.position, _t.position + _t.forward*10.0f, Color.blue); // forward blue
			Debug.DrawLine(_t.position, _t.position + targetDir*10.0f, Color.yellow); // targetDir yellow
			Debug.DrawLine(_t.position, _t.position + targetDir*rotationThrust, Color.red); // targetDir red
		}
		*/

		rigidBody().AddTorque( _t.up * angle *rotationThrust, ForceMode.Force);
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.collider.name == "BattlefieldPlane") {
			return;
		}

		GameUnit otherUnit = collision.gameObject.GetComponent<GameUnit> ();

		print(this.player.playerNumber + " collision " + otherUnit.player.playerNumber);

		if (isEnemyOf (otherUnit)) {
			print(this.player.playerNumber + " collision " + otherUnit.player.playerNumber);
			Destroy (gameObject);
		}

		foreach (ContactPoint contact in collision.contacts) {
			Debug.DrawRay (contact.point, contact.normal, Color.white);
		}

		/*
		if (collision.relativeVelocity.magnitude > 2) {
			//audio.Play ();
			//print("collision");
			Destroy (gameObject);
		}
		*/
	}


	void OnDrawGizmos() {
		/*
		if (Application.isPlaying && canAim) {
			//	Gizmos.DrawSphere(_t.position, 1);

			var obj = closestEnemyObject ();
			if (obj != null) {
				Gizmos.color = Color.red;
				Gizmos.DrawLine (_t.position, obj.transform.position);
				Gizmos.color = Color.yellow;
				Gizmos.DrawRay(_t.position,  lastUpTorque * 20.0f);
			}
		}
		*/
	}
}
