using UnityEngine;
using System.Collections.Generic;

public class Account {
	public float id;
	public string screenName;
	public string email;
	public string accessToken;
	public bool isAvailableToPlay;
	public float lastUpdateTime;
	public List<Account>connectedAccounts;
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

	public Account potentialOpponent {
		get {
			var available = new List<Account>(connectedAccounts.FindAll(a => a.isAvailableToPlay));
			available.Sort((a, b) => {
				if (a.game == null) {
					if (b.game == null) {
						return b.lastUpdateTime.CompareTo(a.lastUpdateTime);
					}
					else {
						return 1;
					}
				}
				else if (b.game == null) {
					return -1;
				}
				else {
					return b.lastUpdateTime.CompareTo(a.lastUpdateTime);
				}
			});

			if (available.Count > 0) {
				return available[0];
			}
			else {
				return null;
			}
		}
	}

	public Account() {
		ResetPlayerList();
		lastUpdateTime = Time.time;
	}

	public Account AccountWithId(float id) {
		if (this.id == id) {
			return this;
		}
		else {
			return connectedAccounts.Find(account => account.id == id);
		}
	}

	public Game GameWithId(string id) {
		if (game != null && game.id == id) {
			return game;
		}
		else {
			foreach (var account in connectedAccounts) {
				if (account.game != null && account.game.id == id) {
					return account.game;
				}
			}
			return null;
		}
	}

	public void SetFromData(JSONObject accountData) {
		JSONObject property;

		id = accountData.GetField("id").n;
		screenName = accountData.GetField("screenName").str;

		if ((property = accountData.GetField("accessToken")) != null && !property.IsNull) {
			accessToken = property.str;
		}

		if ((property = accountData.GetField("email")) != null && !property.IsNull) {
			email = property.str;
		}

		if ((property = accountData.GetField("isAvailableToPlay")) != null && !property.IsNull) {
			isAvailableToPlay = property.b;
		}
	}

	public JSONObject publicPropertyData {
		get {
			var data = new JSONObject();
			data.AddField("id", id);
			data.AddField("screenName", screenName);
			return data;
		}
	}

	public void ResetPlayerList() {
		connectedAccounts = new List<Account>();
	}

	public void PlayerConnected(JSONObject accountData) {
		var account = new Account();
		account.SetFromData(accountData);
		connectedAccounts.Add(account);
	}

	public void PlayerDisconnected(JSONObject accountData) {
		connectedAccounts.Remove(AccountWithId(accountData.GetField("id").n));
	}

	public void PlayerChangedScreenName(JSONObject accountData) {
		var account = AccountWithId(accountData.GetField("id").n);
		if (account == null) {
			App.shared.Log("Account missing: " + accountData.ToString(), this);
		}
		else {
			account.screenName = accountData.GetField("screenName").str;
			account.lastUpdateTime = Time.time;
		}
	}

	public void PlayerBecameAvailableToPlay(JSONObject accountData) {
		var account = AccountWithId(accountData.GetField("id").n);
		if (account == null) {
			App.shared.Log("Account missing: " + accountData.ToString(), this);
		}
		else {
			account.isAvailableToPlay = true;
			account.game = null;
			account.lastUpdateTime = Time.time;
		}
	}

	public void PlayerBecameUnavailableToPlay(JSONObject accountData) {
		var account = AccountWithId(accountData.GetField("id").n);
		if (account == null) {
			App.shared.Log("Account missing: " + accountData.ToString(), this);
		}
		else {
			account.isAvailableToPlay = false;
			account.lastUpdateTime = Time.time;
		}
	}

	public void GamePosted(JSONObject gameData) {
		var game = new Game();
		game.SetFromData(gameData);
		game.host.game = game;
		game.host.lastUpdateTime = Time.time;
	}

	public void GameCancelled(JSONObject gameData) {
		var host = gameData.GetField("host");

		var account = AccountWithId(host.GetField("id").n);
		if (account != null) {
			account.game = null;
			account.lastUpdateTime = Time.time;
		}
	}

	public void SaveToPrefs() {
		App.shared.prefs.screenName = screenName;
		App.shared.prefs.accessToken = accessToken;
	}
}
