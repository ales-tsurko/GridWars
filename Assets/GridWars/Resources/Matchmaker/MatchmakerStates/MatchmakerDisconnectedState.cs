using UnityEngine;
using System.Collections;

public class MatchmakerDisconnectedState : MatchmakerState {

	// AppState

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		app.account.ResetPlayerList();
		//TODO: countdown?
		matchmaker.Connect();
	}

	//MatchmakerDelegate

	public override void MatchmakerConnected() {
		TransitionTo(new MatchmakerPreAuthState());
	}

	public override void MatchmakerErrored() {
		base.MatchmakerErrored();

		//TODO: show retrying with countdown?
		TransitionTo(this);
	}

	// MatchmakerMenuDelegate

	public override void MatchmakerMenuOpened() {
		base.MatchmakerMenuOpened();

		matchmaker.menu.Reset();
		matchmaker.menu.AddNewText()
			.SetText("Unable to connect to the server.\n\nInternet matches disabled.");
		matchmaker.menu.AddNewButton()
			.SetText("Close")
			.SetAction(matchmaker.menu.Close)
			.SetIsBackItem(true);
		matchmaker.menu.Show();
	}

	public override void MatchmakerMenuClosed() {
		base.MatchmakerMenuClosed();

		matchmaker.menu.Reset();
		matchmaker.menu.AddNewButton()
			.SetText("CONNECT TO SERVER")
			.SetTextColor(Color.red)
			.SetAction(matchmaker.menu.Open)
			.UseAlertStyle();
		matchmaker.menu.Show();
		matchmaker.menu.Focus();
	}
}
