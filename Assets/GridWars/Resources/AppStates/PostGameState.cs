using UnityEngine;
using System.Collections;

public class PostGameState : NetworkDelegateState, AppStateOwner {
	public Player victoriousPlayer;

	// LeaveGame

	public void LeaveGame() {
		LeaveGame("");
	}

	public void LeaveGame(string reason) {
		app.ResetMenu();
		menu.AddItem(UI.ActivityIndicator(reason + "RETURNING TO MAIN MENU"));
		menu.Show();


		if (BoltNetwork.isRunning) {
			app.battlefield.HardReset();
			network.ShutdownBolt();
		}
		else {
			BoltShutdownCompleted();
		}
	}

	// Restart

	bool didRestart;

	public void RestartGame() {
		if (didRestart) { //We have to spam bolt events because they're unreliable.  Avoid restarting twice.
			return;
		}

		didRestart = true;

		if (BoltNetwork.isServer) {
			app.StartCoroutine(RestartGameCoroutine());
		}
		else {
			TransitionTo(new PlayingGameState());
		}
	}

	IEnumerator RestartGameCoroutine() {
		battlefield.SoftReset();
		while (battlefield.livingPlayers.Count > 0) {
			yield return null;
		}

		TransitionTo(new PlayingGameState());
	}

	//AppState

	public AppState state { get; set; }

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		network.networkDelegate = this;
		didRestart = false;

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
		IsExiting();
	}

	void IsExiting() {
		Object.FindObjectOfType<CameraController>().EndOrbit();
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

	public override void ZeusDisconnected() {
		base.ZeusDisconnected();

		subState.Disconnected();
	}

	public override void BoltShutdownCompleted() {
		base.BoltShutdownCompleted();

		network.networkDelegate = null;

		TransitionTo(new MainMenuState());
	}

	public override void Disconnected(BoltConnection connection) {
		base.Disconnected(connection);

		subState.Disconnected();
	}

	public override void ReceivedRematchRequest() {
		base.ReceivedRematchRequest();

		subState.ReceivedRematchRequest();
	}

	public override void ReceivedAcceptRematch() {
		base.ReceivedAcceptRematch();

		subState.ReceivedAcceptRematch();
	}
}