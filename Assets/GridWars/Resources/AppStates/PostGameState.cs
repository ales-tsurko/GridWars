using UnityEngine;
using System.Collections;

public class PostGameState : AppState, AppStateOwner {
	public Player victoriousPlayer;

	// LeaveGame

	public void Leave(bool cancelGame) {
		if (cancelGame) {
			matchmaker.Send("cancelGame");
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

		Object.FindObjectOfType<CameraController>().StartOrbit();

		menu.backgroundColor = new Color(0, 0, 0, 0);

		subState = new ShowOutcomeState();
		subState.owner = this;
		subState.EnterFrom(null);
	}

	public override void WillExit() {
		base.WillExit();
		subState.WillExit();
		menu.backgroundColor = new Color(0, 0, 0, 1);
		Object.FindObjectOfType<CameraController>().EndOrbit();
		battlefield.SoftReset();
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