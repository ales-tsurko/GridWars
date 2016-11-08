using UnityEngine;
using System.Collections;

// attach this to a GUIText container

public class HoverText : MonoBehaviour {

	/*
	public Transform target; // Object that this label should follow
	public Camera cam = null;

	public void Start () {
		if (cam == null) {
			cam = Camera.main;
		}
	}
	*/

	public void Update () {
		Camera cam = Camera.main;
		Vector3 directionToCam = Camera.main.transform.position - transform.position; 
		transform.rotation = Quaternion.LookRotation(directionToCam);
		//transform.rotation = Quaternion.LookRotation(n) * Quaternion.Euler(0, 90, 0);
	}
}
