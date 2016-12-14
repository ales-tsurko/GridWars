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
		ResetMessageButton();

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

	void ResetMessageButton() {
		messageButton.matchesNeighborSize = false;
		messageButton.text = "PRESS " + App.shared.inputs.focusMessenger.HotkeyDescription() + " TO MESSAGE " + App.shared.account.opponent.screenName;
		messageButton.action = MessageButtonActivated;
	}

	public void TearDown() {
		isEnabled = false;

		App.shared.notificationCenter.RemoveObserver(this);

		messageButton.Destroy();
		messageButton = null;

		chatView.Destroy();
		chatView = null;

		CancelHideTimer();

		App.shared.battlefield.localPlayer1.inputs.Enabled = true;
	}

	void MessageButtonActivated() {
		chatView.Show();
		chatView.Focus();
	}

	void HideChatView() {
		if (isPlayingGame) { //leave it open after the game
			chatView.Hide();
		}
	}

	public void AppendMessage(Account account, string message) {
		chatView.AddMessage(account.player.uiColor.ColoredTag(account.screenName + ": ") + message);
		StartHideTimer();
	}

	public void HandleTextMessage(JSONObject data) {
		if (chatView.isShown) {
			App.shared.PlayAppSoundNamed("ChatReceived");
		}
		else {
			chatView.Show();
		}
		AppendMessage(App.shared.account.opponent, data.GetField("text").str);
	}

	public void ChatViewEditingEnded(Notification n) {
		var text = chatView.messageText;

		if (text.Length > 0) {
			JSONObject data = new JSONObject();

			App.shared.PlayAppSoundNamed("ChatSent");

			data.AddField("text", text);
			matchmaker.Send("textMessage", data);
			AppendMessage(App.shared.account, text);
		}

		if (isPlayingGame || text.Length == 0) {
			chatView.LoseFocus();
		}
		else {
			chatView.ClearInput();
		}
	}

	public void ChatViewReceivedFocus(Notification n) {
		messageButton.Hide();
		App.shared.battlefield.localPlayer1.inputs.Enabled = false;
		CancelHideTimer();
	}

	public void ChatViewLostFocus(Notification n) {
		messageButton.Show();
		App.shared.battlefield.localPlayer1.inputs.Enabled = true;
		chatView.ClearInput();

		if (hideTimer == null) {
			HideChatView();
		}
	}

	public void Update() {
		if (App.shared.inputs.focusMessenger.WasPressed && !chatView.hasFocus) {
			MessageButtonActivated();
		}
	}

	IEnumerator LoseFocusCouroutine() {
		yield return new WaitForEndOfFrame();
		chatView.LoseFocus();
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
