using UnityEngine;
using System.Collections;

public class MatchmakerUpdateRequiredState : MatchmakerState {

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		matchmaker.menu.isInteractible = true;
		matchmaker.menu.Close();
	}

	// MatchmakerMenuDelegate

	public override void ConfigureForOpen() {
		base.ConfigureForOpen();

		matchmaker.menu.Reset();
		matchmaker.menu.AddNewText()
			.SetText("Download to the latest version to play multiplayer");
		matchmaker.menu.AddNewButton()
			.SetText("Update")
			.SetAction(OpenSite);
		matchmaker.menu.AddNewButton()
			.SetText("Close")
			.SetAction(matchmaker.menu.Close)
			.SetIsBackItem(true);
	}

	public override void ConfigureForClosed() {
		base.ConfigureForClosed();

		matchmaker.menu.Reset();
		matchmaker.menu.AddNewButton()
			.SetText("UPDATE REQUIRED")
			.SetTextColor(Color.red)
			.SetAction(matchmaker.menu.Open)
			.UseAlertStyle();
	}

	void OpenSite() {
		Application.OpenURL("http://www.baremetalgame.com/");
	}
}
