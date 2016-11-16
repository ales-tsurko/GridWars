using UnityEngine;
using System.Collections;

public class AccountMenuState : AppState {
	UIButton title;
	UIInput screenNameInput;
	UIInput emailInput;
	UIButton saveButton;
	UIButton backButton;
	UIActivityIndicator saveIndicator;

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		menu.Reset();

		title = menu.AddNewText().SetText("Account Settings");

		menu.AddNewText().SetText("Email:");
		emailInput = menu.AddNewInput();
		emailInput.characterLimit = 64;
		emailInput.inputComponent.contentType = UnityEngine.UI.InputField.ContentType.EmailAddress;
		emailInput.text = app.account.email;

		menu.AddNewText().SetText("Screen Name:");

		screenNameInput = menu.AddNewInput();
		screenNameInput.capitalizes = true;
		screenNameInput.characterLimit = 20;
		screenNameInput.inputComponent.contentType = UnityEngine.UI.InputField.ContentType.Alphanumeric;
		screenNameInput.text = app.account.screenName;

		saveButton = menu.AddNewButton().SetText("Save").SetAction(SaveActivated);
		saveIndicator = menu.AddNewIndicator();
		saveIndicator.text = "";
		saveIndicator.showsDotsInline = true;
		saveIndicator.Hide();

		backButton = menu.AddNewButton().SetText("Back").SetAction(BackActivated).SetIsBackItem(true);

		menu.Show();

		menu.SelectItem(emailInput);
	}

	public override void WillExit() {
		base.WillExit();

		app.notificationCenter.RemoveObserver(this);
	}

	void SaveActivated() {
		if (matchmaker.isConnected) {
			saveButton.Hide();
			saveIndicator.Show();
			title.UseDefaultStyle();
			JSONObject data = new JSONObject();
			data.AddField("email", emailInput.text);
			data.AddField("screenName", screenNameInput.text);
			matchmaker.Send("saveAccount", data);
			app.notificationCenter.NewObservation()
				.SetNotificationName(MatchmakerState.MatchmakerSaveAccountNotification)
				.SetAction(MatchmakerSavedAccount)
				.Add();
		}
		else {
			title.text = "Can't connect to server";
			title.UseAlertStyle();
		}
	}

	void BackActivated() {
		TransitionTo(new MainMenuState());
	}

	void MatchmakerSavedAccount(Notification n) {
		var data = n.data as JSONObject;

		saveButton.Show();
		saveIndicator.Hide();

		if (data.GetField("success").b) {
			title.text = "Account Updated";
			app.account.email = data.GetField("email").str;
			app.account.screenName = data.GetField("email").str;
			app.account.SaveToPrefs();
			menu.SelectItem(backButton);
		}
		else {
			title.text = data.GetField("error").str;
			title.UseAlertStyle();
			menu.SelectItem(saveButton);
		}

		app.notificationCenter.RemoveObserver(this);
	}
}
