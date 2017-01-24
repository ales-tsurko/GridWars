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
	public bool preventsNext;

	public TutorialPartDelegate tutorialPartDelegate {
		get {
			return GetComponent<TutorialPartDelegate>();
		}
	}

	private int counter = 0;
	private int countsPerCharacter = 3;

	private TextMesh _textMesh = null;
	private string _formattedText = null;

	private Observation exitObservation = null;

	void Start () {

	}

	void ObserveExit () {
		if (exitObservation == null) {
			exitObservation = App.shared.notificationCenter.NewObservation();
			exitObservation.SetNotificationName("AppStateChangedNotification");
			exitObservation.SetAction(AppStateChangedNotification);
			exitObservation.Add();
		}
	}

	public void AppStateChangedNotification(Notification note) {
		if (App.shared.state is MainMenuState) {
			WillExit();
		}
	}

	public void WillExit() {
		TurnOff();
	}


	public GameObject Target() {
		return GameObject.Find(targetName);
	}

	public void Begin() {
		if (Target() == null) {
			Debug.Log("missing target on " + gameObject.name);
			return;
		}

		App.shared.notificationCenter.NewObservation()
			.SetNotificationName(InGameMenu.InGameMenuOpenedNotification)
			.SetAction(MenuOpened)
			.Add();

		App.shared.notificationCenter.NewObservation()
			.SetNotificationName(MatchmakerMenu.MatchmakerMenuOpenedNotification)
			.SetAction(MenuOpened)
			.Add();

		App.shared.notificationCenter.NewObservation()
			.SetNotificationName(InGameMenu.InGameMenuClosedNotification)
			.SetAction(MenuClosed)
			.Add();

		App.shared.notificationCenter.NewObservation()
			.SetNotificationName(MatchmakerMenu.MatchmakerMenuClosedNotification)
			.SetAction(MenuClosed)
			.Add();

		ObserveExit();

		enabled = true;
		TutorialLabel().GetComponent<HoverText>().enabled = true;
		nextTime = Time.time + timeout;

		Vector3 diff = transform.position - Target().transform.position;

		transform.position = transform.position - diff * fractionToTarget;

		App.shared.cameraController.targetPos = transform.position;
		App.shared.cameraController.targetRot = Quaternion.LookRotation(Target().transform.position - transform.position);

		Vector3 p = Target().transform.position;
		p.y += yOffset;
		TutorialLabel().transform.position = p;
		//SetTutorialLabelText(hoverText);


		_textMesh = GameObject.Find("TutorialLabelText").GetComponent<TextMesh>();

		Reset();

		if (tutorialPartDelegate != null) {
			tutorialPartDelegate.DidBegin();
		}
	}

	public void Reset() {
		counter = 0;

		_formattedText = hoverText.Replace("NEWLINE", "\n").ToUpper();
		_formattedText = _formattedText.Replace("CONTINUEHOTKEY", App.shared.inputs.continueTutorial.HotkeyDescription(20)).ToUpper();

		string replacement;
		if (App.shared.inputs.LastInputType == InControl.BindingSourceType.DeviceBindingSource) {
			App.shared.battlefield.player1.isLocal = true;
			App.shared.prefs.keyIconsVisible = true;
			replacement = "press your towers button";
		}
		else {
			replacement = "click your towers";
		}

		_formattedText = _formattedText.Replace("CLICKYOURTOWERS", replacement).ToUpper();
		_hasBegun = true;
	}

	void MenuOpened(Notification n) {
		ShowHideText();
	}

	void MenuClosed(Notification n) {
		ShowHideText();
	}

	void ShowHideText() {
		if (App.shared.matchmaker.menu.isOpen || (App.shared.state as PlayingGameState).primaryInGameMenu.isOpen) {
			_textMesh.GetComponent<Renderer>().enabled = false;
		}
		else {
			_textMesh.GetComponent<Renderer>().enabled = true;
		}
	}

	void Update() {
		if (_hasBegun) {
			if (App.shared.inputs.continueTutorial.WasPressed) {
				if (counter / countsPerCharacter >= _formattedText.Length && !preventsNext) {
					_hasBegun = false;
					App.shared.timerCenter.NewTimer().SetTimeout(0.1f).SetAction(Next).Start();
				}
				else {
					counter = _formattedText.Length * countsPerCharacter;
				}

				//Next();
			}

			if (counter == _formattedText.Length * countsPerCharacter) {
				if (tutorialPartDelegate != null) {
					tutorialPartDelegate.TextDidComplete();
				}
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

	public void Next() {
		TurnOff();
		App.shared.PlayAppSoundNamedAtVolume("MenuItemClicked", 0.5f);

		_textMesh.GetComponent<Renderer>().enabled = true;
		App.shared.notificationCenter.RemoveObserver(this);

		if (tutorialPartDelegate != null) {
			tutorialPartDelegate.DidNext();
		}

		if (nextPart != null) {
			nextPart.GetComponent<TutorialPart>().Begin();
		} else {
			TutorialLabel().GetComponent<HoverText>().enabled = false;
		}
	}

	void TurnOff() {
		enabled = false;
		SetTutorialLabelText("");
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

	void OnDestroy() {
		if (App.shared.notificationCenter != null) {
			App.shared.notificationCenter.RemoveObserver(this);
		}
	}
}