using UnityEngine;

public class MatchmakerJoinedGameState : MatchmakerState {
	// AppState

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		matchmaker.menu.Open();
	}

	// MatchmakerMenuDelegate

	public override void MatchmakerMenuOpened() {
		base.MatchmakerMenuOpened();
		matchmaker.menu.Reset();

		string text;

		if (account.isOpponentReadyForGame) {
			text = "Opponent is Ready";
		}
		else {
			text = "Waiting for Opponent";
		}

		matchmaker.menu.AddNewText().SetText(text);

		if (!account.isReadyForGame) {
			matchmaker.menu.AddNewButton()
				.SetText("Ready")
				.SetAction(Ready);
		}

		matchmaker.menu.AddNewButton()
			.SetText("Leave")
			.SetAction(Leave);

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
			MatchmakerMenuOpened();
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
			MatchmakerMenuOpened();
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
