﻿using UnityEngine;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class MatchmakerPostedGameState : MatchmakerState {
	public Account opponent;

	// AppState

	public override void EnterFrom(AppState state) {
        base.EnterFrom(state);
        matchmaker.menu.Open();
        app.account.LogEvent("PostedGame");
    }

	// MatchmakerMenuDelegate

	string text {
		get {
			if (opponent == null) {
				return "Searching for opponent";
			}
			else {
				return "Waiting for " + opponent.screenName;
			}
		}
	}

	public override void ConfigureForClosed() {
		base.ConfigureForClosed();

		matchmaker.menu.Reset();
		UIButton b = matchmaker.menu.AddNewButton()
			.SetText(text)
			.SetAction(matchmaker.menu.Open);
		b.doesType = true;
		matchmaker.menu.isInteractible = true;
	}

	public override void ConfigureForOpen() {
		base.ConfigureForOpen();

		matchmaker.menu.Reset();

		var indicator = matchmaker.menu.AddNewIndicator();
		indicator.text = text;

		matchmaker.menu.AddNewButton()
			.SetText("Cancel Challenge")
			.SetAction(StopSearching);
		
		var back = matchmaker.menu.AddNewButton()
			.SetText("Back")
			.SetAction(matchmaker.menu.Close)
			.SetIsBackItem(true);

		matchmaker.menu.SelectItem(back);
	}

	// MatchmakerDelegate

	public void HandleOpponentJoinedGame(JSONObject data) {
		account.game = new Game();
		account.game.id = data.GetField("id").str;
		account.game.host = app.account.AccountWithId(data.GetField("host").GetField("id").n);
		account.game.client = app.account.AccountWithId(data.GetField("client").GetField("id").n);

		TransitionTo(new MatchmakerJoinedGameState());
	}

	public override void HandleMyGameCancelled() {
		base.HandleMyGameCancelled();
		matchmaker.menu.Open();
		matchmaker.menu.Reset();
		matchmaker.menu.AddNewText().SetText(opponent.screenName + " Declined");
		matchmaker.menu.AddNewButton().SetText("Back").SetAction(Back).SetIsBackItem(true);
		matchmaker.menu.Focus();
	}

	public void HandlePostGameFailed(JSONObject data) {
		matchmaker.menu.Open();
		matchmaker.menu.Reset();
		matchmaker.menu.AddNewText().SetText(Color.red.ColoredTag("Error: " + data.GetField("error").str));
		matchmaker.menu.AddNewButton().SetText("Back").SetAction(Back).SetIsBackItem(true);
		matchmaker.menu.Focus();
	}

	//possible when both players challenge each other at the same time (race condition)
	public override void HandleGamePosted(JSONObject data) {
		base.HandleGamePosted(data);

		var game = account.GameWithId(data.GetField("id").str);

		if (game.client == account) {
			account.game = game;
			TransitionTo(new MatchmakerReceivedChallengeState());
		}
	}

	void Back() {
		TransitionTo(new MatchmakerPostAuthState());
	}

	void StopSearching() {
		matchmaker.Send("cancelGame");
		TransitionTo(new MatchmakerPostAuthState());
	}
}
