using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class MatchmakerWaitForClientState : MatchmakerWaitForPeerState {
	Timer slowConnectionTimer;

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		slowConnectionTimer = app.timerCenter.NewTimer();
		slowConnectionTimer.action = SlowConnectionTimerFired;
		slowConnectionTimer.timeout = 5;
		slowConnectionTimer.Start();
	}

	void SlowConnectionTimerFired() {
		indicator.text = "Opponent can't connect.  Please check your" + (Application.platform == RuntimePlatform.WindowsPlayer ? " Windows" : "") + " firewall settings.";
	}

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
