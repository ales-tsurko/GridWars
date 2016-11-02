using UnityEngine;
using System.Collections;

public class MatchmakerPlayerListState : MatchmakerState {
	// AppState

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);
	}

	// MatchmakerMenuDelegate

	public override void MatchmakerMenuOpened() {
	}

	public override void MatchmakerMenuClosed() {
		var text = "Play PVP";
		if (app.account.playerList.Count > 0) {
			text += ": " + app.account.playerList.Count + " Online";
		}

		matchmaker.menu.Reset();
		matchmaker.menu.AddNewButton()
			.SetText(text)
			.SetAction(SearchForOpponent);
		matchmaker.menu.Show();
		matchmaker.menu.Focus();
	}

	void SearchForOpponent() {
		TransitionTo(new MatchmakerSearchForOpponentState());
	}
}
