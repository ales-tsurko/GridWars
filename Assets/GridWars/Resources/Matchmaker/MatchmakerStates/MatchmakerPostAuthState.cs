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

	Account previousPotentialOpponent;
	Game previousPotentialOpponentGame;

	void PlayOpponentSounds() {
		if (account.potentialOpponent != null) {
			if (account.potentialOpponent.game != null) {
				if (previousPotentialOpponent != null) {
				}
				if (previousPotentialOpponent == null || previousPotentialOpponentGame == null) {
					App.shared.PlayAppSoundNamedAtVolume("PlayPVP", 1f);
				}
			}
			else if (previousPotentialOpponent == null) {
				App.shared.PlayAppSoundNamedAtVolume("PlayerConnected", 1f);
			}
		}
	}

	public override void HandlePlayerConnected(JSONObject data) {
		previousPotentialOpponent = account.potentialOpponent;
		previousPotentialOpponentGame = previousPotentialOpponent != null ? previousPotentialOpponent.game : null;
		base.HandlePlayerConnected(data);
		UpdateMenu();
		PlayOpponentSounds();
	}

	public override void HandlePlayerDisconnected(JSONObject data) {
		base.HandlePlayerDisconnected(data);
		UpdateMenu();
	}

	public override void HandleGamePosted(JSONObject data) {
		previousPotentialOpponent = account.potentialOpponent;
		previousPotentialOpponentGame = previousPotentialOpponent != null ? previousPotentialOpponent.game : null;

		base.HandleGamePosted(data);

		var game = account.GameWithId(data.GetField("id").str);

		if (game.client == account) {
			account.game = game;
			TransitionTo(new MatchmakerReceivedChallengeState());
		}
		else {
			UpdateMenu();
			PlayOpponentSounds();
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
