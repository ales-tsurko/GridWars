using UnityEngine;
using System.Collections;

public class AccountSettingsState : AppState {
	UIButton title;
	UIInput screenNameInput;
	UIButton over13Toggle;
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

		over13Toggle = menu.AddNewButton().SetText("I'm 13 years of age or older").SetData(false).SetAction(Over13Clicked);

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

	void Over13Clicked() {
		var check = "✓ ";
		if (over13Toggle.text.Contains(check)) {
			over13Toggle.text = over13Toggle.text.Substring(check.Length);
			over13Toggle.data = false;
		}
		else {
			over13Toggle.text = check + over13Toggle.text;
			over13Toggle.data = true;
		}
	}

	void SaveActivated() {
		if (matchmaker.isConnected) {
			if ((bool)over13Toggle.data) {
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
				title.text = "You must be 13 or older to save your account.";
				title.UseAlertStyle();
			}
		}
		else {
			title.text = "Can't connect to server";
			title.UseAlertStyle();
		}
	}

	void BackActivated() {
		TransitionBack();
	}

	void MatchmakerSavedAccount(Notification n) {
		var data = n.data as JSONObject;

		saveButton.Show();
		saveIndicator.Hide();

		if (data.GetField("success").b) {
			title.text = "Account Updated";
			app.account.email = data.GetField("email").str;
			app.account.screenName = data.GetField("screenName").str;
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
