using UnityEngine;
using System.Collections;

public class MatchmakerPlayingGameState : MatchmakerState {
	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		matchmaker.menu.Hide();

		app.state.TransitionTo(new PlayingGameState());
	}

	void HandleGameEnded(JSONObject data) {
		(app.state as PlayingGameState).EndGame(data.GetField("isWinner") ? battlefield.localPlayer1 : battlefield.localPlayer2);
	}
}
