using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using AssemblyCSharp;

public class MatchmakerMessenger {
	UIButton messageButton;
	UIChatView chatView;
	Timer hideTimer;

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
		messageButton.text = "CLICK HERE OR PRESS " + App.shared.inputs.focusMessenger.HotkeyDescription() + " TO MESSAGE " + App.shared.account.opponent.screenName;
		messageButton.action = MessageButtonActivated;

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

	public void AppendMessage(Account account, string message) {
		Debug.Log("AppendMessage");
		chatView.AddMessage(account.player.uiColor.ColoredTag(account.screenName + ": ") + message);
		StartHideTimer();
	}

	public void HandleTextMessage(JSONObject data) {
		chatView.Show();
		AppendMessage(App.shared.account.opponent, data.GetField("text").str);
	}

	public void ChatViewEditingEnded(Notification n) {
		var text = chatView.messageText;

		if (text.Length > 0) {
			JSONObject data = new JSONObject();

			data.AddField("text", text);
			matchmaker.Send("textMessage", data);
			AppendMessage(App.shared.account, text);
		}

		if (matchmaker.matchmakerState is MatchmakerPlayingGameState || text.Length == 0) {
			chatView.LoseFocus();
		}
		else {
			chatView.ClearInput();
		}
	}

	public void ChatViewReceivedFocus(Notification n) {
		Debug.Log("ChatViewReceivedFocus");
		messageButton.Hide();
		App.shared.battlefield.localPlayer1.inputs.Enabled = false;
		CancelHideTimer();
	}

	public void ChatViewLostFocus(Notification n) {
		Debug.Log("ChatViewLostFocus");
		messageButton.Show();
		App.shared.battlefield.localPlayer1.inputs.Enabled = true;
		chatView.ClearInput();

		if (hideTimer == null) {
			chatView.Hide();
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
		hideTimer.timeout = 10f;
		hideTimer.Start();

		Debug.Log("StartHideTimer");
	}

	void HideTimerFired() {
		CancelHideTimer();
		chatView.Hide();
		Debug.Log("HideTimerFired");
	}

	void CancelHideTimer() {
		if (hideTimer != null) {
			Debug.Log("CancelHideTimer");
			hideTimer.Cancel();
			hideTimer = null;
		}
	}
}
