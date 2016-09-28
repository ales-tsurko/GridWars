using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BetterMonoBehaviour : MonoBehaviour {

	[HideInInspector]
	public Transform _t;

	public virtual void Awake() {
		_t = transform;
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


	public virtual float Object_rotX(GameObject obj) {
		return obj.transform.eulerAngles.x;
	}

	public virtual float Object_rotY(GameObject obj) {
		return obj.transform.eulerAngles.y;
	}

	public virtual void Object_setRotX(GameObject obj, float a) {
		var e = obj.transform.eulerAngles;
		obj.transform.eulerAngles = new Vector3(a, e.y, e.z);
	}

	public virtual void Object_setRotY(GameObject obj, float a) {
		var e = obj.transform.eulerAngles;
		obj.transform.eulerAngles = new Vector3(e.x, a, e.z);
	}


	public virtual void Object_rotDX(GameObject obj, float dx) {
		var e = obj.transform.eulerAngles;
		obj.transform.eulerAngles = new Vector3(e.x + dx, e.y, e.z);
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

	// ------------------------------------

	public static float AngleBetweenOnAxis(Vector3 v1, Vector3 v2, Vector3 n)
	{
		// Determine the signed angle between two vectors, 
		// with normal 'n' as the rotation axis.

		return Mathf.Atan2(
			Vector3.Dot(n, Vector3.Cross(v1, v2)),
			Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
	}

	// --- Helpers -------------------------------------------

	public Vector3 ColliderCenter() {
		Vector3 c = GetComponent<BoxCollider>().center;
		return transform.TransformPoint(c);
	}

	public BoxCollider BoxCollider() {
		return gameObject.GetComponent<BoxCollider>();
	}

	public bool IsOfType(Type aType) {

		Type myType = GetType();

		if (myType == aType) { 
			return true; 
		}

		if (myType.IsSubclassOf(aType)) { 
			return true; 
		}

		return false;
	}

	// --- Lists ------------------------------------

}
