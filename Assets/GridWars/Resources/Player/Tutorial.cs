/*
 * using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {

	public bool _isRunning = false;

	public void Run() {
		if (playerNumber == 1 && firstTutorial == null && App.shared.cameraController.initComplete) {
			firstTutorial = GameObject.Find("TutorialStart");
			firstTutorial.GetComponent<TutorialPart>().Begin();
		}
	}

	public void Stop() {

	}

	void Start () {
	
	}
	
	void Update () {
	
	}
}
*/