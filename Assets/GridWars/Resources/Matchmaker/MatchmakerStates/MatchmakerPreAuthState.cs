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

		matchmaker.menu.Close();
	}

	//MatchmakerDelegate

	public void HandleAuthenticate(JSONObject data) {
		account.SetFromData(data);
		account.SaveToPrefs();

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
}
