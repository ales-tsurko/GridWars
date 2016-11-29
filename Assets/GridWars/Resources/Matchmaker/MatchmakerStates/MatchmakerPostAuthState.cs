using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class MatchmakerPostAuthState : MatchmakerState {
	UIButton button;

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		if (matchmaker.messenger.isEnabled) {
			matchmaker.messenger.TearDown();
		}

		matchmaker.menu.Close();
		matchmaker.menu.Show();
	}

	public override void WillExit() {
		base.WillExit();

		CancelStatusTimer();
	}

	// MatchmakerMenuDelegate

	public override void ConfigureForClosed() {
		base.ConfigureForClosed();

		matchmaker.menu.Reset();
		button = matchmaker.menu.AddNewButton();
		UpdateStatus();
	}

	void UpdateStatus() {
		if (matchmaker.menu.isOpen) {
			return;
		}

		if (account.connectedAccounts.Count == 0) {
			button.text = "Online";
			button.action = null;
			matchmaker.menu.isInteractible = false;
			app.state.DisconnectMatchmakerMenu();
		}
		else {
			button.text = account.connectedAccounts.Count + " Online";

			if (account.connectedAccounts.Exists(a => a.status == AccountStatus.Searching)) {
				button.text += ": " + Color.yellow.ColoredTag("1 Posted Challenge");
			}

			MakeMenuOpenable();
		}
	}

	void MakeMenuOpenable() {
		button.action = matchmaker.menu.Open;
		matchmaker.menu.isInteractible = true;
		app.state.ConnectMatchmakerMenu();
	}

	public override void ConfigureForOpen() {
		base.ConfigureForOpen();

		UpdateOpenMenu();
	}

	void UpdateOpenMenu() {
		matchmaker.menu.Reset();

		CancelStatusTimer();

		app.account.connectedAccounts.ForEach((account) => {
			switch (account.status) {
			case AccountStatus.Available:
				matchmaker.menu.AddNewButton()
					.SetText(account.screenName + ": Online: " + Color.yellow.ColoredTag("Send Challenge"))
					.SetAction(() => { PostGameWithOpponent(account); });
				break;
			case AccountStatus.Searching:
				matchmaker.menu.AddNewButton()
					.SetText(account.screenName + ": Posted Challenge: " + Color.yellow.ColoredTag("Accept Challenge"))
					.SetAction(() => { PostGameWithOpponent(account); });
				break;
			case AccountStatus.Playing:
				matchmaker.menu.AddNewText()
					.SetText(account.screenName + ": Playing " + account.opponent.screenName);
				break;
			case AccountStatus.Unavailable:
				matchmaker.menu.AddNewText()
					.SetText(account.screenName + ": Busy");
				break;
			}
		});

		if (app.account.connectedAccounts.Count == 0) {
			matchmaker.menu.AddNewText().SetText("No one else is online");
		}

		matchmaker.menu.AddNewButton().SetText("Back").SetAction(matchmaker.menu.Close).SetIsBackItem(true);

		matchmaker.menu.Focus();
	}

	void PostGameWithOpponent(Account opponent) {
		var data = new JSONObject();
		data.AddField("opponent", opponent.publicPropertyData);
		matchmaker.Send("postGame", data);
		var state = new MatchmakerPostedGameState();
		state.opponent = opponent;
		TransitionTo(state);
	}

	Timer statusTimer;

	void StartStatusTimer() {
		CancelStatusTimer();

		statusTimer = App.shared.timerCenter.NewTimer();
		statusTimer.action = TimerUpdateStatus;
		statusTimer.timeout = 5;
		statusTimer.Start();
	}

	void TimerUpdateStatus() {
		CancelStatusTimer();
		UpdateStatus();
	}

	void CancelStatusTimer() {
		if (statusTimer != null) {
			statusTimer.Cancel();
			statusTimer = null;
		}
	}

	public override void HandlePlayerConnected(JSONObject data) {
		base.HandlePlayerConnected(data);

		if (matchmaker.menu.isOpen) {
			UpdateOpenMenu();
		}
		else {
			App.shared.PlayAppSoundNamedAtVolume("PlayerConnected", 1f);
			button.text = data.GetField("screenName").str + " is online";
			MakeMenuOpenable();
			StartStatusTimer();
		}
	}

	void UpdateMenu() {
		if (matchmaker.menu.isOpen) {
			UpdateOpenMenu();
		}
		else if (statusTimer == null) {
			UpdateStatus();
		}
	}

	public override void HandlePlayerDisconnected(JSONObject data) {
		base.HandlePlayerDisconnected(data);
		UpdateMenu();
	}

	public override void HandleGamePosted(JSONObject data) {
		base.HandleGamePosted(data);

		var game = account.GameWithId(data.GetField("id").str);

		if (game.client == account) {
			account.game = game;
			TransitionTo(new MatchmakerReceivedChallengeState());
		}
		else {
			if (matchmaker.menu.isOpen) {
				UpdateOpenMenu();
			}
			else {
				App.shared.PlayAppSoundNamedAtVolume("PlayPVP", 1f);
				button.text = Color.yellow.ColoredTag(game.host.screenName + " posted a challenge");
				MakeMenuOpenable();
				StartStatusTimer();
			}
		}
	}

	public override void HandleGameCancelled(JSONObject data) {
		base.HandleGameCancelled(data);
		UpdateMenu();
	}

	public override void HandlePlayerBecameAvailableToPlay(JSONObject data) {
		base.HandlePlayerBecameAvailableToPlay(data);
		UpdateMenu();
	}

	public override void HandlePlayerBecameUnavailableToPlay(JSONObject data) {
		base.HandlePlayerBecameUnavailableToPlay(data);
		UpdateMenu();
	}

	public override void HandlePlayerChangedScreenName(JSONObject data) {
		base.HandlePlayerChangedScreenName(data);
		UpdateMenu();
	}
}
