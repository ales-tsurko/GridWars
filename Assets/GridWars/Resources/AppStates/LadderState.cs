using UnityEngine;
using System.Collections;

public class LadderState : AppState {
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
		TransitionTo(new MainMenuState());
	}

	public void MatchmakerReceivedRequestLadder(Notification n) {
		JSONObject data = n.data as JSONObject;

		var lastRank = 0f;

		menu.Reset();

		var accountDataList = data.GetField("ladder").list;

		if (accountDataList.Count == 0) {
			menu.AddNewText().SetText("Play Internet PVP to Establish Your Rank");
		}
		else {
			foreach (var accountData in data.GetField("ladder").list) {
				var account = new Account();
				account.SetFromData(accountData);

				if (account.rank > lastRank + 1) {
					menu.AddNewText().SetText("...");
				}

				var text = account.rank + ". " + account.screenName + " " + account.wins + "-" + account.losses;
				if (account.id == app.account.id) {
					text = Color.yellow.ColoredTag(text);
				}

				menu.AddNewText().SetText(text);

				lastRank = account.rank;
			}
		}

		menu.AddNewButton().SetText("Back").SetAction(Back).SetIsBackItem(true);

		menu.Focus();
	}
}
