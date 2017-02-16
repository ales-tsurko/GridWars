using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPartClosing : TutorialPartDelegate {
	public override void DidNext() {
		App.shared.state.TransitionTo(new PlayingGameState());
	}
}
