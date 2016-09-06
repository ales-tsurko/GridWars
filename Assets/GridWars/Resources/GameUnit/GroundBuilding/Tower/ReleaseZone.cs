using UnityEngine;
using System.Collections.Generic;

public class ReleaseZone : MonoBehaviour {
	public bool isObstructed {
		get {
			foreach (var hitInfo in Physics.BoxCastAll(transform.position, size/2, Vector3.up, Quaternion.identity, size.y)) {
				if (hitInfo.collider.GetComponent<Vehicle>() != null) {
					return true;
				}
			}
			return false;
		}
	}

	protected Vector3 size {
		get {
			return GetComponent<BoxCollider>().size;
		}
	}
}
