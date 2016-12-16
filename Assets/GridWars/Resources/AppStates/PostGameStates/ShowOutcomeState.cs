using UnityEngine;
using System.Collections;

public class ShowOutcomeState : PostGameSubState {
	// AppState
	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		ShowMenu(true);

		ConnectMatchmakerMenu();
	}

	void ShowMenu(bool showRematch) {
		app.ResetMenu();
		var title = "";
        if (postGameState.victoriousPlayer == null) {
            postGameState.Leave();
            return;
        }
		if (battlefield.localPlayers.Count == 1) {
			bool flawlessVictor = postGameState.victoriousPlayer.fortress.TowersNetDamageRatio() == 1f;
			if (postGameState.victoriousPlayer.isLocal) {
				title = "Victory!";
				if (flawlessVictor) {
					title = "Flawless Victory!";
				}
				if (showRematch) {
					app.PlayAppSoundNamed("Victory");
				}
			} else {
				title = "Defeat!";
				if (flawlessVictor) {
					title = "Crushing Defeat!";
				}

				if (showRematch) {
					app.PlayAppSoundNamedAtVolume("Defeat", 0.5f);
				}	
			}
		} else {
			if (showRematch) {
				app.PlayAppSoundNamed("Victory");
			}
			title = postGameState.victoriousPlayer.description + " is Victorious!";
		}

		menu.AddItem(UI.MenuItem(title, null, MenuItemType.ButtonTextOnly));
		if (battlefield.isInternetPVP) {
			title = "Request Rematch";
		} else {
			title = "Rematch!";
		}

		if (showRematch) {
			menu.AddItem(UI.MenuItem(title, RequestRematch));
		}

		menu.AddItem(UI.MenuItem("Leave Game", postGameState.Leave));

		menu.selectsOnShow = !matchmaker.menu.hasFocus; //user might be typing a message.
		menu.Show();
		menu.selectsOnShow = true;
	}

	void RequestRematch() {
		if (battlefield.isInternetPVP) {
			matchmaker.Send("requestRematch");
			TransitionTo(new AwaitRematchResponseState());
		}
		else {
			postGameState.StartGame();
		}
	}

	//PostGameSubState

	public override void ReceivedRematchRequest() {
		base.ReceivedRematchRequest();

		TransitionTo(new ReceivedRematchRequestState());
	}

	public override void GameCancelled() {
		//base.GameCancelled(); don't use default behavior

		ShowMenu(false);
	}

}
