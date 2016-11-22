using UnityEngine;
using System.Collections;

public class MatchmakerPostAuthState : MatchmakerState {
	UIButton button;

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		matchmaker.menu.Close();
		matchmaker.menu.Show();
	}
	// MatchmakerMenuDelegate

	public override void ConfigureForClosed() {
		base.ConfigureForClosed();
		matchmaker.menu.Reset();
		button = matchmaker.menu.AddNewButton();
		UpdateMenu();
	}

	void UpdateMenu() {
		matchmaker.menu.isInteractible = false;
		app.state.DisconnectMatchmakerMenu();
		button.action = null;
		button.data = null;

		if (account.connectedAccounts.Count == 0) {
			button.text = "Online";
		}
		else {
			var potentialOpponent = account.potentialOpponent;
			if (potentialOpponent == null) {
				button.text = account.connectedAccounts.Count + " Online";
			}
			else {
				matchmaker.menu.isInteractible = true;
				app.state.ConnectMatchmakerMenu();
				if (potentialOpponent.game == null) {
					button.text = potentialOpponent.screenName + " is online: " + Color.yellow.ColoredTag("Challenge " + potentialOpponent.screenName);
				}
				else {
					button.text = potentialOpponent.screenName + " posted a challenge: " + Color.yellow.ColoredTag("Accept Challenge");
				}
				button.action = PostGameWithOpponent;
				button.data = potentialOpponent;
			}
		}
	}

	void PostGameWithOpponent() {
		var opponent = button.data as Account;
		var data = new JSONObject();
		data.AddField("opponent", opponent.publicPropertyData);
		matchmaker.Send("postGame", data);
		var state = new MatchmakerPostedGameState();
		state.opponent = opponent;
		TransitionTo(state);
	}

	public override void HandlePlayerConnected(JSONObject data) {
		base.HandlePlayerConnected(data);
		UpdateMenu();
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
			UpdateMenu();
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
