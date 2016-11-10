using UnityEngine;

public class MatchmakerJoinedGameState : MatchmakerState {
	// AppState

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		matchmaker.menu.Open();
	}

	// MatchmakerMenuDelegate

	public override void ConfigureForOpen() {
		base.ConfigureForOpen();
		matchmaker.menu.Reset();
		openSoundtrackName = "Ready";

		//string text;

		if (account.isReadyForGame) {
			matchmaker.menu.AddNewIndicator().SetText("Waiting for " + account.opponent.screenName);
		}
		else {
			matchmaker.menu.AddNewText().SetText(account.opponent.screenName + " accepted your challenge.");
		}

		if (!account.isReadyForGame) {
			matchmaker.menu.AddNewButton()
				.SetText("Ready")
				.SetAction(Ready);
		}

		if (account.isReadyForGame) {
			matchmaker.menu.AddNewButton()
				.SetText("Cancel")
				.SetAction(Leave);
		}
		else {
			matchmaker.menu.AddNewButton()
				.SetText("Decline")
				.SetAction(Leave);
		}


		matchmaker.menu.Show();

		app.Log("matchmaker.menu.Show();", this);
	}

	void Ready() {
		account.isReadyForGame = true;
		matchmaker.Send("readyForGame");
		if (account.isOpponentReadyForGame) {
			WaitForPeer();
		}
		else {
			ConfigureForOpen();
		}
	}

	void Leave() {
		matchmaker.Send("cancelGame");
		TransitionTo(new MatchmakerPostAuthState());
	}

	void OpponentLeftOK() {
		TransitionTo(new MatchmakerPostAuthState());
	}

	// MatchmakerDelegate

	public void HandleGameCancelled(JSONObject data) {
		if (data.GetField("id").str == app.account.game.id) {
			matchmaker.menu.Reset();

			matchmaker.menu.AddNewText()
				.SetText("Opponent left.");

			matchmaker.menu.AddNewButton()
				.SetText("OK")
				.SetAction(OpponentLeftOK);

			matchmaker.menu.Show();
		}
	}

	public void HandleOpponentReadyForGame(JSONObject data) {
		account.isOpponentReadyForGame = true;

		if (account.isReadyForGame) {
			WaitForPeer();
		}
		else {
			ConfigureForOpen();
		}
	}

	void WaitForPeer() {
		if (account.isHost) {
			TransitionTo(new MatchmakerWaitForClientState());
		}
		else {
			TransitionTo(new MatchmakerWaitForServerState());
		}
	}
}
