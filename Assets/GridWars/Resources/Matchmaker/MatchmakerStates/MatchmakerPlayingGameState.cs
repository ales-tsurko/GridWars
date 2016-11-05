using UnityEngine;
using System.Collections;

public class MatchmakerPlayingGameState : MatchmakerState {
	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		matchmaker.menu.Hide();

		battlefield.isInternetPVP = true;
		app.state.TransitionTo(new PlayingGameState());
	}

	public void HandleGameEnded(JSONObject data) {
		TransitionTo(new MatchmakerAfterGameState());

		(app.state as PlayingGameState).EndGame(data.GetField("isWinner").b ? battlefield.localPlayer1 : battlefield.localPlayer1.opponent);
	}

	public void EndGame(Player victor) {
		JSONObject data = new JSONObject();
		data.AddField("isWinner", victor == battlefield.localPlayer1);
		matchmaker.Send("endGame", data);
		TransitionTo(new MatchmakerAfterGameState());
	}
}
