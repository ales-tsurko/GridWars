using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardResultsState : MenuState {
	public LeaderboardIndexMenuState indexState;
	public string periodStart;
	public string periodEnd;
	public string periodDescription;
	JSONObject results;

	protected override void ConfigureMenu() {
		base.ConfigureMenu();

		menu.AddNewText().SetText("Leaderboards > Single Player  > " + periodDescription + ":");

		if (results == null) {
			menu.AddNewIndicator().SetText("Fetching Results");

			app.notificationCenter.NewObservation()
				.SetNotificationName(matchmaker.ReceivedMessageNotificationName("requestLeaderboard"))
				.SetAction(MatchmakerReceivedRequestLeaderboard)
				.SetSender(matchmaker)
				.Add();
			
			JSONObject data = new JSONObject();
			data.AddField("type", indexState.type);
			data.AddField("start", periodStart);
			data.AddField("end", periodEnd);
			matchmaker.Send("requestLeaderboard", data);
		}
		else {
			var foundSelf = false;
			int i = 0;

			foreach (var obj in results.list) {
				var text = obj.GetField("description").str;
				if (obj.GetField("isSelf").b) {
					text = Color.yellow.ColoredTag(text);
					foundSelf = true;
				}
				var item = menu.AddNewText();
				item.SetText(text);

				if (i == 0) {
					item.UseRainbowStyle();
				}
				i++;
			}

			if (!foundSelf) {
				menu.AddNewText().SetText("Play vs the AI to establish your rank.");
			}
		}
	}

	void MatchmakerReceivedRequestLeaderboard(Notification n) {
		results = n.data as JSONObject;
		RebuildMenu();
	}

	public override void WillExit() {
		base.WillExit();

		app.notificationCenter.RemoveObserver(this);
	}

	protected override void Back() {
		base.Back();

		TransitionBack();
	}
}
