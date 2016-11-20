using UnityEngine;
using System.Collections;

public class MatchmakerAfterGameState : MatchmakerState {
	PostGameState postGameState {
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
		postGameState.ReceivedRematchRequest();
	}

	public override void MatchmakerDisconnected() {
		base.MatchmakerDisconnected();
		postGameState.GameCancelled();
	}

	public override void MatchmakerErrored() {
		base.MatchmakerDisconnected();
		postGameState.GameCancelled();
	}

	public override void HandleMyGameCancelled() {
		base.HandleMyGameCancelled();

		postGameState.GameCancelled();
		TransitionTo(new MatchmakerPostAuthState());
	}

	public void HandleStartGame(JSONObject data) {
		TransitionTo(new MatchmakerPlayingGameState());
	}
}
