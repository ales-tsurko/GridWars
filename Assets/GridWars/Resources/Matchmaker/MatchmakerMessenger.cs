using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using AssemblyCSharp;

public class MatchmakerMessenger {
	UIButton messageButton;
	UIChatView chatView;
	Timer hideTimer;
	bool isPlayingGame {
		get {
			return matchmaker.state is MatchmakerPlayingGameState;
		}
	}

	bool isAfterGame {
		get {
			return matchmaker.state is MatchmakerAfterGameState;
		}
	}

	Matchmaker matchmaker {
		get {
			return App.shared.matchmaker;
		}
	}
		
	public bool isEnabled = false;
	public bool hasFocus {
		get {
			return chatView != null && chatView.hasFocus;
		}
	}

	public void Setup() {
		isEnabled = true;

		matchmaker.menu.Reset();

		matchmaker.menu.isInteractible = true;

		messageButton = matchmaker.menu.AddNewButton();
		messageButton.matchesNeighborSize = false;
		messageButton.action = MessageButtonActivated;
		ResetMessageButtonText();

		chatView = UIChatView.Instantiate();
		App.shared.notificationCenter.NewObservation()
			.SetNotificationName(UIChatView.UIChatViewSubmittedNotification)
			.SetSender(chatView)
			.SetAction(ChatViewEditingEnded)
			.Add();

		App.shared.notificationCenter.NewObservation()
			.SetNotificationName(UIChatView.UIChatViewReceivedFocusNotification)
			.SetSender(chatView)
			.SetAction(ChatViewReceivedFocus)
			.Add();

		App.shared.notificationCenter.NewObservation()
			.SetNotificationName(UIChatView.UIChatViewLostFocusNotification)
			.SetSender(chatView)
			.SetAction(ChatViewLostFocus)
			.Add();
	}

	void ResetMessageButtonText() {
		messageButton.text = "PRESS '" + App.shared.inputs.focusMessenger.HotkeyDescription() + "' TO CHAT WITH " + App.shared.account.opponent.screenName;
	}

	public void Show() {
		chatView.Show();
	}

	public void TearDown() {
		isEnabled = false;

		//Debug.Log("TearDown");

		App.shared.notificationCenter.RemoveObserver(this);

		messageButton.Destroy();
		messageButton = null;

		chatView.Hide();
		chatView.Destroy();
		chatView = null;

		CancelHideTimer();

		App.shared.battlefield.localPlayer1.inputs.Enabled = true;
	}

	void MessageButtonActivated() {
		if (App.shared.inputs.LastInputType != InControl.BindingSourceType.DeviceBindingSource) {
			chatView.Show();
			chatView.Focus();
		}
	}

	void HideChatView() {
		if (isPlayingGame) { //leave it open after the game
			chatView.Hide();
		}
	}

	string AttributedMessage(Account account, string message) {
		return account.player.uiColor.ColoredTag(account.screenName + ": ") + message;
	}

	public void HandleTextMessage(JSONObject data) {
		App.shared.PlayAppSoundNamed("ChatReceived");
		var attributedMessage = AttributedMessage(App.shared.account.opponent, data.GetField("text").str);
		chatView.AddMessage(attributedMessage);
		if (!chatView.isShown && isPlayingGame) {
			if (attributedMessage.Length > 60) {
				attributedMessage = attributedMessage.Substring(0, 60) + "...";
			}
			attributedMessage += " ('M' to Respond)";
			messageButton.text = attributedMessage;
		}
	}

	public void ChatViewEditingEnded(Notification n) {
		var text = chatView.messageText;

		if (text.Length > 0) {
			JSONObject data = new JSONObject();

			App.shared.PlayAppSoundNamed("ChatSent");

			data.AddField("text", text);
			matchmaker.Send("textMessage", data);
			chatView.AddMessage(AttributedMessage(App.shared.account, text));
		}

		chatView.ClearInput();

		if (text.Length == 0) {
			chatView.LoseFocus();
		}
	}

	public void ChatViewReceivedFocus(Notification n) {
		messageButton.Hide();
		App.shared.battlefield.localPlayer1.inputs.Enabled = false;
		CancelHideTimer();
	}

	public void ChatViewLostFocus(Notification n) {
		ResetMessageButtonText();
		messageButton.Show();
		App.shared.battlefield.localPlayer1.inputs.Enabled = true;
		chatView.ClearInput();
		HideChatView();
	}

	public void Update() {
		if ((isPlayingGame || isAfterGame) && !chatView.hasFocus) {
			if (App.shared.inputs.focusMessenger.WasPressed) {
				MessageButtonActivated();
			}
			else if (App.shared.focusedMenu == null && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))) {
				App.shared.StartCoroutine(MessageButtonActivatedAtEndOfFrame()); //so UIChatView doesn't read enter key
			}
		}
		else if (chatView.hasFocus && Input.GetKeyDown(KeyCode.Tab)) {
			chatView.LoseFocus();
		}
	}

	IEnumerator MessageButtonActivatedAtEndOfFrame() {
		yield return new WaitForEndOfFrame();
		MessageButtonActivated();
	}

	void StartHideTimer() {
		CancelHideTimer();

		hideTimer = App.shared.timerCenter.NewTimer();
		hideTimer.action = HideTimerFired;
		hideTimer.timeout = 5f;
		hideTimer.Start();
	}

	void HideTimerFired() {
		CancelHideTimer();
		HideChatView();
	}

	void CancelHideTimer() {
		if (hideTimer != null) {
			hideTimer.Cancel();
			hideTimer = null;
		}
	}
}
