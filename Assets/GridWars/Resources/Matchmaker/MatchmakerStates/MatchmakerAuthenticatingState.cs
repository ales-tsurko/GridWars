using UnityEngine;
using System.Collections;

public class MatchmakerAuthenticatingState : MatchmakerState {
	// AppState

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		var data = new JSONObject();
		data.AddField("version", App.shared.version);

		var credentials = new JSONObject(JSONObject.Type.OBJECT);
		data.AddField("credentials", credentials);

		matchmaker.Send("authenticate", data);
	}

	//MatchmakerDelegate

	public override void HandleMessage(string name, JSONObject data) {
		base.HandleMessage(name, data);

		if (name == "authenticate") {
			app.account.screenName = data.GetField("screenName").str;
			app.account.accessToken = data.GetField("accessToken").str;
			TransitionTo(new MatchmakerRequestPlayerListState());
		}
		else if (name == "updateRequired") {
			TransitionTo(new MatchmakerUpdateRequiredState());
		}
		else {
			HandleUnexpectedMessage(name, data);
		}
	}

	// MatchmakerMenuDelegate

	public override void MatchmakerMenuOpened() {
		throw new System.Exception("MatchmakerMenu unexpectedly opened.");
	}

	public override void MatchmakerMenuClosed() {
		matchmaker.menu.Reset();
		matchmaker.menu.AddNewText()
			.SetText("Connecting to server ...");
		matchmaker.menu.Show();
		matchmaker.menu.Focus();
	}
}
