using UnityEngine;
using System.Collections;

public class MatchmakerPostAuthState : MatchmakerState {
	UIButton button;

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		matchmaker.menu.Close();
	}
	// MatchmakerMenuDelegate

	public override void MatchmakerMenuClosed() {
		base.MatchmakerMenuClosed();

		matchmaker.menu.Reset();
		button = matchmaker.menu.AddNewButton().SetAction(SearchForOpponent);
		UpdateText();
		matchmaker.menu.Show();
		matchmaker.menu.Focus();
	}

	void UpdateText() {
		var text = "Play PVP";
		if (app.account.otherPlayers.Count > 0) {
			text += ": " + app.account.otherPlayers.Count + " Online";
		}
		button.text = text;
	}

	void SearchForOpponent() {
		TransitionTo(new MatchmakerPostedGameState());
	}

	public virtual void HandlePlayerConnected(JSONObject data) {
		base.HandlePlayerConnected(data);
		UpdateText();
	}

	public virtual void HandlePlayerDisconnected(JSONObject data) {
		base.HandlePlayerDisconnected(data);
		UpdateText();
	}
}
