using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardIndexMenuState : MenuState {
	public string type;

	protected override void ConfigureMenu() {
		base.ConfigureMenu();

		menu.AddNewText().SetText("Leaderboards > Single Player:");
		menu.AddNewButton().SetText("All Time").SetAction(AllTime);
		menu.AddNewButton().SetText("Weekly").SetAction(Weekly);
		menu.AddNewButton().SetText("Daily").SetAction(Daily);
	}

	void AllTime() {
		var s = new LeaderboardResultsState();
		s.indexState = this;
		s.periodStart = "1451606400000";
		s.periodEnd = "4607280000000";
		s.periodDescription = "All Time";
		TransitionTo(s);
	}

	void Weekly() {
		var s = new LeaderboardPeriodIndexMenuState();
		s.indexState = this;
		s.periodType = "Weekly";
		TransitionTo(s);
	}

	void Daily() {
		var s = new LeaderboardPeriodIndexMenuState();
		s.indexState = this;
		s.periodType = "Daily";
		TransitionTo(s);
	}

	protected override void Back() {
		base.Back();

		TransitionTo(new LeaderboardsMenuState());
	}
}
