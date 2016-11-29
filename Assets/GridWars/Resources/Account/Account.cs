using UnityEngine;
using System.Collections.Generic;
using System.Collections.Generic;
using UnityEngine.Analytics;
using System.Linq;

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

	public Player player {
		get {
			if (this == App.shared.account) {
				return App.shared.battlefield.localPlayer1;
			}
			else {
				return App.shared.battlefield.localPlayer1.opponent;
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

	public AccountStatus status {
		get {
			if (isAvailableToPlay) {
				if (game == null) {
					return AccountStatus.Available;
				}
				else {
					return AccountStatus.Searching;
				}
			}
			else {
				if (game == null) {
					return AccountStatus.Unavailable;
				}
				else {
					return AccountStatus.Playing;
				}
			}
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

		if ((property = accountData.GetField("players")) != null && !property.IsNull) {
			foreach (var otherAccountData in accountData.GetField("players").list) {
				var playerAccount = new Account();
				playerAccount.SetFromData(otherAccountData);
				connectedAccounts.Add(playerAccount);
			}

			foreach (var otherAccountData in accountData.GetField("players").list) {
				if ((property = otherAccountData.GetField("game")) != null && !property.IsNull) {
					var game = new Game();
					game.SetFromData(otherAccountData.GetField("game"));
				}
			}

			Sort();
		}
		lastUpdateTime = Time.time;
	}

	void Sort() {
		connectedAccounts.Sort((a, b) => {
			switch(a.status) {
			case AccountStatus.Searching:
				if (b.status == AccountStatus.Searching) {
					return b.lastUpdateTime.CompareTo(a.lastUpdateTime);
				}
				else {
					return -1;
				}
			case AccountStatus.Available:
				if (b.status == AccountStatus.Searching) {
					return 1;
				}
				else if (b.status == AccountStatus.Available) {
					return b.lastUpdateTime.CompareTo(a.lastUpdateTime);
				}
				else {
					return -1;
				}
			case AccountStatus.Playing:
				if (b.status == AccountStatus.Unavailable) {
					return -1;
				}
				if (b.status == AccountStatus.Playing) {
					return b.lastUpdateTime.CompareTo(a.lastUpdateTime);
				}
				else {
					return 1;
				}
			case AccountStatus.Unavailable:
				if (b.status == AccountStatus.Unavailable) {
					return b.lastUpdateTime.CompareTo(a.lastUpdateTime);
				}
				else {
					return 1;
				}
			}

			return b.lastUpdateTime.CompareTo(a.lastUpdateTime);
		});
	}

	public void ResetPlayerList() {
		connectedAccounts = new List<Account>();
	}

	public void PlayerConnected(JSONObject accountData) {
		var account = new Account();
		account.SetFromData(accountData);
		connectedAccounts.Add(account);
		Sort();
	}

	public void PlayerDisconnected(JSONObject accountData) {
		connectedAccounts.Remove(AccountWithId(accountData.GetField("id").n));
		Sort();
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
		Sort();
	}

	public void PlayerBecameAvailableToPlay(JSONObject accountData) {
		var account = AccountWithId(accountData.GetField("id").n);
		if (account == null) {
			App.shared.Log("Account missing: " + accountData.ToString(), this);
		}
		else {
			//App.shared.Log("PlayerBecameAvailableToPlay: " + account.screenName);
			account.isAvailableToPlay = true;
			account.game = null;
			account.lastUpdateTime = Time.time;
		}
		Sort();
	}

	public void PlayerBecameUnavailableToPlay(JSONObject accountData) {
		var account = AccountWithId(accountData.GetField("id").n);
		if (account == null) {
			App.shared.Log("Account missing: " + accountData.ToString(), this);
		}
		else {
			//App.shared.Log("PlayerBecameUnavailableToPlay: " + account.screenName);
			account.isAvailableToPlay = false;
			account.lastUpdateTime = Time.time;
			var gameData = accountData.GetField("game");
			if (gameData != null && !gameData.IsNull) {
				var game = GameWithId(gameData.GetField("id").str);
				if (game == null) {
					//its a challenge -- we never received the posted game
					game = new Game();
				}
				game.SetFromData(gameData); //will set game property on host and client
			}
		}
		Sort();
	}

	public void GamePosted(JSONObject gameData) {
		var game = new Game();
		game.SetFromData(gameData);
		Sort();
		//App.shared.Log("GamePosted: " + game.host.id);
	}

	public void GameCancelled(JSONObject gameData) {
		var host = gameData.GetField("host");

		//App.shared.Log("GameCancelled: " + host.GetField("id").n);

		var account = AccountWithId(host.GetField("id").n);
		if (account != null) {
			//App.shared.Log(account.screenName + ": account.game = null;", this);
			account.game = null;
			account.lastUpdateTime = Time.time;
			Sort();
		}
	}

	public void SaveToPrefs() {
		App.shared.prefs.screenName = screenName;
		App.shared.prefs.accessToken = accessToken;
	}

    public void LogEvent(string eventName){
        Dictionary <string, object> dict = new Dictionary<string, object>
        {
            { "platform", Application.platform.ToString() },
            { "id", id.ToString()},
            { "screenName", screenName },
        };
       
        if (opponent != null) {
            dict.Add("opponentId", opponent.id.ToString());
            dict.Add("opponentScreenName", opponent.screenName);
        }
        if (App.shared.battlefield !=null && App.shared.battlefield.isPlayingGame) {
            dict.Add("gameType", App.shared.battlefield.GetGameType().ToString());
        }
        if (App.shared.config == null) {
            Debug.LogError("Event Not Sent, config is null");
            return;
        }
        if (App.shared.config.name != "Release") {
            string t = "<color=green>Event Would Be Sent: " + eventName + "</color>\n" + string.Join(",", dict.Select(kv => kv.Key + "=" + kv.Value).ToArray());
            Debug.Log(t);
        } else {
            Analytics.CustomEvent(eventName, dict);
        }

    }
}

public enum AccountStatus { Available, Unavailable, Searching, Playing };