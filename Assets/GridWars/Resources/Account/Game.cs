using UnityEngine;
using System.Collections;

public class Game {
	public string id;
	public Account host;
	public bool hostIsReady;
	public Account client;
	public bool clientIsReady;

	public void SetFromData(JSONObject data) {
		id = data.GetField("id").str;

		host = App.shared.account.AccountWithId(data.GetField("host").GetField("id").n);
		var clientData = data.GetField("client");
		if (clientData != null && !clientData.IsNull) {
			client = App.shared.account.AccountWithId(clientData.GetField("id").n);
		}

		host.game = this;

		host.lastUpdateTime = Time.time;
	}

	public JSONObject publicPropertyData {
		get {
			var data = new JSONObject();
			data.AddField("id", id);
			data.AddField("host", host.publicPropertyData);

			return data;
		}
	}
}
