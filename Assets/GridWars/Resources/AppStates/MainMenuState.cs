using UnityEngine;
using System.Collections;

public class MainMenuState : AppState {
	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		app.battlefield.SoftReset();
		network.Reset();

		battlefield.isInternetPVP = false;
		battlefield.player1.isLocal = false;
		battlefield.player2.isLocal = false;

		app.ResetMenu();
		menu.AddItem(UI.MenuItem("Shared Screen PVP", SharedScreenPvpClicked));
		menu.AddItem(UI.MenuItem("Player vs AI", PlayerVsCompClicked));
		menu.AddItem(UI.MenuItem("AI vs AI", CompVsCompClicked));
		menu.AddItem(UI.MenuItem("Tutorial", Tutorial));
		menu.AddItem(UI.MenuItem("Chat", ChatClicked));
		menu.AddItem(UI.MenuItem("Quit", Quit));
		menu.Show();
		menu.backgroundColor = new Color(0, 0, 0, 1);

		matchmaker.menu.Show();

		App.shared.SoundtrackNamed("MenuBackgroundMusic").Play();
	}

	void SharedScreenPvpClicked() {
		battlefield.player1.isLocal = true;
		battlefield.player2.isLocal = true;

		TransitionTo(new BindInputsToPlayersState());
	}

	void PlayerVsCompClicked() {
		battlefield.player1.isLocal = true;
		battlefield.player2.isLocal = false;

		battlefield.player2.npcModeOn = true;

		TransitionTo(new WaitForBoltState());
	}

	void CompVsCompClicked() {
		battlefield.isAiVsAi = true;
		battlefield.player1.isLocal = false;
		battlefield.player2.isLocal = false;
		battlefield.player1.npcModeOn = true;
		battlefield.player2.npcModeOn = true;

		TransitionTo(new WaitForBoltState());
	}

	void ChatClicked() {
		Application.OpenURL("http://slack.baremetalgame.com/");
	}

	void Tutorial() {
		battlefield.isAiVsAi = true;

		battlefield.player1.isLocal = false;
		battlefield.player1.npcModeOn = true;
		battlefield.player1.isTutorialMode = true;

		battlefield.player2.isLocal = false;
		battlefield.player2.npcModeOn = true;
		battlefield.player2.isTutorialMode = true;

		TransitionTo(new WaitForBoltState());
	}

	void Quit() {
		Application.Quit();

		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#endif
	}
}
