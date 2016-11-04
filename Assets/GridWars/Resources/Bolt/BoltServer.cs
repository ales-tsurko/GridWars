using UnityEngine;
using System.Collections;

public class BoltServer : BoltAgent {

	public override void Start() {
		base.Start();

		app.Log("BoltLauncher.StartServer()", this);
		BoltLauncher.StartServer(UdpKit.UdpEndPoint.Parse("0.0.0.0:0"));
	}

	public override void BoltStartDone() {
		base.BoltStartDone();

		app.Log("Setting Host Info", this);
		var serverToken = new ServerToken();
		serverToken.gameId = app.account.game.id;
		BoltNetwork.SetHostInfo("GridWars", serverToken);
	}

	public override void ConnectRequest(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token) {
		base.ConnectRequest(endpoint, token);

		BoltNetwork.Accept(endpoint);
	}
}
