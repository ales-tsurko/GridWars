using UnityEngine;

public class MatchmakerJoinedGameState : MatchmakerState {
	// AppState

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);
		openSoundtrackName = "Ready";

		app.state.DisconnectMatchmakerMenu();
		matchmaker.menu.Open();
	}

	// MatchmakerMenuDelegate

	public override void ConfigureForOpen() {
		base.ConfigureForOpen();

		matchmaker.menu.Reset();

		//string text;

		if (account.isReadyForGame) {
			matchmaker.menu.AddNewIndicator().SetText("Waiting for " + account.opponent.screenName);
		}
		else {
			string text = "";
			if (account.isHost) {
				//local player initiated
				text += Color.yellow.ColoredTag("challenge accepted") + "\n\n";
			}

			text += account.screenName + " vs " + account.opponent.screenName;

			matchmaker.menu.AddNewText().SetText(text);
		}

		if (!account.isReadyForGame) {
			matchmaker.menu.AddNewButton()
				.SetText("Ready")
				.SetAction(Ready);
		}

		matchmaker.menu.AddNewButton()
			.SetText("Cancel")
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

	public override void HandleMyGameCancelled() {
		base.HandleMyGameCancelled();

		matchmaker.menu.Reset();

		matchmaker.menu.AddNewText().SetText(account.opponent.screenName + " left");

		matchmaker.menu.AddNewButton()
			.SetText("Back")
			.SetAction(OpponentLeftOK)
			.SetIsBackItem(true);

		matchmaker.menu.Show();
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
