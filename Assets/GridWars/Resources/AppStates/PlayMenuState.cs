using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;

public class PlayMenuState : AppState {
	UIButton internetPvpButton;

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		if (ShouldPlayTutorial()) {
			Tutorial();
		}
		else {
			ShowMenu();
		}
	}

	public override void WillExit() {
		base.WillExit();

		app.notificationCenter.RemoveObserver(this);
	}

	void ShowMenu() {
		menu.Reset();
		menu.AddNewButton().SetText("Singleplayer").SetAction(Singleplayer);
		menu.AddNewButton().SetText("Multiplayer").SetAction(Multiplayer);
		menu.AddNewButton().SetText("Tutorial").SetAction(Tutorial);
		menu.AddNewButton().SetText("Back").SetAction(Back).SetIsBackItem(true);
		menu.Show();
	}

	void Singleplayer() {
		TransitionTo(new SinglePlayerMenuState());
	}

	void Multiplayer() {
		TransitionTo(new MultiPlayerMenuState());
	}

	bool ShouldPlayTutorial() {
		return !app.prefs.hasPlayedTutorial;
	}

	void Tutorial() {
		app.prefs.hasPlayedTutorial = true;

		battlefield.isAiVsAi = true;

		battlefield.player1.isLocal = false;
		battlefield.player1.npcModeOn = true;
		battlefield.player1.isTutorialMode = true;
		battlefield.player1.firstTutorial = null;

		battlefield.player2.isLocal = false;
		battlefield.player2.npcModeOn = true;
		battlefield.player2.isTutorialMode = true;

		/*Analytics.CustomEvent("TutorialClicked", new Dictionary<string, object> {
			{ "playTime", Time.timeSinceLevelLoad }
		});*/

		TransitionTo(new WaitForBoltState());
	}

	void Back() {
		TransitionTo(new MainMenuState());
	}
}
