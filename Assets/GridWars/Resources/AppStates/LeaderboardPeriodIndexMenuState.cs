using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardPeriodIndexMenuState : MenuState {
	public LeaderboardIndexMenuState indexState;
	public string periodType;

	JSONObject results;

	protected override void ConfigureMenu() {
		base.ConfigureMenu();

		menu.AddNewText().SetText("Leaderboards > " + indexState.type + " > " + periodType);

		if (results == null) {
			menu.AddNewIndicator().SetText("Fetching Times");

			app.notificationCenter.NewObservation()
				.SetNotificationName(matchmaker.ReceivedMessageNotificationName("requestLeaderboardPeriods"))
				.SetAction(MatchmakerReceivedRequestLeaderboardPeriods)
				.SetSender(matchmaker)
				.Add();

			JSONObject data = new JSONObject();
			data.AddField("periodType", periodType);
			matchmaker.Send("requestLeaderboardPeriods", data);
		}
		else {
			results.list.ForEach((obj) => {
				menu.AddNewButton().SetText(obj.GetField("description").str).SetAction(() => {
					var s = new LeaderboardResultsState();
					s.indexState = indexState;
					s.periodStart = obj.GetField("start").str;
					s.periodEnd = obj.GetField("end").str;
					s.periodDescription = obj.GetField("description").str;
					TransitionTo(s);
				});
			});
		}
	}

	void MatchmakerReceivedRequestLeaderboardPeriods(Notification n) {
		results = n.data as JSONObject;
		RebuildMenu();
	}

	public override void WillExit() {
		base.WillExit();

		app.notificationCenter.RemoveObserver(this);
	}

	protected override void Back() {
		base.Back();

		TransitionTo(indexState);
	}
}
