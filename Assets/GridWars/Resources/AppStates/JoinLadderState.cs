using UnityEngine;

public class JoinLadderState : AppState {
	public string terms;

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		menu.Reset();

		if (app.account.email == null || app.account.email == "") {
			menu.AddNewText().SetText("Email Address is Required to Join Ladder");
			menu.AddNewButton().SetText("Go To Account Settings").SetAction(GoToSettings);
			menu.AddNewButton().SetText("Back").SetAction(TransitionBack);
		}
		else {
			menu.AddNewText().SetText("Contest Terms and Conditions");
			var sv = menu.AddNewTextScrollView().SetText(terms);
			
			sv.GetComponent<RectTransform>().sizeDelta = new Vector2(765, 600);
			menu.AddNewButton().SetText("I’VE READ THE CONTEST RULES, AM ELIGIBLE AND AGREE").SetAction(JoinLadder);
			menu.AddNewButton().SetText("Back").SetAction(Back).SetIsBackItem(true);
		}
		
		menu.Show();
	}

	public override void WillExit() {
		base.WillExit();
		
		app.notificationCenter.RemoveObserver(this);
	}

	void GoToSettings() {
		TransitionTo(new AccountSettingsState());
	}

	void JoinLadder() {
		menu.Reset();
		menu.AddNewIndicator().SetText("Joining Ladder");
		menu.AddNewText().SetText("Back").SetAction(Back).SetIsBackItem(true);
		menu.Show();

		app.notificationCenter.NewObservation()
		.SetNotificationName(matchmaker.ReceivedMessageNotificationName("joinLadder"))
		.SetAction(HandleMatchmakerReceivedJoinLadder)
		.Add();

		matchmaker.Send("joinLadder");
	}

	void HandleMatchmakerReceivedJoinLadder(Notification n) {
		app.notificationCenter.RemoveObserver(this);
		
		menu.Reset();
		menu.AddNewText().SetText("Good Luck Have Fun");
		menu.AddNewButton().SetText("Back").SetAction(Back).SetIsBackItem(true);
		menu.Show();
	}

	void Back() {
		TransitionTo(new LadderState());
	}
}
