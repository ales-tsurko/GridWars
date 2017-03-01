using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardsMenuState : MenuState {
	protected override void ConfigureMenu() {
		base.ConfigureMenu();

		menu.AddNewButton().SetText("Singleplayer").SetAction(Singleplayer);
		menu.AddNewButton().SetText("Multiplayer").SetAction(Multiplayer);
	}

	void Singleplayer() {
		var s = new LeaderboardIndexMenuState();
		s.type = "Level";
		TransitionTo(s);
	}

	void Multiplayer() {
		TransitionTo(new LadderState());
	}

	protected override void Back() {
		base.Back();

		TransitionTo(new MainMenuState());
	}
}
