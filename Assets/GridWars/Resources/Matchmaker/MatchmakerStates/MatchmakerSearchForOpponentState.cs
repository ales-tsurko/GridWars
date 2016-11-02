using UnityEngine;

public class MatchmakerSearchForOpponentState : MatchmakerState {
	// AppState

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		matchmaker.Send("postChallenge");
	}

	// MatchmakerMenuDelegate

	public override void MatchmakerMenuClosed() {
		matchmaker.menu.Reset();
		matchmaker.menu.AddNewButton()
			.SetText("Searching for Game ...")
			.SetAction(matchmaker.menu.Open);
		matchmaker.menu.Show();
		matchmaker.menu.Focus();
	}

	public override void MatchmakerMenuOpened() {
		matchmaker.menu.Reset();

		matchmaker.menu.AddNewButton()
			.SetText("Stop Searching")
			.SetAction(StopSearching)
			.SetIsBackItem(true);
		
		matchmaker.menu.AddNewButton()
			.SetText("Close")
			.SetAction(matchmaker.menu.Close)
			.SetIsBackItem(true);

		matchmaker.menu.Show();
	}

	// MatchmakerDelegate

	public override void HandleMessage(string name, JSONObject data) {
		base.HandleMessage(name, data);

		if (name == "playerAcceptedChallenge") {
			TransitionTo(new MatchmakerPreGameState());
		}
	}

	void StopSearching() {
		matchmaker.Send("cancelChallenge");
		matchmaker.menu.Close();
		TransitionTo(new MatchmakerPlayerListState());
	}
}
