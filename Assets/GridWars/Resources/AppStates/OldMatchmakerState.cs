using UnityEngine;
using System.Collections;

public class OldMatchmakerState : AppState, MatchmakerDelegate {
	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		app.ResetMenu();
		menu.AddItem(UI.ActivityIndicator("SEARCHING FOR OPPONENT"));
		cancelItem = UI.MenuItem("Cancel", Cancel);
		menu.AddItem(cancelItem, true);
		menu.Show();

		matchmaker.matchmakerDelegate = this;
		if (!matchmaker.isConnected) {
			matchmaker.Connect();
		}
	}

	public void MatchmakerConnected() {
	}


	public void MatchmakerDisconnected() {
		matchmaker.Connect();
	}

	public void MatchmakerErrored() {
		matchmaker.Connect();
	}

	public void MatchmakerReceivedMessage(JSONObject message) {
	}

	public void MatchmakerReceivedHost(string gameId) {
		var s = new WaitForClientState();
		s.gameId = gameId;
		cancelItem.action = s.Cancel;
		TransitionTo(s);
	}

	public void MatchmakerReceivedJoin(string gameId) {
		var s = new WaitForServerState();
		s.gameId = gameId;
		cancelItem.action = s.Cancel;
		TransitionTo(s);
	}

	public void MatchmakerReceivedVersion(string version) {
		app.ResetMenu();
		menu.AddItem(UI.MenuItem("DOWNLOAD LATEST VERSION TO PLAY ONLINE", null, MenuItemType.ButtonTextOnly));
		menu.AddItem(UI.MenuItem("OK", Cancel));
		menu.Show();
	}

	UIButton cancelItem;

	void Cancel() {
		Disconnect();
		TransitionTo(new MainMenuState());
	}

	void Disconnect() {
		matchmaker.matchmakerDelegate = null;
		matchmaker.Disconnect();
	}
}
