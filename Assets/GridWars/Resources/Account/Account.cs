using UnityEngine;
using System.Collections.Generic;

public class Account {
	public string id;
	public string screenName;
	public string email;
	public string accessToken;
	public List<Account>playerList;
	public Challenge challenge;

	public Account() {
		ResetPlayerList();
	}

	public Account AccountNamed(string screenName) {
		return playerList.Find(account => account.screenName == screenName);
	}

	public void ResetPlayerList() {
		playerList = new List<Account>();
	}

	public void PlayerConnected(string screenName) {
		var account = new Account();
		account.screenName = screenName;
		playerList.Add(account);
	}

	public void PlayerDisconnected(string screenName) {
		playerList.Remove(AccountNamed(screenName));
	}
}
