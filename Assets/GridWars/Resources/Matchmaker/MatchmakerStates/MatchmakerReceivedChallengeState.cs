using UnityEngine;
using System.Collections;

public class MatchmakerReceivedChallengeState : MatchmakerState {
	public override void EnterFrom(AppState state) {
		openSoundtrackName = "Ready";

		base.EnterFrom(state);

		matchmaker.menu.Open();
	}
	// MatchmakerMenuDelegate

	public override void ConfigureForClosed() {
		base.ConfigureForClosed();

		matchmaker.menu.Reset();
		matchmaker.menu.AddNewButton().SetText(game.host.screenName + " challenged you to a match").SetAction(matchmaker.menu.Open);
		matchmaker.menu.isInteractible = true;
	}

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
