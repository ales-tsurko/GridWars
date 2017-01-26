using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;

public class MultiPlayerMenuState : AppState {
	UIButton internetPvpButton;

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		app.notificationCenter.NewObservation()
			.SetNotificationName(Matchmaker.MatchmakerConnectedNotification)
			.SetAction(MatchmakerConnected)
			.SetSender(app.matchmaker);

		app.notificationCenter.NewObservation()
			.SetNotificationName(Matchmaker.MatchmakerDisconnectedNotification)
			.SetAction(MatchmakerDisconnected)
			.SetSender(app.matchmaker);

		app.notificationCenter.NewObservation()
			.SetNotificationName(Matchmaker.MatchmakerErroredNotification)
			.SetAction(MatchmakerDisconnected)
			.SetSender(app.matchmaker);

		ShowMenu();
	}

	public override void WillExit() {
		base.WillExit();

		app.notificationCenter.RemoveObserver(this);
	}

	void ShowMenu() {
		menu.Reset();
		internetPvpButton = menu.AddNewButton().SetText("Internet").SetAction(InternetPvpClicked);
		menu.AddItem(UI.MenuItem("Shared Screen", SharedScreenPvpClicked));
		menu.AddItem(UI.MenuItem("View Ladder", LadderClicked));
		menu.AddItem(UI.MenuItem("Back", Back).SetIsBackItem(true));
		menu.Show();
	}

	void InternetPvpClicked() {
		/*Analytics.CustomEvent("InternetPvpClicked", new Dictionary<string, object> {
			{ "playTime", Time.timeSinceLevelLoad }
		});*/

		if (matchmaker.isConnected) {
			matchmaker.matchmakerState.PostGame();
		}
		else {
			menu.Reset();
			menu.AddNewText()
				.SetText("Unable to connect to the server.\n\nInternet matches disabled.");
			menu.AddNewButton()
				.SetText("Close")
				.SetAction(ShowMenu)
				.SetIsBackItem(true);
			menu.Show();
		}
	}

	void LadderClicked() {
		TransitionTo(new LadderState());
	}

	public void MatchmakerConnected(Notification n) {
		if (internetPvpButton != null) {
			internetPvpButton.UseDefaultStyle();
			if (menu.hasFocus) {
				menu.soundsEnabled = false;
				menu.Focus(); //reselect to update style
				menu.soundsEnabled = true;
			}
		}
	}

	public void MatchmakerDisconnected(Notification n) {
		if (internetPvpButton != null) {
			internetPvpButton.UseAlertStyle();
			if (menu.hasFocus) {
				menu.soundsEnabled = false;
				menu.Focus(); //reselect to update style
				menu.soundsEnabled = true;
			}
		}
	}

	void SharedScreenPvpClicked() {
		battlefield.player1.isLocal = true;
		battlefield.player2.isLocal = true;

		battlefield.player2.npcModeOn = false;
		battlefield.player2.npcModeOn = false;

		battlefield.player1.isTutorialMode = false;
		battlefield.player2.isTutorialMode = false;

		/*Analytics.CustomEvent("SharedScreenPvPClicked", new Dictionary<string, object>
			{
				{ "playTime", Time.timeSinceLevelLoad }
			});*/
		TransitionTo(new BindInputsToPlayersState());
	}

	void Back() {
		TransitionBack();
	}
}
