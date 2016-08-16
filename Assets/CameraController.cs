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
	// Use this for initialization
	void Start () {
		pos = -1;
		NextPosition ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.C)) {
			NextPosition ();
		}
		if (!moving) {
			return;
		}
		if (Vector3.Distance (cam.position, targetPos) < .05f) {
			moving = false;
		}
		print ("Moving " + Time.time);
		float timeSinceStarted = Time.time - startTime;
		float percentageComplete = timeSinceStarted / moveSpeed;
		cam.position = Vector3.Lerp (startPos, targetPos, percentageComplete);
		cam.rotation = Quaternion.Lerp (startRot, targetRot, percentageComplete);

	}
	void NextPosition () {
		print ("Next Called");
		pos++;
		Transform newTarget = positions [pos % positions.Count];
		targetPos = newTarget.position;
		targetRot = newTarget.rotation;
		startPos = cam.position;
		startRot = cam.rotation;
		startTime = Time.time;
		moving = true;

	}

}
