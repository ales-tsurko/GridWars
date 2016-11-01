using UnityEngine;
using System.Collections;

public class MatchmakerPlayerListState : MatchmakerState {
	// AppState

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);
	}

	// MatchmakerMenuDelegate

	public override void MatchmakerMenuOpened() {
		matchmaker.menu.Reset();
		foreach (var account in app.account.playerList) {
			matchmaker.menu.AddNewText()
				.SetText(account.screenName);
		}
		matchmaker.menu.AddNewButton()
			.SetText("Close")
			.SetAction(matchmaker.menu.Close)
			.SetIsBackItem(true);
		matchmaker.menu.Show();
	}

	public override void MatchmakerMenuClosed() {
		matchmaker.menu.Reset();
		matchmaker.menu.AddNewButton()
			.SetText(app.account.playerList.Count + " players online")
			.SetAction(matchmaker.menu.Open);
		matchmaker.menu.Show();
		matchmaker.menu.Focus();
	}
}
