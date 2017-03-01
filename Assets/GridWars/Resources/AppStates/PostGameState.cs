using UnityEngine;
using System.Collections;

public class PostGameState : AppState, AppStateOwner {
	public Player victoriousPlayer;

	// LeaveGame

	public void Leave(bool cancelGame) {
		if (cancelGame && matchmaker.state is MatchmakerAfterGameState) {
			(matchmaker.state as MatchmakerAfterGameState).CancelGame();
		}

		TransitionTo(new MainMenuState());
	}

	public void Leave() {
		Leave(true);
	}

	//AppState

	public AppState state { get; set; }

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		Object.FindObjectOfType<CameraController>().GameEnded();

		subState = new ShowOutcomeState();
		subState.owner = this;
		subState.EnterFrom(null);

		if (battlefield.isPvsAI() && !battlefield.isPvELadder) {
			if (matchmaker.isConnected) {
				JSONObject data = new JSONObject();

				data.AddField("didWin", !victoriousPlayer.npcModeOn);
				data.AddField("npcLevel", battlefield.localPlayer1.opponent.npcLevel);
				matchmaker.Send("pveGameResult", data);

				victoriousPlayer.DidWin();
				victoriousPlayer.opponent.DidLose();
			}
		}
	}

	public override void WillExit() {
		base.WillExit();
		subState.WillExit();
	}

	// Network

	PostGameSubState subState {
		get {
			return (PostGameSubState)state;
		}

		set {
			state = value;
		}
	}

	/*
	public override void ZeusDisconnected() {
		base.ZeusDisconnected();

		subState.Disconnected();
	}

	public override void Disconnected(BoltConnection connection) {
		base.Disconnected(connection);

		subState.Disconnected();
	}
	*/

	public void ReceivedRematchRequest() {
		subState.ReceivedRematchRequest();
	}

	public void ReceivedAcceptRematch() {
		subState.ReceivedAcceptRematch();
	}

	public void GameCancelled() {
		subState.GameCancelled();
	}

	public void StartGame() {
		TransitionTo(new PlayingGameState());
	}
}