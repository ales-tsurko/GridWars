using UnityEngine;
using System.Collections;

public class SwitchAccountState : AppState {
	UIButton titleText;
	UIInput emailInput;
	string email;
	UIActivityIndicator requestCodeIndicator;
	UIButton requestCodeButton;

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		ShowRequestMenu();
	}

	void ShowRequestMenu() {
		menu.Reset();

		if (matchmaker.isConnected) {
			titleText = menu.AddNewText().SetText("Enter Email:");
			emailInput = menu.AddNewInput();
			emailInput.SetText("");
			emailInput.capitalizes = false;
			requestCodeIndicator = menu.AddNewIndicator();
			requestCodeIndicator.showsDotsInline = true;
			requestCodeIndicator.Hide();
			requestCodeButton = menu.AddNewButton().SetText("Request Login Code").SetAction(RequestLoginCode);
		}
		else {
			menu.AddNewText().SetText("Can't connect to server.").UseAlertStyle();
		}

		menu.AddNewButton().SetText("Back").SetAction(Back).SetIsBackItem(true);

		menu.Show();
	}

	public override void WillExit() {
		base.WillExit();

		app.notificationCenter.RemoveObserver(this);
	}

	void RequestLoginCode() {
		requestCodeIndicator.Show();
		requestCodeButton.Hide();
		titleText.UseDefaultStyle();

		var messageName = "requestLoginCode";

		app.notificationCenter.NewObservation()
			.SetAction(ReceiveLoginCode)
			.SetNotificationName(matchmaker.ReceivedMessageNotificationName(messageName))
			.SetSender(matchmaker)
			.Add();

		email = emailInput.text;

		JSONObject data = new JSONObject();
		data.AddField("email", email);
		matchmaker.Send(messageName, data);
	}

	void ReceiveLoginCode(Notification n) {
		app.notificationCenter.RemoveObserver(this);

		var data = n.data as JSONObject;

		requestCodeButton.Show();
		requestCodeIndicator.Hide();

		if (data.GetField("success").b) {
			ShowCodeForm();
		}
		else {
			titleText.text = data.GetField("error").str;
			titleText.UseAlertStyle();
			menu.SelectItem(requestCodeButton);
		}
	}

	UIInput codeInput;
	UIButton submitCodeButton;
	UIActivityIndicator submitCodeIndicator;

	void ShowCodeForm() {
		menu.Reset();

		titleText = menu.AddNewText().SetText("Enter Code Emailed To " + email + ":");

		codeInput = menu.AddNewInput();
		codeInput.capitalizes = true;
		codeInput.characterLimit = 6;

		submitCodeButton = menu.AddNewButton().SetText("Submit").SetAction(SubmitCode);
		submitCodeIndicator = menu.AddNewIndicator();
		submitCodeIndicator.showsDotsInline = true;
		submitCodeIndicator.Hide();

		menu.AddNewButton().SetText("Cancel").SetAction(CancelCode).SetIsBackItem(true);

		menu.Show();

		menu.SelectItem(codeInput);
	}

	void SubmitCode() {
		submitCodeIndicator.Show();
		submitCodeButton.Hide();
		titleText.UseDefaultStyle();

		var messageName = "credentialsForCode";

		app.notificationCenter.NewObservation()
			.SetAction(ReceiveCredentialsForCode)
			.SetNotificationName(matchmaker.ReceivedMessageNotificationName(messageName))
			.SetSender(matchmaker)
			.Add();

		JSONObject data = new JSONObject();
		data.AddField("authCode", codeInput.text);
		matchmaker.Send(messageName, data);
	}

	void ReceiveCredentialsForCode(Notification n) {
		Debug.Log("ReceiveCredentialsForCode");
		app.notificationCenter.RemoveObserver(this);

		var data = n.data as JSONObject;

		submitCodeButton.Show();
		submitCodeIndicator.Hide();

		Debug.Log(data.GetField("success").b);

		if (data.GetField("success").b) {
			var credentials = data.GetField("credentials");
			app.account.accessToken = credentials.GetField("accessToken").str;
			app.account.email = credentials.GetField("email").str;
			app.account.screenName = credentials.GetField("screenName").str;
			app.account.SaveToPrefs();


			menu.Reset();
			menu.AddNewText().SetText("You're now logged in as " + app.account.screenName);
			menu.AddNewButton().SetText("Back").SetAction(Back).SetIsBackItem(true);
			menu.Show();

			matchmaker.Disconnect();

		}
		else {
			titleText.text = data.GetField("error").str;
			titleText.UseAlertStyle();
			menu.SelectItem(codeInput);
		}
	}

	void CancelCode() {
		ShowRequestMenu();
	}

	void Back() {
		if (matchmaker.isConnected) {
			TransitionTo(new AccountMenuState());
		}
		else {
			TransitionTo(new MainMenuState());
		}
	}
}