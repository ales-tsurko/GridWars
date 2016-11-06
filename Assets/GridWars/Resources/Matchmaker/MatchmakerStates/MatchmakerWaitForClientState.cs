using UnityEngine;
using System.Collections;

public class MatchmakerWaitForClientState : MatchmakerWaitForPeerState {
	protected override void StartBolt() {
		base.StartBolt();

		BoltLauncher.StartServer(UdpKit.UdpEndPoint.Parse("0.0.0.0:0"));
	}

	public override void BoltStartDone() {
		base.BoltStartDone();

		var serverToken = new ServerToken();
		serverToken.gameId = app.account.game.id;
		var serverName = "BareMetal";
		BoltNetwork.SetHostInfo(serverName, serverToken);
		app.Log("SetHostInfo: serverName: " + serverName + ", gameId: " + serverToken.gameId, this);
	}

	public override void ConnectRequest(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token) {
		base.ConnectRequest(endpoint, token);

		BoltNetwork.Accept(endpoint);
	}
}
