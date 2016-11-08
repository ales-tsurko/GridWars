using UnityEngine;
using System.Collections;

public class MatchmakerPostAuthState : MatchmakerState {
	UIButton button;

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		matchmaker.menu.Close();

		MainMenuState mainMenuState = app.state as MainMenuState;
		if (mainMenuState != null) {
			mainMenuState.MatchmakerConnected();
		}
	}
	// MatchmakerMenuDelegate

	public override void MatchmakerMenuClosed() {
		base.MatchmakerMenuClosed();

		matchmaker.menu.Reset();
		button = matchmaker.menu.AddNewButton().SetAction(SearchForOpponent);
		UpdateText();
	}

	void UpdateText() {
		string text;

		if (app.state is MainMenuState) {
			if (app.account.otherPlayers.Count > 0) {
				text = app.account.otherPlayers.Count + " Online";
			}
			else {
				text = "Online";
			}
		}
		else {
			text = "Play PVP";
			if (app.account.otherPlayers.Count > 0) {
				text += ": " + app.account.otherPlayers.Count + " Online";
			}
		}
			

		button.text = text;
	}

	public override void MainMenuEntered() {
		base.MainMenuEntered();

		UpdateText();
	}

	public override void MainMenuExited() {
		base.MainMenuExited();

		UpdateText();
	}

	void SearchForOpponent() {
		TransitionTo(new MatchmakerPostedGameState());
	}

	public override void MainMenuInternetPvpClicked() {
		SearchForOpponent();
		base.MainMenuInternetPvpClicked();
	}

	public override void HandlePlayerConnected(JSONObject data) {
		base.HandlePlayerConnected(data);
		UpdateText();
	}

	public override void HandlePlayerDisconnected(JSONObject data) {
		base.HandlePlayerDisconnected(data);
		UpdateText();
	}
}
