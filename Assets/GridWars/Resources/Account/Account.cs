using UnityEngine;
using System.Collections.Generic;

public class Account {
	public string id;
	public string screenName;
	public string email;
	public string accessToken;
	public List<Account>playerList;
	public Game game;

	public bool isHost {
		get {
			return game.host.id == id;
		}
	}

	public bool isReadyForGame {
		get {
			return isHost ? game.hostIsReady : game.clientIsReady;
		}

		set {
			if (isHost) {
				game.hostIsReady = value;
			}
			else {
				game.clientIsReady = value;
			}
		}
	}

	public bool isOpponentReadyForGame {
		get {
			return isHost ? game.clientIsReady : game.hostIsReady;
		}

		set {
			if (isHost) {
				game.clientIsReady = value;
			}
			else {
				game.hostIsReady = value;
			}
		}
	}

	public Account opponent {
		get {
			if (game == null) {
				return null;
			}
			else {
				return isHost ? game.client : game.host;
			}
		}
	}

	public List<Account>otherPlayers {
		get {
			return playerList.FindAll(account => account != this);
		}
	}

	public Account() {
		ResetPlayerList();
	}

	public Account AccountWithId(string id) {
		if (this.id == id) {
			return this;
		}
		else {
			return playerList.Find(account => account.id == id);
		}
	}

	public void ResetPlayerList() {
		playerList = new List<Account>();
		playerList.Add(this);
	}

	public void PlayerConnected(JSONObject accountData) {
		var account = new Account();
		account.screenName = accountData.GetField("screenName").str;
		account.id = accountData.GetField("id").n.ToString();
		playerList.Add(account);
	}

	public void PlayerDisconnected(JSONObject accountData) {
		playerList.Remove(AccountWithId(accountData.GetField("id").n.ToString()));
	}

	public void PlayerChangedScreenName(JSONObject accountData) {
		var account = AccountWithId(accountData.GetField("id").n.ToString());
		if (account == null) {
			App.shared.Log("Account missing: " + accountData.ToString(), this);
		}
		else {
			account.screenName = accountData.GetField("screenName").str;
		}
	}

	public void SaveToPrefs() {
		App.shared.prefs.screenName = screenName;
		App.shared.prefs.accessToken = accessToken;
	}
}
