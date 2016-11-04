using UnityEngine;
using System.Collections;

public class ShowOutcomeState : PostGameSubState {
	// AppState
	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		app.ResetMenu();
		menu.SetBackground(Color.black, 0);
		var title = "";
        if (postGameState.victoriousPlayer == null) {
            postGameState.LeaveGame();
            return;
        }
		if (battlefield.localPlayers.Count == 1) {
			if (postGameState.victoriousPlayer.isLocal) {
				title = "Victory!";
				app.PlayAppSoundNamed("Victory");
			} else {
				title = "Defeat!";
				app.PlayAppSoundNamedAtVolume("Defeat", 0.5f);
			}
		} else {
			title = postGameState.victoriousPlayer.description + " is Victorious!";
		}

		menu.AddItem(UI.MenuItem(title, null, MenuItemType.ButtonTextOnly));
		if (battlefield.isInternetPVP) {
			title = "Request Rematch";
		} else {
			title = "Rematch!";
		}

		menu.AddItem(UI.MenuItem(title, RequestRematch));
		menu.AddItem(UI.MenuItem("Leave Game", postGameState.LeaveGame));

		menu.Show();
	}

	void RequestRematch() {
		if (battlefield.isInternetPVP) {
			TransitionTo(new AwaitRematchResponseState());
		}
		else {
			postGameState.RestartGame();
		}
	}

	//PostGameSubState

	public override void ReceivedRematchRequest() {
		base.ReceivedRematchRequest();

		TransitionTo(new ReceivedRematchRequestState());
	}

	public override void Disconnected() {
		base.Disconnected();

		postGameState.LeaveGame();
	}

}
