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

		bool flawlessVictor = postGameState.victoriousPlayer.fortress.TowersNetDamageRatio() == 1f;

		if (battlefield.localPlayers.Count == 1) {
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
			string winnerName = postGameState.victoriousPlayer.description;
			title = postGameState.victoriousPlayer.description + " is Victorious!";
			if (flawlessVictor) {
				title = postGameState.victoriousPlayer.description + " is Victorious!";
			}
		}

		menu.AddItem(UI.MenuItem(title, null, MenuItemType.ButtonTextOnly));
		if (battlefield.isInternetPVP) {
			title = "Request Rematch";
		}
		else {
			title = "Rematch!";

			if (battlefield.isPvsAI()) {
				if (postGameState.victoriousPlayer.isLocal) {
					title = "Next Level";
				}
				else if (App.shared.battlefield.GetGameType() != GameType.PvELadder) {
					title = "Continue";
				}
			}
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
            if (App.shared.battlefield.GetGameType() == GameType.PvELadder && postGameState.victoriousPlayer.isLocal) {
                App.shared.battlefield.pveLadderLevel++;
            }
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
