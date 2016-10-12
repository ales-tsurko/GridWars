using UnityEngine;
using System.Collections.Generic;

public class ReleaseZone : MonoBehaviour {
	public Vector3 size;
	public GameUnit hiddenUnit;

	public bool isObstructed {
		get {
			if (hiddenUnit != null) {
				return true;
			}

			foreach (var hitInfo in Physics.BoxCastAll(transform.position, size/2, Vector3.up, Quaternion.identity, size.y)) {
				if (hitInfo.collider.GetComponent<Vehicle>() != null) {
					return true;
				}
			}
			return false;
		}
	}
		
	void OnDrawGizmos() {
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube(transform.position + new Vector3(0f, size.y/2, 0f), size);
	}
}
