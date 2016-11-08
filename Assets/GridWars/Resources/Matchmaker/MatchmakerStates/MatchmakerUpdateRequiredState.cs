using UnityEngine;
using System.Collections;

public class MatchmakerUpdateRequiredState : MatchmakerState {

	// MatchmakerMenuDelegate

	public override void ConfigureForOpen() {
		base.ConfigureForOpen();

		matchmaker.menu.Reset();
		matchmaker.menu.AddNewText()
			.SetText("Update to the latest version to play multiplayer");
		matchmaker.menu.AddNewButton()
			.SetText("Update")
			.SetAction(OpenSite);
		matchmaker.menu.AddNewButton()
			.SetText("Close")
			.SetAction(matchmaker.menu.Close)
			.SetIsBackItem(true);
		matchmaker.menu.Show();
	}

	public override void ConfigureForClosed() {
		base.ConfigureForClosed();

		matchmaker.menu.Reset();
		matchmaker.menu.AddNewButton()
			.SetText("UPDATE REQUIRED")
			.SetTextColor(Color.red)
			.SetAction(matchmaker.menu.Open)
			.UseAlertStyle();
		matchmaker.menu.Show();
		matchmaker.menu.Focus();
	}

	void OpenSite() {
		Application.OpenURL("http://www.baremetalgame.com/");
	}
}
