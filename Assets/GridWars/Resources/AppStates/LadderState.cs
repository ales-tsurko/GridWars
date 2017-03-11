using UnityEngine;
using System;

public class LadderState : AppState {
	string terms;

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		menu.Reset();

		menu.AddNewIndicator().SetText("Fetching Ladder");
		menu.AddNewButton().SetText("Back").SetAction(Back).SetIsBackItem(true);

		menu.Show();

		app.notificationCenter.NewObservation()
			.SetNotificationName(matchmaker.ReceivedMessageNotificationName("requestLadder"))
			.SetAction(MatchmakerReceivedRequestLadder)
			.SetSender(matchmaker)
			.Add();

		matchmaker.Send("requestLadder");
	}

	public override void WillExit() {
		base.WillExit();

		app.notificationCenter.RemoveObserver(this);
	}

	void Back() {
		TransitionTo(new LeaderboardsMenuState());
	}

	public void MatchmakerReceivedRequestLadder(Notification n) {
		JSONObject data = n.data as JSONObject;

		var lastRank = 0f;

		menu.Reset(); 

		var accountDataList = data.GetField("ladder").list;
		/*
		var prize = data.GetField("prize").n;
		var terminationTime = DateTime.Parse(data.GetField("terminationTime").str);
		var joinedLadder = data.GetField("joinedLadder").b;
		*/
		terms = data.GetField("terms").str.Replace("\\n", "\n");

		//menu.AddNewText().SetText("THE #1 PLAYER AT " + terminationTime + " WINS " + Color.yellow.ColoredTag("$" + prize + " IN BITCOIN"));

		var isRanked = false; 

		menu.AddNewText().SetText("Leaderboards > Multiplayer > All Time:");

		var first = true;
		foreach (var accountData in accountDataList) {
			var account = new Account();
			account.SetFromData(accountData);

			if (account.rank > lastRank + 1) {
				menu.AddNewText().SetText("...");
			}

			var text = account.rank + ". " + account.screenName + " " + account.wins + "-" + account.losses;
			if (account.id == app.account.id) {
				isRanked = true;
				if (!first) {
					text = Color.yellow.ColoredTag(text);
				}
			}

			var item = menu.AddNewText().SetText(text);

			lastRank = account.rank;

			if (first) {
				item.UseRainbowStyle();
			}

			first = false;
		}

		if (!isRanked) {
			if (accountDataList.Count > 0) {
				menu.AddNewText().SetText(Color.yellow.ColoredTag("??? " + app.account.screenName));
			}

			/*
			if (joinedLadder) {
			*/
				menu.AddNewText().SetText("Play Internet PVP to Establish Your Rank");
			/*}
			else {
				//menu.AddNewButton().SetText("Join Ladder").SetAction(JoinLadder);
			}
			*/
		}

		menu.AddNewButton().SetText("Back").SetAction(Back).SetIsBackItem(true);

		menu.Focus();
	}

	void JoinLadder() {
		var s = new JoinLadderState();
		Debug.Log(terms);
		s.terms = terms;
		TransitionTo(s);
	}
}
