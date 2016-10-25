using UnityEngine;
using System.Collections;

public class ReceivedRematchRequestState : PostGameSubState {
	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		app.ResetMenu();
		menu.AddItem(UI.ActivityIndicator("Opponent Requests a Rematch"));
		menu.AddItem(UI.MenuItem("Accept", AcceptRematch));
		menu.AddItem(UI.MenuItem("Decline", postGameState.LeaveGame));
		menu.Show();
	}

	void AcceptRematch() {
		attempts = 0;
		SendAcceptEvent();
		postGameState.RestartGame();
	}

	int attempts;

	void SendAcceptEvent() {
		if (attempts < 10) { //Spam this because Bolt is terrible.
			AcceptRematchEvent.Create(Bolt.GlobalTargets.Others, Bolt.ReliabilityModes.ReliableOrdered).Send();
			app.Log("AcceptRematchEvent.Send", this);

			attempts ++;

			var t = app.timerCenter.NewTimer();
			t.action = SendAcceptEvent;
			t.timeout = 0.05f;
			t.Send();
		}
	}
}
