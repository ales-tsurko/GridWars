﻿using UnityEngine;
using System.Collections;

public class MatchmakerPlayingGameState : MatchmakerState {
	PlayingGameState playingGameState {
		get {
			return app.state as PlayingGameState;
		}
	}

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		matchmaker.menu.Close();

		matchmaker.messenger.Setup();

		battlefield.isInternetPVP = true;
		battlefield.isAiVsAi = false;
		app.state.TransitionTo(new PlayingGameState());
	}

	public override void WillExit() {
		base.WillExit();

		if (!(matchmaker.state is MatchmakerAfterGameState)) {
			matchmaker.messenger.TearDown();
		}
	}

	public void HandleGameEnded(JSONObject data) {
		playingGameState.GameEnded(data.GetField("isWinner").b ? battlefield.localPlayer1 : battlefield.localPlayer1.opponent);

		TransitionTo(new MatchmakerAfterGameState());
	}

	public override void HandleMyGameCancelled() {
		base.HandleMyGameCancelled();

		playingGameState.GameCancelled();
		TransitionTo(new MatchmakerPostAuthState());
	}

	public override void MatchmakerDisconnected() {
		base.MatchmakerDisconnected();
		playingGameState.GameCancelled();
	}

	public override void MatchmakerErrored() {
		base.MatchmakerDisconnected();
		playingGameState.GameCancelled();
	}

	public void EndGame(Player victor) {
		JSONObject data = new JSONObject();
		data.AddField("isWinner", victor == battlefield.localPlayer1);
		matchmaker.Send("endGame", data);
		TransitionTo(new MatchmakerAfterGameState());
	}
}
