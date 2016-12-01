using UnityEngine;
using System.Collections.Generic;
using AssemblyCSharp;

public class MatchmakerMessenger {
	UIButton messageButton;
	UIInput messageInput;

	Matchmaker matchmaker {
		get {
			return App.shared.matchmaker;
		}
	}

	bool isEditingMessageInput = false;

	public bool isEnabled = false;

	Timer messageTimer;

	List<string> messages;


	public void Setup() {
		isEnabled = true;

		matchmaker.menu.Reset();

		matchmaker.menu.isInteractible = true;

		messageButton = matchmaker.menu.AddNewButton();
		messageButton.matchesNeighborSize = false;
		messageButton.text = "CLICK HERE OR PRESS " + App.shared.inputs.focusMessenger.HotkeyDescription() + " TO MESSAGE " + App.shared.account.opponent.screenName;
		messageButton.action = ShowMessageInput;

		messageInput = matchmaker.menu.AddNewInput();
		messageInput.capitalizes = true;
		messageInput.matchesNeighborSize = false;
		messageInput.characterLimit = 64;
		messageInput.inputComponent.contentType = UnityEngine.UI.InputField.ContentType.Standard;
		messageInput.Hide();

		messages = new List<string>();

		App.shared.notificationCenter.NewObservation()
			.SetSender(matchmaker.menu)
			.SetNotificationName(UIMenu.UIMenuDeselectedItemNotification)
			.SetAction(MenuDeselectedItem)
			.Add();
	}

	public void TearDown() {
		isEnabled = false;

		App.shared.notificationCenter.RemoveObserver(this);

		if (messageTimer != null) {
			messageTimer.Cancel();
			messageTimer = null;
		}

		messageButton.Destroy();
		messageButton = null;

		messageInput.Destroy();
		messageInput = null;

		App.shared.battlefield.localPlayer1.inputs.Enabled = true;

		isEditingMessageInput = false;
	}

	void ShowMessageInput() {
		isEditingMessageInput = true;

		App.shared.battlefield.localPlayer1.inputs.Enabled = false;

		messageButton.Hide();

		messageInput.text = "";
		messageInput.Show();
		matchmaker.menu.SelectItem(messageInput);

		if (messageTimer != null) {
			messageTimer.Cancel();
			messageTimer = null;
		}
	}

	void HideMessageInput() {
		Debug.Log("HIDE MESSAGE INPUT");
		isEditingMessageInput = false;

		App.shared.battlefield.localPlayer1.inputs.Enabled = true;

		messageInput.Hide();

		messageButton.Show();

		ShowNextMessage();
	}

	public void MenuDeselectedItem(Notification n) {
		if ((n.data as UIInput) == messageInput) {
			HideMessageInput();
		}
	}

	public void AppendMessage(Account account, string message) {
		messages.Add(account.player.uiColor.ColoredTag(account.screenName + ": ") + message);
		ShowNextMessage();
	}

	public void PrependMessage(Account account, string message) {
		messages.Insert(0, account.player.uiColor.ColoredTag(account.screenName + ": ") + message);
		ShowNextMessage();
	}

	void ShowNextMessage() {
		if (messages.Count > 0 && !isEditingMessageInput && messageTimer == null) {
			messageButton.text = messages[0];
			messages.RemoveAt(0);

			if (messages.Count > 0) {
				messageTimer = App.shared.timerCenter.NewTimer();
				messageTimer.action = TimerShowNextMessage;
				messageTimer.timeout = 1f;
				messageTimer.Start();
			}
		}
	}

	void TimerShowNextMessage() {
		messageTimer = null;
		ShowNextMessage();
	}

	public void HandleTextMessage(JSONObject data) {
		AppendMessage(App.shared.account.opponent, data.GetField("text").str);
	}

	public void Update() {
		if (isEditingMessageInput) {
			if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) {
				var text = messageInput.text.Trim();

				if (text.Length > 0) {
					JSONObject data = new JSONObject();

					data.AddField("text", text);
					matchmaker.Send("textMessage", data);
					PrependMessage(App.shared.account, text);
				}

				matchmaker.menu.LoseFocus();
			}
			else if (Input.GetKeyDown(KeyCode.Escape)) {
				matchmaker.menu.LoseFocus();
			}
		}
		else if (App.shared.inputs.focusMessenger.WasPressed){
			ShowMessageInput();
		}
	}
}
