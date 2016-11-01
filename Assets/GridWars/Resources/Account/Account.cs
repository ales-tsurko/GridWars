using UnityEngine;
using System.Collections.Generic;

public class Account {
	public string id;
	public string screenName;
	public string email;
	public string accessToken;
	public List<Account>playerList;

	public Account() {
		playerList = new List<Account>();
	}
}
