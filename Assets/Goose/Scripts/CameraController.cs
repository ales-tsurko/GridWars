using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CameraController : MonoBehaviour {
	private static CameraController _instance;
	public static CameraController instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<CameraController> ();
			}
			return _instance;
		}
	}
	public List<Transform> positions = new List<Transform>();
	[System.Serializable]
	public class OriginalPosition {
		public Vector3 position;
		public Quaternion rotation;
	}
	public List<OriginalPosition> originalPositions = new List<OriginalPosition>();
    [HideInInspector]
	public int pos;
	public bool moving;
	public float moveSpeed;
	Vector3 startPos;
	Vector3 targetPos;
	Quaternion startRot;
	Quaternion targetRot;
	float startTime;
	public Transform cam;
	MouseLook mouseLook;
	bool actionMode;
	bool initComplete = false;
	void Start () {
		initComplete = false;
		foreach (Transform pos in positions) {
			originalPositions.Add (new OriginalPosition () { position = pos.position, rotation = pos.rotation });
		}
		mouseLook = cam.GetComponent<MouseLook> ();
		//DontDestroyOnLoad (gameObject);
		InitCamera ();
	}
	public void InitCamera () {
		StartCoroutine (WaitForTowers ());
	}
	IEnumerator WaitForTowers () {
		Tower[] towers = FindObjectsOfType<Tower> ();
        while (towers.Length == 0 || App.shared == null) {
			towers = FindObjectsOfType<Tower> ();
			yield return null;
		}
		Tower closest = towers[0];
		float closestDist = Mathf.Infinity;
		foreach (Tower tower in towers) {
			float thisDist = Vector3.Distance (tower.transform.position, cam.transform.position);
			if (thisDist < closestDist) {
				closestDist = thisDist;
				closest = tower;
			}
		}
		InitCamera (closest.transform);
	}
	public void InitCamera (Transform _base){
		if (_base == null) {
			Debug.LogError ("Tower is null, can't init camera positions");
			return;
		}
		for (int i = 0; i < positions.Count; i++) {
			cam.position = originalPositions [i].position;
			cam.rotation = originalPositions [i].rotation;
			float mod = 0;
			#if !UNITY_EDITOR
            thisScreenRes = lastScreenRes = GetMainGameViewSize();
			#endif
			while (true) {
				Vector3 screenPoint = cam.GetComponent<Camera> ().WorldToViewportPoint (_base.transform.position);
				if (screenPoint.z > 0.1f && screenPoint.x > 0.1f +mod && screenPoint.x < .9f-mod && screenPoint.y > 0.1f+mod && screenPoint.y < .9f-mod) {
					positions [i].position = cam.position;
					break;
				} else {
					cam.transform.position -= cam.transform.forward;
				}
			}
		}

		cam.position = positions [0].position;
		cam.rotation = positions [0].rotation;
        pos = App.shared.prefs.camPosition - 1;
		NextPosition ();
		initComplete = true;
	}
    [HideInInspector]
    Vector2 lastScreenRes, thisScreenRes;
	public static Vector2 GetMainGameViewSize()
	{
		System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
		System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
		System.Object Res = GetSizeOfMainGameView.Invoke(null,null);
		return (Vector2)Res;
	}

	void Update () {
		if (!initComplete) {
			return;
		}
		#if UNITY_EDITOR
		if (Time.frameCount % 10 == 0){
			thisScreenRes = GetMainGameViewSize();
			if (thisScreenRes != lastScreenRes){
				lastScreenRes = thisScreenRes;
				//InitCamera();
				return;
			}
		}
		#endif
        if (Input.GetKeyDown (Keys.CHANGECAM.GetKey())) {
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

	public void NextPosition () {
		print ("Next Called");
		actionMode = mouseLook.enabled = false;
		cam.parent = null;
		pos++;
		Transform newTarget = positions [pos % positions.Count];
		targetPos = newTarget.position;
		targetRot = newTarget.rotation;
		startPos = cam.position;
		startRot = cam.rotation;
		//print (Time.timeScale);
		startTime = Time.time;
		moving = true;
        App.shared.prefs.camPosition = pos;
       
	}

	public void ResetCamera () {
		NextPosition ();
	}

}
