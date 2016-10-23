using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CameraController : MonoBehaviour {
	public List<Transform> positions = new List<Transform>();

	public List<SerializedTransform> gamePositions = new List<SerializedTransform>();
    [HideInInspector]
	public int pos;
	public bool moving;
	public float moveSpeed; // time in seconds to complete animation

	//Vector3 startPos;
	//Quaternion startRot;

	Vector3 targetPos;
	Quaternion targetRot;

	//float startTime;
	public Transform cam;
	MouseLook mouseLook;
	bool actionMode;
	bool initComplete = false;
	public KeyIconRotation keyIconRotation;
	public List<CameraControllerDelegate> cameraControllerDelegates;
	public bool isInFirstPerson;

	// orbits

	private bool isOrbiting = false;
	private Vector3 orbitCenter = new Vector3(0,0,0);
	private float orbitRadius = 80f;
	private float orbitPeriod = 50f; // second per cycles 
	private float orbitAngle;
	private float orbitHeight = 30f;

	void Start () {
		initComplete = false;
		mouseLook = cam.GetComponent<MouseLook> ();
		cameraControllerDelegates = new List<CameraControllerDelegate>();
		//DontDestroyOnLoad (gameObject);
	}
	public void InitCamera () {
		cameraControllerDelegates = new List<CameraControllerDelegate>();

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
				var keyIconRotation = transform.GetComponent<KeyIconRotation>();

				if (transform.gameObject.name == "TopDownBackView" || transform.gameObject.name == "MainBackView") {
					mirrorAxis = new Vector3(1, 1, -1);
				}
				else {
					mirrorAxis = new Vector3(-1, 1, 1);
				}
				
				keyIconRotation.rotation = new Vector3(keyIconRotation.rotation.x, keyIconRotation.rotation.y, keyIconRotation.rotation.z + 180);

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
		ResetCamera();
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

	public void StartOrbit() {
		isOrbiting = true;
	}

	public void EndOrbit() {
		if (isOrbiting) {
			isOrbiting = false;
			pos--;
			NextPosition();
		}
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
					isInFirstPerson = true;
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
		/*
		float timeSinceStarted = Time.time - startTime;
		float percentageComplete = timeSinceStarted / moveSpeed;

		cam.localPosition = Vector3.Lerp (startPos, targetPos, percentageComplete);
		cam.localRotation = Quaternion.Lerp (startRot, targetRot, percentageComplete);
		*/

		float pf = 0.05f;
		float rf = 0.05f;
		float v = 1f + 0.95f * Mathf.Sin(Time.time / 20f);

		// adjust target pos & rot

		if (isOrbiting) {
			//orbitAngle += 2f * Mathf.PI * Time.deltaTime / orbitPeriod;
			orbitAngle = 2f * Mathf.PI * Time.time / orbitPeriod;
			targetPos = new Vector3( 
				orbitRadius * Mathf.Cos(orbitAngle), 
				orbitHeight * v,
				orbitRadius * Mathf.Sin(orbitAngle)
			);
				
			var rotationLookAt = Quaternion.LookRotation(orbitCenter - cam.position);
			targetRot = Quaternion.Slerp(cam.rotation, rotationLookAt, 1f);

			if (Vector3.Distance(cam.position, targetPos) > 0.1f) {
				pf = 0.005f;
			} else {
				rf = 0.2f;
			}
		}

		// move towards target position & rotation

		cam.localPosition = Vector3.Lerp (cam.localPosition, targetPos, pf);
		cam.localRotation = Quaternion.Lerp (cam.localRotation, targetRot, rf);

	}

	void MoveToActionPosition (Transform _target) {
		if (!actionMode) {
			pos--;
		}
		actionMode = true;
		cam.parent = _target.transform;
		targetPos = Vector3.zero + new Vector3 (0, 2, 0);
		targetRot = Quaternion.Euler (Vector3.zero);
		//startPos = cam.localPosition;
		//startRot = cam.localRotation;
		//startTime = Time.time;
		moving = true;
	}

	public void NextPosition () {
		//print ("Next Called");
		actionMode = false;
		if (mouseLook != null) {
			mouseLook.enabled = false;
		}

		cam.parent = null;
		pos++;
		var transform = gamePositions[pos % gamePositions.Count];
		keyIconRotation = positions[pos % positions.Count].GetComponent<KeyIconRotation>();
		targetPos = transform.position;
		targetRot = transform.rotation;
		//startPos = cam.position;
		//startRot = cam.rotation;
		//print (Time.timeScale);
		//startTime = Time.time;
		moving = true;
        App.shared.prefs.camPosition = pos;

		foreach (var cameraControllerDelegate in cameraControllerDelegates) {
			cameraControllerDelegate.CameraControllerBeganTransition();
		}
       
	}

	public void ResetCamera () {
		UnlockCursor();
		cam.transform.parent =  null;
		NextPosition();
		StartCoroutine(ResetIsInFirstPerson());
	}

	//skip 1 frame so InGameMenu isn't focused
	IEnumerator ResetIsInFirstPerson() {
		yield return null;
		isInFirstPerson = false;
	}

	void UnlockCursor() {
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

    void OnDestroy (){
		UnlockCursor();
    }

}

public interface CameraControllerDelegate {
	void CameraControllerBeganTransition();
}