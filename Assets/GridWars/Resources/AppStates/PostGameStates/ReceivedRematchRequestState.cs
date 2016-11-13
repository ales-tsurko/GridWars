using UnityEngine;
using System.Collections;

public class ReceivedRematchRequestState : PostGameSubState {
	public override void EnterFrom(AppState state) {
		openSoundtrackName = "Rematch";
		base.EnterFrom(state);

		app.ResetMenu();
		menu.AddItem(UI.ActivityIndicator("Opponent Requests a Rematch"));
		menu.AddItem(UI.MenuItem("Accept", AcceptRematch));
		menu.AddItem(UI.MenuItem("Decline", postGameState.Leave));
		menu.Show();
	}

	void RejectRematch() {
		postGameState.Leave();
	}

	void AcceptRematch() {
		matchmaker.Send("requestRematch");

		app.ResetMenu();
		menu.AddNewIndicator().SetText("Starting Game");
		menu.AddNewButton().SetText("Leave").SetAction(postGameState.Leave);
		menu.Show();
	}
}
