using UnityEngine;
using System.Collections;

public class MatchmakerPostAuthState : MatchmakerState {
	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		matchmaker.menu.Close();
	}
	// MatchmakerMenuDelegate

	public override void MatchmakerMenuClosed() {
		base.MatchmakerMenuClosed();

		var text = "Play PVP";
		if (app.account.otherPlayers.Count > 0) {
			text += ": " + app.account.otherPlayers.Count + " Online";
		}

		matchmaker.menu.Reset();
		matchmaker.menu.AddNewButton()
			.SetText(text)
			.SetAction(SearchForOpponent);
		matchmaker.menu.Show();
		matchmaker.menu.Focus();
	}

	void SearchForOpponent() {
		TransitionTo(new MatchmakerPostedGameState());
	}
}
