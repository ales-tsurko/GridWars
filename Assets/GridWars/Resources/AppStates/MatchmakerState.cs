using UnityEngine;
using System.Collections;

public class MatchmakerState : AppState, MatchmakerDelegate {
	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		app.ResetMenu();
		menu.AddItem(UI.ActivityIndicator("SEARCHING FOR OPPONENT"));
		cancelItem = UI.MenuItem("Cancel", Cancel);
		menu.AddItem(cancelItem);
		menu.Show();

		matchmaker.matchmakerDelegate = this;
		if (!matchmaker.isConnected) {
			matchmaker.Start();
		}
	}

	public void MatchmakerDisconnected() {
		matchmaker.Start();
	}

	public void MatchmakerErrored() {
		matchmaker.Start();
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
