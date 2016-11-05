using UnityEngine;
using System.Collections;

public class MatchmakerAfterGameState : MatchmakerState {
	PostGameState postGateState {
		get {
			return app.state as PostGameState;
		}
	}

	public void HandleOpponentRequestedRematch(JSONObject data) {
		postGateState.ReceivedRematchRequest();
	}

	public void HandleGameCancelled(JSONObject data) {
		if (data.GetField("id").str == app.account.game.id) {
			postGateState.GameCancelled();
		}
	}

	public void HandleStartGame(JSONObject data) {
		TransitionTo(new MatchmakerPlayingGameState());
	}
}
