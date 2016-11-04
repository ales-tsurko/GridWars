using UnityEngine;
using System.Collections;

public class MatchmakerPlayingGameState : MatchmakerState {
	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		matchmaker.menu.Open();
	}

	public override void MatchmakerMenuOpened() {
		base.MatchmakerMenuOpened();

		matchmaker.menu.Reset();

		matchmaker.menu.AddNewIndicator().SetText("Playing Game");

		matchmaker.menu.AddNewButton()
			.SetText("Concede")
			.SetAction(Concede);

		matchmaker.menu.AddNewButton()
			.SetText("Win")
			.SetAction(Win);

		matchmaker.menu.AddNewButton()
			.SetText("Disconnect")
			.SetAction(Disconnect);

		matchmaker.menu.Show();
	}

	void Concede() {
		var data = new JSONObject();
		data.AddField("isWinner", false);
		matchmaker.Send("endGame", data);
	}

	void Win() {
		var data = new JSONObject();
		data.AddField("isWinner", true);
		matchmaker.Send("endGame", data);
	}

	void Disconnect() {
		account.boltAgent.Shutdown();
	}
}
