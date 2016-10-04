using UnityEngine;
using System.Collections;

public class MainMenuState : AppState {
	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		app.ResetMenu();
		menu.AddItem(UI.MenuItem("Internet PVP", InternetPvpClicked));
		menu.AddItem(UI.MenuItem("Shared Screen PVP", SharedScreenPvpClicked));
		menu.AddItem(UI.MenuItem("Player vs AI", PlayerVsCompClicked));
		menu.AddItem(UI.MenuItem("AI vs AI", CompVsCompClicked));
        menu.AddItem(UI.MenuItem("Options", OptionsClicked));
		menu.AddItem(UI.MenuItem("Quit", Quit));
		menu.Show();
	}
	
	void InternetPvpClicked() {
		TransitionTo(new MatchmakerState());
	}

	void SharedScreenPvpClicked() {
		battlefield.player1.isLocal = true;
		battlefield.player2.isLocal = true;

		TransitionTo(new WaitForBoltState());
	}

	void PlayerVsCompClicked() {
		battlefield.player1.isLocal = true;

		battlefield.player2.npcModeOn = true;

		TransitionTo(new WaitForBoltState());
	}

	void CompVsCompClicked() {
		battlefield.player1.npcModeOn = true;
		battlefield.player2.npcModeOn = true;

		TransitionTo(new WaitForBoltState());
	}

    void OptionsClicked() {
        TransitionTo(new OptionsMenuState());
    }

	void Quit() {
		Application.Quit();

		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#endif
	}
}
