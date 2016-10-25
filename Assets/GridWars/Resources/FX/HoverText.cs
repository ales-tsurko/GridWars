using UnityEngine;
using System.Collections;

// attach this to a GUIText

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
		/*
		Transform pt = transform;
		pt.position = cam.WorldToViewportPoint(target.position + offset);

		Vector3 v = cam.transform.position - pt.position;
		v.x = v.z = 0.0f;
		pt.LookAt(cam.transform.position - v); 
		pt.Rotate(0, 180, 0);
		*/

		Vector3 v = cam.transform.position - transform.position; 
		transform.rotation = Quaternion.LookRotation( v );
	


		//transform.rotation = Quaternion.LookRotation(n) * Quaternion.Euler(0, 90, 0);

	}
}
