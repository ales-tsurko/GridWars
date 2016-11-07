using UnityEngine;
using System.Collections;

public class MatchmakerAfterGameState : MatchmakerState {
	PostGameState postGateState {
		get {
			return app.state as PostGameState;
		}
	}

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		matchmaker.menu.Hide();
	}

	public void CancelGame() {
		matchmaker.Send("cancelGame");
		TransitionTo(new MatchmakerPostAuthState());
	}

	public void HandleOpponentRequestedRematch(JSONObject data) {
		postGateState.ReceivedRematchRequest();
	}

	public override void MatchmakerDisconnected() {
		base.MatchmakerDisconnected();
		postGateState.GameCancelled();
	}

	public void HandleGameCancelled(JSONObject data) {
		if (data.GetField("id").str == app.account.game.id) {
			postGateState.GameCancelled();
			TransitionTo(new MatchmakerPostAuthState());
		}
	}

	public void HandleStartGame(JSONObject data) {
		TransitionTo(new MatchmakerPlayingGameState());
	}
}
