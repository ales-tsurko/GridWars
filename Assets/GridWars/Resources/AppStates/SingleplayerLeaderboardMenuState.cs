using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;

public class SingleplayerLeaderboardMenuState : MenuState {
	protected override void ConfigureMenu() {
		base.ConfigureMenu();

		menu.AddNewButton().SetText("Wins and Losses").SetAction(WinsAndLosses);
		menu.AddNewButton().SetText("Wins").SetAction(Wins);
		menu.AddNewButton().SetText("Win Streak").SetAction(WinStreak);
	}

	void WinsAndLosses() {
		var s = new LeaderboardIndexMenuState();
		s.type = "Wins and Losses";
		TransitionTo(s);
	}

	void Wins() {
		var s = new LeaderboardIndexMenuState();
		s.type = "Wins";
		TransitionTo(s);
	}

	void WinStreak() {
		var s = new LeaderboardIndexMenuState();
		s.type = "Win Streak";
		TransitionTo(s);
	}

	protected override void Back() {
		base.Back();

		TransitionTo(new LeaderboardsMenuState());
	}
}
