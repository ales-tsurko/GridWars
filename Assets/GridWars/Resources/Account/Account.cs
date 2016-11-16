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
			return game.host.screenName == screenName;
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

	public Account AccountNamed(string screenName) {
		return playerList.Find(account => account.screenName == screenName);
	}

	public void ResetPlayerList() {
		playerList = new List<Account>();
		playerList.Add(this);
	}

	public void PlayerConnected(JSONObject accountData) {
		var account = new Account();
		account.screenName = accountData.GetField("screenName").str;
		playerList.Add(account);
	}

	public void PlayerDisconnected(JSONObject accountData) {
		playerList.Remove(AccountNamed(accountData.GetField("screenName").str));
	}

	public void SaveToPrefs() {
		App.shared.prefs.screenName = screenName;
		App.shared.prefs.accessToken = accessToken;
	}
}
