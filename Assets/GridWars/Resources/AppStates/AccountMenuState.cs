using UnityEngine;
using System.Collections;

public class AccountMenuState : AppState {
	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		menu.Reset();


		if (matchmaker.isConnected) {
			menu.AddNewText().SetText(app.account.screenName + ":");
			menu.AddNewButton().SetText("Settings").SetAction(Settings);
			menu.AddNewButton().SetText("Switch Account").SetAction(SwitchAccount);
		}
		else {
			menu.AddNewText().SetText("Can't connect to server.").UseAlertStyle();
		}

		menu.AddNewButton().SetText("Back").SetAction(Back).SetIsBackItem(true);

		menu.Show();
	}

	void Settings() {
		TransitionTo(new AccountSettingsState());
	}

	void SwitchAccount() {
		TransitionTo(new SwitchAccountState());
	}

	void Back() {
		TransitionTo(new MainMenuState());
	}
}
