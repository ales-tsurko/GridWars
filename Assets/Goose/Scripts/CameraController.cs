using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CameraController : MonoBehaviour {
	public List<Transform> positions = new List<Transform>();
	int pos;
	bool moving;
	public float moveSpeed;
	Vector3 startPos;
	Vector3 targetPos;
	Quaternion startRot;
	Quaternion targetRot;
	float startTime;
	public Transform cam;
	MouseLook mouseLook;
	bool actionMode;
	// Use this for initialization
	void Start () {
		pos = -1;
		mouseLook = cam.GetComponent<MouseLook> ();
		NextPosition ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.C)) {
			NextPosition ();
		}
		if (Input.GetMouseButtonDown (0) && Input.GetKey(KeyCode.LeftShift)) {
			RaycastHit hit;
			Ray vRay = cam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast (vRay, out hit, 3000)) {
				if (hit.transform.GetComponent<GameUnit> ()) {
					MoveToActionPosition (hit.transform);	
				}
			}
		}
		if (!moving) {
			return;
		}
		if (Vector3.Distance (cam.localPosition, targetPos) < .05f && Quaternion.Angle(cam.localRotation, targetRot) < .1f) {
			if (actionMode) {
				mouseLook.enabled = true;
			}
			moving = false;
		}
		//print ("Moving " + Time.time);
		float timeSinceStarted = Time.time - startTime;
		float percentageComplete = timeSinceStarted / moveSpeed;
		cam.localPosition = Vector3.Lerp (startPos, targetPos, percentageComplete);
		cam.localRotation = Quaternion.Lerp (startRot, targetRot, percentageComplete);

	}

	void MoveToActionPosition (Transform _target) {
		if (!actionMode) {
			pos--;
		}
		actionMode = true;
		cam.parent = _target.transform;
		targetPos = Vector3.zero + new Vector3 (0, 2, 0);
		targetRot = Quaternion.Euler (Vector3.zero);
		startPos = cam.localPosition;
		startRot = cam.localRotation;
		startTime = Time.time;
		moving = true;
	}

	void NextPosition () {
		//print ("Next Called");
		actionMode = mouseLook.enabled = false;
		cam.parent = null;
		pos++;
		Transform newTarget = positions [pos % positions.Count];
		targetPos = newTarget.position;
		targetRot = newTarget.rotation;
		startPos = cam.position;
		startRot = cam.rotation;
		startTime = Time.time;
		moving = true;

	}

	public void ResetCamera () {
		NextPosition ();
	}

}
