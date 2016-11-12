using UnityEngine;
using System.Collections;

public class MatchmakerPreAuthState : MatchmakerState {
	// AppState

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		var data = new JSONObject();
		data.AddField("version", App.shared.version);

		var credentials = new JSONObject(JSONObject.Type.OBJECT);

		if (app.prefs.accessToken != null) {
			credentials.AddField("screenName", app.prefs.screenName);
			credentials.AddField("accessToken", app.prefs.accessToken);
		}

		data.AddField("credentials", credentials);

		matchmaker.Send("authenticate", data);
	}

	//MatchmakerDelegate

	public void HandleAuthenticate(JSONObject data) {
		app.account.screenName = data.GetField("screenName").str;
		app.account.accessToken = data.GetField("accessToken").str;

		app.prefs.screenName = app.account.screenName;
		app.prefs.accessToken = app.account.accessToken;

		foreach (var obj in data.GetField("players").list) {
			var account = new Account();
			account.screenName = obj.GetField("screenName").str;
			app.account.playerList.Add(account);
		}
		TransitionTo(new MatchmakerPostAuthState());
	}

	public void HandleAuthenticateFailed(JSONObject data) {
		app.prefs.screenName = "";
		app.prefs.accessToken = "";

		TransitionTo(new MatchmakerPreAuthState());
	}

	public void HandleUpdateRequired(JSONObject data) {
		TransitionTo(new MatchmakerUpdateRequiredState());
	}

	// MatchmakerMenuDelegate

	public override void ConfigureForClosed() {
		base.ConfigureForClosed();

		matchmaker.menu.Reset();
		matchmaker.menu.AddNewText()
			.SetText("Connecting to server");
	}

	public override void ConfigureForOpen() {
		base.ConfigureForOpen();

		matchmaker.menu.Reset();
		matchmaker.menu.AddNewIndicator()
			.SetText("Connecting to server");
		matchmaker.menu.AddNewButton()
			.SetText("OK")
			.SetAction(matchmaker.menu.Close);
	}
}
