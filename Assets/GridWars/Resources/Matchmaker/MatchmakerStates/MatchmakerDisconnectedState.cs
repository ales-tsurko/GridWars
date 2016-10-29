using UnityEngine;
using System.Collections;

public class MatchmakerDisconnectedState : MatchmakerState {
	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);
	}

	public override void MatchmakerMenuOpened() {
		matchmaker.menu.Reset();
		matchmaker.menu.AddNewText()
			.SetText("Unable to connect to the server.\nInternet matches disabled.");
		matchmaker.menu.AddNewButton()
			.SetText("Close")
			.SetAction(matchmaker.menu.Close)
			.SetIsBackItem(true);
		matchmaker.menu.Show();
	}

	public override void MatchmakerMenuClosed() {
		matchmaker.menu.Reset();
		matchmaker.menu.AddNewButton()
			.SetText("CONNECT TO SERVER")
			.SetTextColor(Color.red)
			.SetAction(matchmaker.menu.Open)
			.UseAlertStyle();
		matchmaker.menu.Show();
	}
}
