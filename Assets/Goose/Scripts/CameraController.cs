using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CameraController : MonoBehaviour {
	public List<Transform> positions = new List<Transform>();

	public List<SerializedTransform> gamePositions = new List<SerializedTransform>();
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
		mouseLook = cam.GetComponent<MouseLook> ();
		//DontDestroyOnLoad (gameObject);
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
			throw new System.Exception("Tower is null, can't init camera positions");
		}
		gamePositions = new List<SerializedTransform>();
		foreach (var transform in positions) {
			var gamePosition = new SerializedTransform(transform);
			gamePositions.Add(gamePosition);

			if (App.shared.battlefield.localPlayers.Count == 1 && App.shared.battlefield.PlayerNumbered(2).isLocal) {
				Vector3 mirrorAxis;
				if (transform.gameObject.name == "TopDownBackView" || transform.gameObject.name == "MainBackView") {
					mirrorAxis = new Vector3(1, 1, -1);
				}
				else {
					mirrorAxis = new Vector3(-1, 1, 1);
				}

				gamePosition.position = Vector3.Scale(gamePosition.position, mirrorAxis);
				gamePosition.rotation = Quaternion.Euler(gamePosition.rotation.eulerAngles + new Vector3(0f, 180f, 0f));
			}

			cam.position = gamePosition.position;
			cam.rotation = gamePosition.rotation;

			float mod = 0;
			#if UNITY_EDITOR
                thisScreenRes = lastScreenRes = GetMainGameViewSize();
			#endif
			while (true) {
				Vector3 screenPoint = cam.GetComponent<Camera> ().WorldToViewportPoint (_base.transform.position);
				if (screenPoint.z > 0.1f && screenPoint.x > 0.1f +mod && screenPoint.x < .9f-mod && screenPoint.y > 0.1f+mod && screenPoint.y < .9f-mod) {
					gamePosition.position = cam.position;
					break;
				} else {
					cam.transform.position -= cam.transform.forward;
				}
			}
		}

		cam.position = gamePositions[0].position;
		cam.rotation = gamePositions[0].rotation;
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

		if (Input.GetMouseButtonDown (0) && Input.GetKey(KeyCode.LeftShift)) {
			RaycastHit hit;
			Ray vRay = cam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast (vRay, out hit, 3000)) {
				if (hit.transform.GetComponent<GameUnit> ()) {
					MoveToActionPosition (hit.transform);	
				}
			}
		}
        if (cam == null) {
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

	public void NextPosition () {
		//print ("Next Called");
		actionMode = mouseLook.enabled = false;
		cam.parent = null;
		pos++;
		var transform = gamePositions[pos % gamePositions.Count];
		targetPos = transform.position;
		targetRot = transform.rotation;
		startPos = cam.position;
		startRot = cam.rotation;
		//print (Time.timeScale);
		startTime = Time.time;
		moving = true;
        App.shared.prefs.camPosition = pos;
       
	}

	public void ResetCamera () {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
		NextPosition ();
	}
    void OnDestroy (){
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}
