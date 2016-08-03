using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameUnit : MonoBehaviour {
	public float thrust;

	void Start () {
		thrust = 0.0f;
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

	public virtual List<GameObject> enemyObjects() {
		var objs = activeGameObjects();
		var results = new List<GameObject>();

		foreach (GameObject obj in objs) {
			if (!obj.tag.Equals(this.tag)) {
				results.Add(obj);
			}
		}

		return results;
	}

	public virtual GameObject closestEnemyObject() {
		var objs = enemyObjects();
		GameObject closest = null;
		float distance = Mathf.Infinity;
		Vector3 position = transform.position;
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
		transform.position = new Vector3 (x, transform.position.y, transform.position.z);
	}
		
	public virtual void setY(float y) {
		transform.position = new Vector3 (transform.position.x, y, transform.position.z);
	}

	public virtual void setZ(float z) {
		transform.position = new Vector3 (transform.position.x, transform.position.y, z);
	}

	// -----------------------

	public virtual float x() {
		return transform.position.x;
	}

	float y() {
		return transform.position.y;
	}
		
	public virtual float z() {
		return transform.position.z;
	}

	// -----------------------

	public virtual float rotY() {
		return transform.eulerAngles.y;
	}

	public virtual void setRotY(float v) {
		var e = transform.eulerAngles;
		transform.eulerAngles = new Vector3(e.x, v, e.z);
	}

	// -----------------------

	public virtual void FixedUpdate () {
		rigidBody().AddForce(-transform.up * thrust);
	}

	public virtual void rotateTowardObject(GameObject obj) {
		
	}

}
