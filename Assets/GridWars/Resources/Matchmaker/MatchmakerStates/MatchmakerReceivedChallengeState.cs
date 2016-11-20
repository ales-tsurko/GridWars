using UnityEngine;
using System.Collections;

public class MatchmakerReceivedChallengeState : MatchmakerState {
	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		matchmaker.menu.Open();
	}
	// MatchmakerMenuDelegate

	public override void ConfigureForOpen() {
		base.ConfigureForOpen();

		matchmaker.menu.Reset();
		matchmaker.menu.AddNewIndicator().SetText(game.host.screenName + " challenged you to a match");
		matchmaker.menu.AddNewButton().SetText("Accept").SetAction(Accept);
		matchmaker.menu.AddNewButton().SetText("Decline").SetAction(Decline).SetIsBackItem(true);
	}

	void Accept() {
		matchmaker.Send("joinGame", game.publicPropertyData);
		TransitionTo(new MatchmakerJoinedGameState());
	}

	void Decline() {
		matchmaker.Send("cancelGame", game.publicPropertyData);
		account.game = null;
		TransitionTo(new MatchmakerPostAuthState());
	}

	public override void HandleMyGameCancelled() {
		base.HandleMyGameCancelled();

		matchmaker.menu.Reset();
		matchmaker.menu.AddNewIndicator().SetText(game.host.screenName + " Left");
		matchmaker.menu.AddNewButton().SetText("Back").SetIsBackItem(true).SetAction(Back);
		matchmaker.menu.Focus();
	}

	void Back() {
		TransitionTo(new MatchmakerPostAuthState());
	}
}
