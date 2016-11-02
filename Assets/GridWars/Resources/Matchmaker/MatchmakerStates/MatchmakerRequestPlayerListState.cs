﻿using UnityEngine;
using System.Collections;

public class MatchmakerRequestPlayerListState : MatchmakerState {

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		matchmaker.Send("playerList", new JSONObject());
	}

	// MatchmakerMenuDelegate

	public override void HandleMessage(string name, JSONObject data) {
		base.HandleMessage(name, data);

		if (name == "playerList") {
			foreach (var obj in data.list) {
				var account = new Account();
				account.screenName = obj.GetField("screenName").str;
				app.account.playerList.Add(account);
			}
			TransitionTo(new MatchmakerPlayerListState());
		}
		else {
			HandleUnexpectedMessage(name, data);
		}
	}

	public override void MatchmakerMenuClosed() {
		base.MatchmakerMenuClosed();

		matchmaker.menu.Reset();
		matchmaker.menu.AddNewButton()
			.SetText("fetching player list ...");
		matchmaker.menu.Show();
		matchmaker.menu.Focus();
	}
}
