using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class MatchmakerPostAuthState : MatchmakerState {
	UIButton button;

	int _itemWindowStart = 0;
	int _itemWindowSize = 5;
	float _scrollRate = 1 / 0.2f;
	Timer _scrollTimer;
	UIButton _serverEmptyButton;
	UIButton _backButton;

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		matchmaker.menu.Close();
		matchmaker.menu.Show();

		App.shared.notificationCenter.NewObservation()
			.SetNotificationName(UIMenu.UIMenuSelectedItemNotification)
			.SetAction(MenuItemSelected)
			.SetSender(matchmaker.menu)
			.Add();
	}

	public override void WillExit() {
		base.WillExit();

		App.shared.notificationCenter.RemoveObserver(this);

		CancelStatusTimer();
	}

	// Scrolling

	public void MenuItemSelected(Notification n) {
		if (app.account.connectedAccounts.Count == 0 || _scrollTimer != null) {
			return;
		}

		_scrollTimer = App.shared.timerCenter.NewTimer().SetAction(ScrollMenu).SetTimeout(1/_scrollRate).Start();
	}

	public void ScrollMenu() {
		//Debug.Log("matchmaker.menu.selectedItemIndex: " + matchmaker.menu.selectedItemIndex);

		_scrollTimer = null;

		int lastItemOffset = 3;

		if (matchmaker.menu.selectedItemIndex == 0 && _itemWindowStart > 0) {
			//Debug.Log("scrolled up");
			//Debug.Log("hide: " + Mathf.Min(_itemWindowStart + _itemWindowSize - 1, matchmaker.menu.items.Count - lastItemOffset));
			matchmaker.menu.items[Mathf.Min(_itemWindowStart + _itemWindowSize - 1, matchmaker.menu.items.Count - lastItemOffset)].Hide();
			//Debug.Log("show: " + (_itemWindowStart - 1));
			matchmaker.menu.items[_itemWindowStart - 1].Show();
			_itemWindowStart--;
			//Debug.Log("new _itemWindowStart: " + _itemWindowStart);
		}
		else if (matchmaker.menu.selectedItemIndex == _itemWindowSize - 1 && _itemWindowStart + _itemWindowSize < matchmaker.menu.items.Count - lastItemOffset + 1) {
			//Debug.Log("scrolled down");
			//Debug.Log("hide: " + Mathf.Min(_itemWindowStart));
			matchmaker.menu.items[_itemWindowStart].Hide();
			//Debug.Log("show: " + Mathf.Min(_itemWindowStart + _itemWindowSize, matchmaker.menu.items.Count - lastItemOffset));
			matchmaker.menu.items[Mathf.Min(_itemWindowStart + _itemWindowSize, matchmaker.menu.items.Count - lastItemOffset)].Show();
			_itemWindowStart++;
			//Debug.Log("new _itemWindowStart: " + _itemWindowStart);

		}
	}

	// MatchmakerMenuDelegate

	public override void ConfigureForClosed() {
		base.ConfigureForClosed();

		matchmaker.menu.Reset();
		button = matchmaker.menu.AddNewButton();
		matchmaker.menu.inputWraps = true;
		UpdateStatus();
	}

	void UpdateStatus() {
		if (matchmaker.menu.isOpen) {
			return;
		}

		if (account.connectedAccounts.Count == 0) {
			button.text = "Online";
			button.action = null;
			matchmaker.menu.isInteractible = false;
			app.state.DisconnectMatchmakerMenu();
		}
		else {
			button.text = account.connectedAccounts.Count + " Online";

			if (account.connectedAccounts.Exists(a => a.status == AccountStatus.Searching)) {
				button.text += ": " + Color.yellow.ColoredTag("1 Posted Challenge");
			}

			MakeMenuOpenable();
		}
	}

	void MakeMenuOpenable() {
		button.action = matchmaker.menu.Open;
		matchmaker.menu.isInteractible = true;
		app.state.ConnectMatchmakerMenu();
	}

	public override void ConfigureForOpen() {
		base.ConfigureForOpen();

		matchmaker.menu.Reset();
		matchmaker.menu.inputWraps = false;

		app.account.connectedAccounts.ForEach((account) => {
			AddButtonForAccount(account);
		});

		_serverEmptyButton = matchmaker.menu.AddNewText().SetText("No one else is online");
		_serverEmptyButton.Hide();

		_backButton = matchmaker.menu.AddNewButton().SetText("Back").SetAction(matchmaker.menu.Close).SetIsBackItem(true);

		matchmaker.menu.Focus();

		UpdateOpenMenu();
	}

	public void AddButtonForAccount(Account account) {
		matchmaker.menu.AddNewButton().SetData(account).SetMenuIndex(app.account.connectedAccounts.FindIndex(a => a.id == account.id));
	}

	void UpdateOpenMenu() {
		CancelStatusTimer();

		int itemIndex = 0;

		app.account.connectedAccounts.ForEach((account) => {
			UIButton b = matchmaker.menu.items.Find(i => i.data == account);

			switch (account.status) {
			case AccountStatus.Available:
				b.SetText(account.screenName + ": Online: " + Color.yellow.ColoredTag("Send Challenge"));
				b.SetAction(() => { PostGameWithOpponent(account); });
				break;
			case AccountStatus.Searching:
				b.SetText(account.screenName + ": Posted Challenge: " + Color.yellow.ColoredTag("Accept Challenge"));
				b.SetAction(() => { PostGameWithOpponent(account); });
				break;
			case AccountStatus.Playing:
				b.SetText(account.screenName + ": Playing " + account.opponent.screenName);
				break;
			case AccountStatus.Unavailable:
				b.SetText(account.screenName + ": Busy");
				break;
			}

			if (itemIndex < _itemWindowStart || itemIndex >= _itemWindowStart + _itemWindowSize) {
				//Debug.Log("Hiding " + itemIndex);
				b.Hide();
			}
			else {
				b.Show();
			}

			itemIndex ++;
		});

		if (app.account.connectedAccounts.Count == 0) {
			_serverEmptyButton.Show();
			_backButton.Show();
			_backButton.Select();
			//app.timerCenter.NewTimer().SetAction(_backButton.Select).SetTimeout(0.1f).Start(); //Return doesn't work otherwise?
		}
		else {
			_serverEmptyButton.Hide();
		}
	}

	void PostGameWithOpponent(Account opponent) {
		var data = new JSONObject();
		data.AddField("opponent", opponent.publicPropertyData);
		matchmaker.Send("postGame", data);
		var state = new MatchmakerPostedGameState();
		state.opponent = opponent;
		TransitionTo(state);
	}

	Timer statusTimer;

	void StartStatusTimer() {
		CancelStatusTimer();

		statusTimer = App.shared.timerCenter.NewTimer();
		statusTimer.action = TimerUpdateStatus;
		statusTimer.timeout = 5;
		statusTimer.Start();
	}

	void TimerUpdateStatus() {
		CancelStatusTimer();
		UpdateStatus();
	}

	void CancelStatusTimer() {
		if (statusTimer != null) {
			statusTimer.Cancel();
			statusTimer = null;
		}
	}

	public override void HandlePlayerConnected(JSONObject data) {
		base.HandlePlayerConnected(data);

		if (matchmaker.menu.isOpen) {
			var accountIndex = app.account.connectedAccounts.FindIndex(a => a.id == data.GetField("id").f);
			var account = app.account.connectedAccounts[accountIndex];

			AddButtonForAccount(account);

			if (app.account.connectedAccounts.Count > _itemWindowSize) {
				if (accountIndex <= _itemWindowStart) {
					_itemWindowStart++;
				}
				else if (matchmaker.menu.selectedItemIndex != -1 && accountIndex <= _itemWindowStart + matchmaker.menu.selectedItemIndex) {
					_itemWindowStart++;
				}
			}

			UpdateOpenMenu();
		}
		else {
			//App.shared.PlayAppSoundNamedAtVolume("PlayerConnected", 1f);
			button.text = data.GetField("screenName").str + " is online";
			MakeMenuOpenable();
			StartStatusTimer();
		}
	}

	void UpdateMenu() {
		if (matchmaker.menu.isOpen) {
			UpdateOpenMenu();
		}
		else if (statusTimer == null) {
			UpdateStatus();
		}
	}

	public override void HandlePlayerDisconnected(JSONObject data) {
		var accountIndex = app.account.connectedAccounts.FindIndex(a => a.id == data.GetField("id").f);
		var account = app.account.connectedAccounts[accountIndex];

		if (app.account.connectedAccounts.Count > _itemWindowSize) {
			if (accountIndex <= _itemWindowStart) {
				_itemWindowStart--;
			}
			else if (matchmaker.menu.selectedItemIndex != -1 && accountIndex <= _itemWindowStart + matchmaker.menu.selectedItemIndex) {
				_itemWindowStart--;
			}
		}

		base.HandlePlayerDisconnected(data);


		var button = matchmaker.menu.items.Find(i => i.data != null && ((Account)i.data).id == account.id );
		if (button != null) {
			matchmaker.menu.DestroyItem(button);
		}

		UpdateMenu();
	}

	public override void HandleGamePosted(JSONObject data) {
		base.HandleGamePosted(data);

		var game = account.GameWithId(data.GetField("id").str);

		if (game.client == account) {
			account.game = game;
			TransitionTo(new MatchmakerReceivedChallengeState());
		}
		else {
			if (matchmaker.menu.isOpen) {
				UpdateOpenMenu();
			}
			else {
				App.shared.PlayAppSoundNamedAtVolume("PlayPVP", 1f);
				button.text = Color.yellow.ColoredTag(game.host.screenName + " posted a challenge");
				MakeMenuOpenable();
				StartStatusTimer();
			}
		}
	}

	public override void HandleGameCancelled(JSONObject data) {
		base.HandleGameCancelled(data);
		UpdateMenu();
	}

	public override void HandlePlayerBecameAvailableToPlay(JSONObject data) {
		base.HandlePlayerBecameAvailableToPlay(data);
		UpdateMenu();
	}

	public override void HandlePlayerBecameUnavailableToPlay(JSONObject data) {
		base.HandlePlayerBecameUnavailableToPlay(data);
		UpdateMenu();
	}

	public override void HandlePlayerChangedScreenName(JSONObject data) {
		base.HandlePlayerChangedScreenName(data);
		UpdateMenu();
	}
}
