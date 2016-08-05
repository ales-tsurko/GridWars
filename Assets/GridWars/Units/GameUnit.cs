using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameUnit : MonoBehaviour {
	public float thrust;
	public float rotationThrust;
	public  Player player;
	[HideInInspector]
	public Transform _t;
	void Awake () {
		_t = transform;
	}

	public virtual void Start () {
		thrust = 0.0f;
		rotationThrust = 0.1f;
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
		//GameUnit otherUnit = otherGameObject.GetComponent<GameUnit> ();
		//return otherUnit.tag.Contains("Player") && !otherUnit.tag.Equals(this.tag
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

	// -----------------------

	public virtual void setX(float x) {
		_t.position = new Vector3 (x, _t.position.y, _t.position.z);
	}
		
	public virtual void setY(float y) {
		_t.position = new Vector3 (_t.position.x, y, _t.position.z);
	}

	public virtual void setZ(float z) {
		_t.position = new Vector3 (_t.position.x, _t.position.y, z);
	}

	// -----------------------

	public virtual float x() {
		return _t.position.x;
	}

	public virtual float y() {
		return _t.position.y;
	}
		
	public virtual float z() {
		return _t.position.z;
	}

	// -----------------------

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
		
	public virtual void FixedUpdate () {
		rigidBody().AddForce(forwardVector() * thrust);

		if (isOutOfBounds() ) {
			Destroy (gameObject);
		}
	}

	// -------------------

	/// Determine the signed angle between two vectors, with normal 'n'
	/// as the rotation axis.
	/// 
	public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
	{
		return Mathf.Atan2(
			Vector3.Dot(n, Vector3.Cross(v1, v2)),
			Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
	}

	public virtual Vector3 forwardVector() {
		return _t.forward;
	}

	public virtual Vector3 upVector() {
		return _t.up;
	}

	public virtual Vector3 rightVector() {
		return _t.right;
	}

	public virtual void aimTowardsNearestEnemy() {
		var obj = closestEnemyObject ();
		if (obj != null) {
			rotateTowardObject (obj);
		}
	}

	public virtual void rotateTowardObject(GameObject obj) {
		//print (tag + " target " + obj.tag);
		rotateTowardsPos (obj.transform.position);
	}

	public virtual void rotateTowardsPos(Vector3 targetPos)
	{
		Vector3 targetDir = (targetPos - _t.position).normalized;
		float angle = AngleSigned(_t.forward, targetDir, _t.up);

		//print ("angle " + Mathf.Floor(angle));

		//float v = angle > 0 ? Mathf.Sqrt(Mathf.Abs(angle)) : - Mathf.Sqrt(Mathf.Abs(angle));
		//rigidBody().AddTorque(- f * v * rotationThrust, ForceMode.Force);
		rigidBody().AddTorque( _t.up * angle * rotationThrust, ForceMode.Force);
		print ("aiming");
	}

	void OnCollisionEnter(Collision collision) {
		GameUnit otherUnit = collision.gameObject.GetComponent<GameUnit> ();

		if (isEnemyOf (otherUnit)) {
			print(this.player.playerNumber + " collision " + otherUnit.player.playerNumber);
			Destroy (gameObject);
		}

		foreach (ContactPoint contact in collision.contacts) {
			Debug.DrawRay (contact.point, contact.normal, Color.white);
		}
		if (collision.relativeVelocity.magnitude > 2) {
			//audio.Play ();
			//print("collision");
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
//		Gizmos.DrawSphere(_t.position, 1);

		var obj = closestEnemyObject ();
		if (obj != null) {
			Gizmos.DrawLine(_t.position, obj.transform.position);
		}
	}
}
