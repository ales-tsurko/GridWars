﻿using UnityEngine;

public class MatchmakerPostedGameState : MatchmakerState {
	// AppState

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		matchmaker.Send("postGame");
	}

	// MatchmakerMenuDelegate

	public override void ConfigureForClosed() {
		base.ConfigureForClosed();

		matchmaker.menu.Reset();
		UIButton b = matchmaker.menu.AddNewButton()
			.SetText("Searching for Opponent")
			.SetAction(matchmaker.menu.Open);
		b.doesType = true;
	}

	public override void ConfigureForOpen() {
		base.ConfigureForOpen();

		matchmaker.menu.Reset();

		matchmaker.menu.AddNewText()
			.SetText("We'll notify you when we've found an opponent.");

		matchmaker.menu.AddNewButton()
			.SetText("Cancel Search")
			.SetAction(StopSearching);
		
		matchmaker.menu.AddNewButton()
			.SetText("Back")
			.SetAction(matchmaker.menu.Close)
			.SetIsBackItem(true);
	}

	// MatchmakerDelegate

	public void HandleOpponentJoinedGame(JSONObject data) {
		account.game = new Game();
		account.game.id = data.GetField("id").str;
		account.game.host = app.account.AccountNamed(data.GetField("host").GetField("screenName").str);
		account.game.client = app.account.AccountNamed(data.GetField("client").GetField("screenName").str);

		TransitionTo(new MatchmakerJoinedGameState());
	}

	void StopSearching() {
		matchmaker.Send("cancelGame");
		matchmaker.menu.Close();
		TransitionTo(new MatchmakerPostAuthState());
	}
}
