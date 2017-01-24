using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPartDelegate : MonoBehaviour {
	protected void PauseBattlefield() {
		App.shared.battlefield.isPaused = true;
	}

	protected void ResumeBattlefield() {
		App.shared.battlefield.isPaused = false;
	}

	protected TutorialPart tutorialPart {
		get {
			return GetComponent<TutorialPart>();
		}
	}

	public virtual void DidBegin() {

	}

	public virtual void TextDidComplete() {

	}

	public virtual void DidNext() {

	}
}