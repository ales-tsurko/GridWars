using UnityEngine;

public class MatchmakerPostedGameState : MatchmakerState {
	public Account opponent;

	// AppState

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);
		matchmaker.menu.Open();
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
		matchmaker.menu.Reset();
		matchmaker.menu.AddNewText().SetText(opponent.screenName + " Declined");
		matchmaker.menu.AddNewButton().SetText("Back").SetAction(Back).SetIsBackItem(true);
		matchmaker.menu.Focus();
	}

	void Back() {
		TransitionTo(new MatchmakerPostAuthState());
	}

	void StopSearching() {
		matchmaker.Send("cancelGame");
		TransitionTo(new MatchmakerPostAuthState());
	}
}
