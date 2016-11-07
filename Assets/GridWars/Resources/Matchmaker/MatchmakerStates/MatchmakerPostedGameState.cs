using UnityEngine;

public class MatchmakerPostedGameState : MatchmakerState {
	// AppState

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		matchmaker.Send("postGame");
	}

	// MatchmakerMenuDelegate

	public override void MatchmakerMenuClosed() {
		base.MatchmakerMenuClosed();

		matchmaker.menu.Reset();
		matchmaker.menu.AddNewButton()
			.SetText("Searching for Opponent")
			.SetAction(matchmaker.menu.Open);
		matchmaker.menu.Show();
		matchmaker.menu.Focus();
	}

	public override void MatchmakerMenuOpened() {
		base.MatchmakerMenuOpened();

		matchmaker.menu.Reset();

		matchmaker.menu.AddNewIndicator()
			.SetText("Searching for Opponent.  ");

		matchmaker.menu.AddNewButton()
			.SetText("Cancel Search")
			.SetAction(StopSearching);
		
		matchmaker.menu.AddNewButton()
			.SetText("Back")
			.SetAction(matchmaker.menu.Close)
			.SetIsBackItem(true);

		matchmaker.menu.Show();
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
