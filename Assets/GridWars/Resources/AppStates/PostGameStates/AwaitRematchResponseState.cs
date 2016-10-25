using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class AwaitRematchResponseState : PostGameSubState {
	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		SendRematchEvent();
		//App.shared.PlayAppSoundNamedAtVolume("Rematch", 0.3f); // want to play this until menu is removed

		app.ResetMenu();
		menu.AddItem(UI.ActivityIndicator("WAITING FOR RESPONSE"));
		menu.AddItem(UI.MenuItem("Cancel", postGameState.LeaveGame), true);
		menu.Show();
	}

	public override void ReceivedRematchRequest() {
		base.ReceivedRematchRequest();

		postGameState.RestartGame();
	}

	bool receivedAcceptRematch;

	public override void ReceivedAcceptRematch() {
		base.ReceivedAcceptRematch();

		postGameState.RestartGame();
	}

	public override void Disconnected() {
		base.Disconnected();

		postGameState.LeaveGame("Opponent Declined. ");
	}

	public override void WillExit() {
		base.WillExit();

		CancelTimer();
	}

	Timer sendRematchTimer;

	void SendRematchEvent() {
		RequestRematchEvent.Create(Bolt.GlobalTargets.Others, Bolt.ReliabilityModes.ReliableOrdered).Send();
		app.Log("RequestRematchEvent.Send", this);

		sendRematchTimer = app.timerCenter.NewTimer();
		sendRematchTimer.action = SendRematchEvent;
		sendRematchTimer.timeout = 0.5f;
		sendRematchTimer.Start();
	}

	void CancelTimer() {
		if (sendRematchTimer != null) {
			sendRematchTimer.Cancel();
			sendRematchTimer = null;
		}
	}
}
