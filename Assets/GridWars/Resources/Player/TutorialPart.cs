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
	public bool _hasBegun = false;

	private int counter = 0;
	private int countsPerCharacter = 3;

	private TextMesh _textMesh = null;
	private string _formattedText = null;

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

		enabled = true;
		TutorialLabel().GetComponent<HoverText>().enabled = true;
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
		//SetTutorialLabelText(hoverText);


		_textMesh = GameObject.Find("TutorialLabelText").GetComponent<TextMesh>();
		_formattedText = hoverText.Replace("NEWLINE", "\n").ToUpper();
		_hasBegun = true;
	}
	
	void FixedUpdate () {
		if (_hasBegun) {
			if (Input.GetKeyDown("space")) {
				_hasBegun = false;
				App.shared.timerCenter.NewTimer().SetTimeout(0.1f).SetAction(Next).Start();
				//Next();
			}

			counter ++;

			if (counter % countsPerCharacter == 0) {
				SetTutorialLabelText(VisibleText());
			}
		}
	}

	public string VisibleText() {
		string s = _formattedText;
		int max = Mathf.Clamp(counter / countsPerCharacter, 0, s.Length);
		return s.Substring(0, max) + s.Substring(max, s.Length - max).ReplacedNonWhiteSpaceWithSpaces();
	}

	void Next() {
		enabled = false;
		SetTutorialLabelText("");
		App.shared.PlayAppSoundNamedAtVolume("MenuItemClicked", 0.5f);

		if (nextPart != null) {
			nextPart.GetComponent<TutorialPart>().Begin();
		} else {
			TutorialLabel().GetComponent<HoverText>().enabled = false;
			DoneTutorial();
		}
	}

	public GameObject TutorialLabel() {
		return GameObject.Find("TutorialLabel");
	}

	public void SetTutorialLabelText(string s) {
		if (_textMesh.text != s) {
			_textMesh.text = s;
			_textMesh.characterSize = characterSize;
			App.shared.PlayAppSoundNamedAtVolume("bleep2", 0.02f);
		}
	}

	public void DoneTutorial() {
		App.shared.battlefield.isAiVsAi = false;

		App.shared.battlefield.player1.isLocal = true;
		App.shared.battlefield.player1.isTutorialMode = false;
		App.shared.battlefield.player1.npcModeOn = false;

		App.shared.battlefield.player2.isTutorialMode = false;
		App.shared.battlefield.player2.npcModeOn = true;

		App.shared.cameraController.ResetCamera();
		App.shared.cameraController.pos = 0;
		App.shared.cameraController.NextPosition();
	}
}
