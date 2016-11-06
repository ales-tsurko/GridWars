using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class WaitForBoltState : NetworkDelegateState {
	Timer timer;

	//AppState

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		if (BoltNetwork.isRunning) {
			network.ShutdownBolt();
		}
		else {
			StartBolt();
		}

		timer = app.timerCenter.NewTimer();
		timer.action = ShowStartingGame;
		timer.timeout = 0.5f;
		timer.Start();
	}

	void StartBolt() {
		BoltLauncher.StartServer();
	}

	void ShowStartingGame() {
		app.ResetMenu();
		menu.AddNewIndicator().SetText("Starting game");
		menu.Show();
	}

	// NetworkDelegate

	public override void BoltStartDone() {
		base.BoltStartDone();

		timer.Cancel();

		TransitionTo(new PlayingGameState());
	}

	//might not be shutdown yet from previous game
	public override void BoltShutdownCompleted() {
		base.BoltShutdownCompleted();

		StartBolt();
	}
}
