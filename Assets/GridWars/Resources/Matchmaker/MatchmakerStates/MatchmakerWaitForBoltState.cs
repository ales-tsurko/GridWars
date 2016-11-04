using UnityEngine;

public class MatchmakerWaitForBoltState : MatchmakerState {
	// AppState

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		matchmaker.menu.Open();

		if (BoltNetwork.isRunning) {
			app.network.ShutdownBolt();
		}
		else {
			account.StartBoltAgent();
		}
	}

	// Possibly sent from PlayingGameState

	public void BoltShutdownCompleted() {
		account.StartBoltAgent();
	}

	// MatchmakerMenuDelegate

	public override void MatchmakerMenuOpened() {
		base.MatchmakerMenuOpened();

		matchmaker.menu.Reset();

		matchmaker.menu.AddNewIndicator().SetText("Connecting to opponent.");

		matchmaker.menu.AddNewButton()
			.SetText("Leave")
			.SetAction(Leave);

		matchmaker.menu.Show();
	}

	void Leave() {
		matchmaker.Send("cancelGame");
		account.boltAgent.Shutdown();
		account.boltAgent.boltAgentDelegate = null;
		TransitionTo(new MatchmakerPostAuthState());
	}

	void OpponentLeftOK() {
		TransitionTo(new MatchmakerPostAuthState());
	}

	// MatchmakerDelegate

	public void HandleGameCancelled(JSONObject data) {
		if (data.GetField("id").str == app.account.game.id) {

			account.boltAgent.Shutdown();
			account.boltAgent.boltAgentDelegate = null;

			matchmaker.menu.Reset();
			matchmaker.menu.AddNewText()
				.SetText("Opponent left.");
			matchmaker.menu.AddNewButton()
				.SetText("OK")
				.SetAction(OpponentLeftOK);
			matchmaker.menu.Show();
		}
	}

	public void HandleStartGame(JSONObject data) {
		TransitionTo(new MatchmakerPlayingGameState());
	}
}
