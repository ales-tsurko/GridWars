using UnityEngine;
using System.Collections;

public class MatchmakerPostAuthState : MatchmakerState {
	UIButton button;

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		matchmaker.menu.Close();
	}
	// MatchmakerMenuDelegate

	public override void ConfigureForClosed() {
		base.ConfigureForClosed();
		matchmaker.menu.Reset();
		button = matchmaker.menu.AddNewButton().SetAction(SearchForOpponent);
		UpdateText();
	}

	void UpdateText() {
		string text;

		/*
		if (app.state is MainMenuState) {
			if (app.account.otherPlayers.Count > 0) {
				text = app.account.otherPlayers.Count + " Online";
				App.shared.PlayAppSoundNamedAtVolume("PlayerConnected", 1f);
			}
			else {
				text = "Online";
			}
		}
		else {
			text = "Play PVP";
			if (app.account.otherPlayers.Count > 0) {
				text += ": " + app.account.otherPlayers.Count + " Online";
				App.shared.PlayAppSoundNamedAtVolume("PlayPVP", 1f);
			}
		}
		*/

		if (app.account.otherPlayers.Count > 0) {
			text = app.account.otherPlayers.Count + " Online";
			App.shared.PlayAppSoundNamedAtVolume("PlayerConnected", 1f);
		}
		else {
			text = "Online";
		}

		button.text = text;
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
