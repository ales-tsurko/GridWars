using UnityEngine;
using System.Collections;

public class MatchmakerPreAuthState : MatchmakerState {
	// AppState

	bool sendsCredentials = true;

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		var data = new JSONObject();
		data.AddField("version", App.shared.version);

		var credentials = new JSONObject(JSONObject.Type.OBJECT);

		if (sendsCredentials && app.prefs.accessToken != null) {
			credentials.AddField("screenName", app.prefs.screenName);
			credentials.AddField("accessToken", app.prefs.accessToken);
		}

		data.AddField("credentials", credentials);

		matchmaker.Send("authenticate", data);
	}

	//MatchmakerDelegate

	public void HandleAuthenticate(JSONObject data) {
		account.email = data.GetField("email").str;
		account.screenName = data.GetField("screenName").str;
		account.accessToken = data.GetField("accessToken").str;
		account.SaveToPrefs();

		foreach (var obj in data.GetField("players").list) {
			var playerAccount = new Account();
			playerAccount.screenName = obj.GetField("screenName").str;
			app.account.playerList.Add(playerAccount);
		}
		TransitionTo(new MatchmakerPostAuthState());
	}

	public void HandleAuthenticateFailed(JSONObject data) {
		account.email = "";
		account.screenName = "";
		account.accessToken = "";
		account.SaveToPrefs();

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
