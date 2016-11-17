using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {
	public static string CameraControllerBeganTransitionNotification = "CameraControllerBeganTransitionNotification";

	public List<Transform> referencePositions = new List<Transform>();
	public List<GameObject> finalPositions = new List<GameObject>();

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
	public bool isInFirstPersonMode;
    public bool menuHasFocus;
    int FPSindex = 0;

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

	void Start () {

		initComplete = false;
		mouseLook = cam.GetComponent<MouseLook> ();
		//DontDestroyOnLoad (gameObject);
	}

	public void InitCamera () {
		SetupFinalPositions();

		pos = positionForName(App.shared.prefs.cameraPosition) - 1;
		ResetCamera();
		StartCoroutine(MonitorInitialCamMovement());
		initComplete = true;
		menuHasFocus = false;
	}

	int positionForName(string name) {
		return Mathf.Max(referencePositions.FindIndex(p => p.name == name), 0);
	}
		
	public void SetupFinalPositions () {
		List<GameObject> towerPlacements;
		if (App.shared.battlefield.localPlayer1 == null) {
			towerPlacements = App.shared.battlefield.player1.fortress.towerPlacements;
		}
		else {
			towerPlacements = App.shared.battlefield.localPlayer1.fortress.towerPlacements;
		}
			

		if (finalPositions.Count == 0) {
			foreach (var transform in referencePositions) {
				var obj = new GameObject();
				obj.name = transform.name + " (Final)";
				obj.transform.parent = transform.parent;
				obj.AddComponent<KeyIconRotation>();
				finalPositions.Add(obj);
			}
		}

		var savedCamPosition = cam.transform.position;
		var savedCamRotation = cam.transform.rotation;

		var i = 0;
		foreach (var transform in referencePositions) {
			var finalPosition = finalPositions[i];

			var keyIconRotation = transform.GetComponent<KeyIconRotation>();

			if (App.shared.battlefield.localPlayers.Count == 1 && App.shared.battlefield.PlayerNumbered(2).isLocal) {
				Vector3 mirrorAxis;

				if (transform.gameObject.name == "TopDownBackView" || transform.gameObject.name == "MainBackView") {
					mirrorAxis = new Vector3(1, 1, -1);
				}
				else {
					mirrorAxis = new Vector3(-1, 1, 1);
				}
					
				finalPosition.GetComponent<KeyIconRotation>().rotation = new Vector3(keyIconRotation.rotation.x, keyIconRotation.rotation.y, keyIconRotation.rotation.z + 180);

				finalPosition.transform.position = Vector3.Scale(transform.position, mirrorAxis);
				finalPosition.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));
			}
			else {
				finalPosition.GetComponent<KeyIconRotation>().rotation = new Vector3(keyIconRotation.rotation.x, keyIconRotation.rotation.y, keyIconRotation.rotation.z);
				finalPosition.transform.position = transform.position;
				finalPosition.transform.rotation = transform.rotation;
			}

			var sorted = new List<GameObject>(towerPlacements);
				
			sorted.Sort((a, b) => {
				var aSqrMag = (a.transform.position - finalPosition.transform.position).sqrMagnitude;
				var bSqrMag = (b.transform.position - finalPosition.transform.position).sqrMagnitude;

				return aSqrMag.CompareTo(bSqrMag);
			});

			var closest = sorted[0];

			float mod = 0;
			var steps = 0;

			cam.transform.position = finalPosition.transform.position;
			cam.transform.rotation = finalPosition.transform.rotation;
			while (steps < 100) {
				Vector3 screenPoint = cam.GetComponent<Camera>().WorldToViewportPoint(closest.transform.position);

				if (screenPoint.z > 0.1f && screenPoint.x > 0.1f + mod && screenPoint.x < .9f - mod && screenPoint.y > 0.1f + mod && screenPoint.y < .9f - mod) {
					break;
				}
				else {
					cam.transform.position -= cam.transform.forward;
				}
				steps ++;
			}

			finalPosition.transform.position = cam.transform.position;
			finalPosition.transform.rotation = cam.transform.rotation;

			i ++;
		}

		cam.transform.position = savedCamPosition;
		cam.transform.rotation = savedCamRotation;
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
		var _position = Mathf.Abs(pos % finalPositions.Count);
		var _transform = finalPositions[_position].transform;
		keyIconRotation = finalPositions[_position].GetComponent<KeyIconRotation>();
		targetPos = _transform.position;
		targetRot = _transform.rotation;
		moving = true;
		App.shared.prefs.cameraPosition = referencePositions[_position].name;

		App.shared.notificationCenter.NewNotification()
			.SetName(CameraControllerBeganTransitionNotification)
			.SetSender(this)
			.Post();
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