using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPartClosing : TutorialPartDelegate {
	public override void DidNext() {
		App.shared.battlefield.isPaused = false;

		App.shared.battlefield.isAiVsAi = false;

		App.shared.battlefield.player1.isLocal = true;
		App.shared.battlefield.player1.isTutorialMode = false;
		App.shared.battlefield.player1.npcModeOn = false;

		App.shared.battlefield.player2.isTutorialMode = false;
		App.shared.battlefield.player2.npcModeOn = true;

		App.shared.state.TransitionTo(new PlayingGameState());
	}
}
