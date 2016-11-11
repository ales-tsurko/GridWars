using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {
	
	public List<Transform> positions = new List<Transform>();
	public List<SerializedTransform> gamePositions = new List<SerializedTransform>();

    [HideInInspector]

	public int pos;
	public bool moving;

	public Vector3 targetPos;
	public Quaternion targetRot;

	public Transform cam;
	MouseLook mouseLook;
	bool actionMode;
	public bool initComplete = false;
	public KeyIconRotation keyIconRotation;
	public List<CameraControllerDelegate> cameraControllerDelegates;
	public bool isInFirstPersonMode;
    public bool menuHasFocus;
    int FPSindex = 0;


	private List <GameObject> camLocations; 


	// camera modes

	private bool isInMainMenu = false;

	// orbits

	private bool isOrbiting = false;
	private Vector3 orbitCenter = new Vector3(0,0,0);
	private float orbitRadius = 80f;
	private float orbitPeriod = 50f; // second per cycles 
	private float orbitAngle;
	private float orbitHeight = 30f;


	float zoomRate = 0.05f;
	float rotationRate = 0.05f;

	//float currentZoomRate;
	//float currentRotationRate;

	Vector3 gamePositionPosAt(int index) {
		return gamePositions[index].position;
	}

	void SetupCamLocations() {
		camLocations = new List <GameObject>();
		GameObject obj = null;

		// 1
		obj = new GameObject();
		obj.transform.position = new Vector3(-5.6f, 52f, -1.5f);
		obj.transform.rotation = Quaternion.Euler(90, 0, 90);

		// 2
		obj = new GameObject();
		obj.transform.position = new Vector3(-4.1f, 85f, -6.8f);
		obj.transform.rotation = Quaternion.Euler(90, 0, 0);

		// 3
		obj = new GameObject();
		obj.transform.position = new Vector3(-3.4f, 46f, -57.7f);
		obj.transform.rotation = Quaternion.Euler(45.4f, 0, 0);

		// 4
		obj = new GameObject();
		obj.transform.position = new Vector3(0, 50, 0);
		obj.transform.rotation = Quaternion.Euler(90, 0, 90);

		// 5
		obj = new GameObject();
		obj.transform.position = new Vector3(45f, 32.2f, 0);
		obj.transform.rotation = Quaternion.Euler(33.4f, -90f, 0);
	}

	void Start () {

		initComplete = false;
		mouseLook = cam.GetComponent<MouseLook> ();
		cameraControllerDelegates = new List<CameraControllerDelegate>();
		//DontDestroyOnLoad (gameObject);
	}

	public void InitCamera () {
		SetupCamLocations();
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

	private bool didInit = false;
	public void InitCamera (Transform _base) {
		//isInMainMenu = true;
		if (!didInit) {

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
					} else {
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
					Vector3 screenPoint = cam.GetComponent<Camera>().WorldToViewportPoint(_base.transform.position);
					if (screenPoint.z > 0.1f && screenPoint.x > 0.1f + mod && screenPoint.x < .9f - mod && screenPoint.y > 0.1f + mod && screenPoint.y < .9f - mod) {
						gamePosition.position = cam.position;
						break;
					} else {
						cam.transform.position -= cam.transform.forward;
					}
				}
			}
			didInit = true;
		}


		//PickMainCameraLocation();
		//UseSlowZoomRate();

        pos = App.shared.prefs.camPosition - 1;
		ResetCamera();
        StartCoroutine(MonitorInitialCamMovement());
		initComplete = true;
        menuHasFocus = false;
	}
		
    IEnumerator MonitorInitialCamMovement () {
        yield return new WaitForEndOfFrame();
        while (moving) {
            yield return null;
        }
        foreach (PowerSource _powerSource in GameObject.FindObjectsOfType<PowerSource>()){
            _powerSource.gameStart = true;
        }
		if (BoltNetwork.isServer) {
			foreach(var player in App.shared.battlefield.players) {
					player.fortress.Unhide();
			}
		}
		yield break;
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

	// --- game state notifications ------------------

	public void MainMenuEntered() {
		isInMainMenu = true;
		UseSlowZoomRate();
		EndOrbit();
		PickMainCameraLocation();
		Debug.Log("CameraController - MainMenuEntered");
	}

	public void GameStarted() {
		isInMainMenu = false;
		EndOrbit();
		Debug.Log("CameraController - GameStarted");
	}

	public void GameEnded() {
		isInMainMenu = false;
		StartOrbit();
		Debug.Log("CameraController - GameEnded");
	}

	// --- orbit ---------------------------------------


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

	// --- zoom rates ---------------------------------------

	void ResetZoomRates() {
		zoomRate = 0.05f;
		rotationRate = 0.05f;
	}

	void UseSlowZoomRate2() {
		zoomRate = 0.025f;
		rotationRate = 0.025f;
	}

	void UseSlowZoomRate() {
		zoomRate = 0.03f;
		rotationRate = 0.03f;
	}

	void UpdateResolution() {
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
	}

	void CheckForFPSMode() {
		PlayerInputs inputs = App.shared.inputs;
		/* DISABLE FOR NOW
		if (Input.GetMouseButtonDown (0) && Input.GetKey(KeyCode.LeftShift)) {
			RaycastHit hit;
			Ray vRay = cam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast (vRay, out hit, 3000)) {
				if (hit.transform.GetComponent<GameUnit> ()) {
					isInFirstPersonMode = true;
					MoveToActionPosition (hit.transform);	
				}
			}
		}
		*/

		//check for Joystick input for FPS Mode
		if (isInFirstPersonMode) {
			if (inputs.goBack.WasPressed || inputs.exitFPS.WasPressed) {
				FindObjectOfType<CameraController>().ResetCamera();
				return;
			}
			FPSindex += (inputs.unitNext.WasPressed || inputs.rightItem.WasPressed) ? ChangeFPSUnit(1) : 0;
			FPSindex += (inputs.unitPrev.WasPressed || inputs.leftItem.WasPressed) ? ChangeFPSUnit(-1) : 0;
		}
		if (!isInFirstPersonMode && App.shared.inputs.enterFPS.WasPressed) {
			EnterFPSModeFromJoystick();
		}

		if (cam == null) {
			return;
		}
		//check for Camera position change

		/*
		 * DISABLE UNTIL WE RETHINK FIRST PERSON
        if (!isInFirstPersonMode && (inputs.camPrev.WasPressed || inputs.camNext.WasPressed)) {
            NextPosition(inputs.camNext.WasPressed);
        }
        */
	}

	void DetectStoppedMoving() {
		//if (Vector3.Distance (cam.localPosition, targetPos) < .05f 
		//		&& Quaternion.Angle(cam.localRotation, targetRot) < .1f) {

		if (Vector3.Distance (cam.localPosition, targetPos) < .05f*600f 
			&& Quaternion.Angle(cam.localRotation, targetRot) < .1f*600f) {
			if (actionMode) {
				mouseLook.enabled = true;
			}

			moving = false;
			ResetZoomRates();
		}
	}

	void Update () {
		if (isInMainMenu) {
			UpdateForMainMenu();
			return;
		}

		UpdateResolution();
		//CheckForFPSMode();
		DetectStoppedMoving();

		if (isOrbiting) {
			UpdateForOrbit(); 
		} else {
			UpdateForInGame();
		}
	}

	// --- update modes: main menu, in game, orbit

	private Vector3 mainTargetPos = Vector3.zero;
	private Quaternion mainTargetRot = Quaternion.identity;

	void PickMainCameraLocation() {
		Vector2 r = Random.insideUnitCircle * 1200f;
		//mainTargetPos = new Vector3(r.x, 200f + 200f * UnityEngine.Random.value, r.y);
		mainTargetPos = new Vector3(r.x, 200f, r.y);
		mainTargetRot = Quaternion.LookRotation(-mainTargetPos); // look at zero
		Vector3 e = mainTargetRot.eulerAngles;
		e.x = -10;
		mainTargetRot.eulerAngles = e;
	}

	void UpdateForMainMenu() {		
		if (mainTargetPos == Vector3.zero) {
			PickMainCameraLocation();

		}

		float pf = zoomRate; // 0.05f;
		float rf = rotationRate; //0.05f;

		/*
		targetPos = new Vector3(0, 60f, 0);
		Vector3 e = targetRot.eulerAngles;
		e.x = 0f;
		e.y += 0.1f;
		e.z = 0f;
		targetRot.eulerAngles = e;
		*/
		targetPos = mainTargetPos;
		targetRot = mainTargetRot;

		cam.localPosition = Vector3.Lerp (cam.localPosition, targetPos, pf * Time.deltaTime * 60f);
		cam.localRotation = Quaternion.Lerp (cam.localRotation, targetRot, rf * Time.deltaTime * 60f);
	}

	void UpdateForInGame() {
		float pf = zoomRate; // 0.05f;
		float rf = rotationRate; //0.05f;

		if (App.shared.testEndOfGameMode) {
			pf = 1.0f;
			rf = 1.0f;
		}

		cam.localPosition = Vector3.Lerp (cam.localPosition, targetPos, pf * Time.deltaTime * 60f);
		cam.localRotation = Quaternion.Lerp (cam.localRotation, targetRot, rf * Time.deltaTime * 60f);
	}

	float DistanceToCamTarget() {
		return Vector3.Distance(cam.position, targetPos);
	}

	void UpdateForOrbit() {
		float pf = zoomRate; // 0.05f;
		float rf = rotationRate; //0.05f;

		// adjust target pos & rot

		float v = 1f + 0.95f * Mathf.Sin(Time.time / 20f);

		orbitAngle = 2f * Mathf.PI * Time.time / orbitPeriod;
		targetPos = new Vector3( 
			orbitRadius * Mathf.Cos(orbitAngle), 
			orbitHeight * v,
			orbitRadius * Mathf.Sin(orbitAngle)
		);
			
		var rotationLookAt = Quaternion.LookRotation(orbitCenter - cam.position);
		targetRot = Quaternion.Slerp(cam.rotation, rotationLookAt, 1f);

		// slowly move into orbit if we're not in it yet
		if (DistanceToCamTarget() > 0.1f) {
			pf = 0.005f;
		} else {
			rf = 0.2f;
		}
			
		cam.localPosition = Vector3.Lerp (cam.localPosition, targetPos, pf * Time.deltaTime * 60f);
		cam.localRotation = Quaternion.Lerp (cam.localRotation, targetRot, rf * Time.deltaTime * 60f);
	}

    void EnterFPSModeFromJoystick () {
        List<GameUnit> units = GetUnits();
        units.RemoveAll(i => i.gameObject.layer == 10);
        if (units.Count > 0) {
            FPSindex = units.Count - 1;
            isInFirstPersonMode = true;
            MoveToActionPosition (units[FPSindex].transform);
        }
    }

    int ChangeFPSUnit (int x) {
        int index = FPSindex + x;
        List<GameUnit> units = GetUnits();
        units.RemoveAll(i => i == null || i.gameObject.layer == 10);
        if (units == null) {
            ResetCamera();
            return 0;
        }
        index = index == -1 ? units.Count - 1 : index % units.Count;
        MoveToActionPosition(units[index].transform);
        return index - FPSindex;
    }

   List<GameUnit> GetUnits () {
        Player[] players = FindObjectsOfType<Player>();
        foreach (Player player in players){
            if (player.isLocalPlayer1){
                return player.units;
            }
        }
        return new List<GameUnit>();
    }


	void MoveToActionPosition (Transform _target) {
		if (!actionMode) {
			pos--;
		}
		actionMode = true;
		cam.parent = _target.transform;
		targetPos = Vector3.zero + new Vector3 (0, 2, 0);
		targetRot = Quaternion.Euler (Vector3.zero);
		moving = true;
	}

	//ResetZoomRates

    public void NextPosition (bool next = true) {
		//print ("Next Called");
        int _dir = next ? 1 : -1;
		actionMode = false;
		if (mouseLook != null) {
			mouseLook.enabled = false;
		}

		cam.parent = null;
        pos += _dir;
        var _position = Mathf.Abs(pos % gamePositions.Count);
        var _transform = gamePositions[_position];
        keyIconRotation = positions[_position].GetComponent<KeyIconRotation>();
		targetPos = _transform.position;
		targetRot = _transform.rotation;
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
		isInFirstPersonMode = false;
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