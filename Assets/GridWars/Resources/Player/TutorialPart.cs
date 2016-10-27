using UnityEngine;
using System.Collections;

public class TutorialPart : MonoBehaviour {

	public string targetName;
	public string hoverText;

	public float timeout = 30f;
	public float nextTime = 0f;
	public float yOffset = 10f;
	public float characterSize = 7f;

	public float fractionToTarget = 0f;

	public GameObject nextPart;

	void Start () {
	}

	public GameObject Target() {
		return GameObject.Find(targetName);
	}

	public void Begin() {
		if (Target() == null) {
			Debug.Log("missing target on " + gameObject.name);
			return;
		}

		TutorialLabel().SetActive(true);
		gameObject.SetActive(true);
		nextTime = Time.time + timeout;

		Vector3 diff = transform.position - Target().transform.position;

		transform.position = transform.position - diff * fractionToTarget;

		App.shared.cameraController.targetPos = transform.position;

		//App.shared.cameraController.targetRot = transform.rotation;
		//App.shared.cameraController.targetRot = Quaternion.LookRotation(transform.position - Target().transform.position);

		App.shared.cameraController.targetRot = Quaternion.LookRotation(Target().transform.position - transform.position);

		Vector3 p = Target().transform.position;
		p.y += yOffset;
		TutorialLabel().transform.position = p;
		SetTutorialLabelText(hoverText);
	}
	
	void Update () {
		if (Input.GetKeyDown("space")) {
			Next();
		}
	}

	/*
	GameObject NextPart() {
		Regex x = new Regex("(\\(\\])(.*?)(\\)\\])");
		string repl = "the replacement text";
		string Result = x.Replace(gameObject.name, "$1" + gameObject.name + "$3");
	}
	*/

	void Next() {
		gameObject.SetActive(false);
		App.shared.PlayAppSoundNamedAtVolume("MenuItemClicked", 0.5f);

		if (nextPart != null) {
			nextPart.GetComponent<TutorialPart>().Begin();
		} else {
			TutorialLabel().SetActive(false);
			DoneTutorial();
		}
	}

	public GameObject TutorialLabel() {
		return GameObject.Find("TutorialLabel");
	}

	public void SetTutorialLabelText(string s) {
		TextMesh tm = GameObject.Find("TutorialLabelText").GetComponent<TextMesh>();

		s = s.Replace("NEWLINE","\n");
		//s += "\n\n[ press space ]";
		tm.text = s.ToUpper();
		tm.characterSize = characterSize;
	}


	public void DoneTutorial() {
		App.shared.battlefield.isAiVsAi = false;

		App.shared.battlefield.player1.isLocal = true;
		App.shared.battlefield.player1.isTutorialMode = false;
		App.shared.battlefield.player1.npcModeOn = false;

		App.shared.battlefield.player2.isTutorialMode = false;
		App.shared.battlefield.player2.npcModeOn = true;

		App.shared.cameraController.ResetCamera();
		App.shared.cameraController.pos = 1;
		App.shared.cameraController.NextPosition();
	}
}
