using UnityEngine;
using System.Collections.Generic;

public class Account {
	public string id;
	public string screenName;
	public string email;
	public List<Account>friends;

	public Account() {
		friends = new List<Account>();
	}
}
