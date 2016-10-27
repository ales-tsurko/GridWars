using UnityEngine;
using System.Collections;

// attach this to a GUIText container

public class HoverText : MonoBehaviour {

	public Transform target;			// Object that this label should follow
	//Vector3 offset = Vector3.up * 5f;	// Units in world space to offset; 1 unit above object by default
	public Camera cam = null;

	public void Start () {
		if (cam == null) {
			cam = Camera.main;
		}
	}

	public void Update () {
		Vector3 directionToCam = Camera.main.transform.position - transform.position; 
		transform.rotation = Quaternion.LookRotation(directionToCam);

		//transform.rotation = Quaternion.LookRotation(n) * Quaternion.Euler(0, 90, 0);
	}
}
