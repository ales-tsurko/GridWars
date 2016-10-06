using UnityEngine;
using System.Collections;

public class WaitForClientState : BoltRendezvousState {

	//private

	ServerToken serverToken;

	//AppState
	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		app.Log("BoltLauncher.StartServer()", this);
		BoltLauncher.StartServer(UdpKit.UdpEndPoint.Parse("0.0.0.0:0"));
	}

	//NetworkDelegate

	public override void BoltStartDone() {
		base.BoltStartDone();

		app.Log("Setting Host Info", this);
		serverToken = new ServerToken();
		serverToken.gameId = gameId;
		BoltNetwork.SetHostInfo("GridWars", serverToken);
	}

	public override void ConnectRequest(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token) {
		base.ConnectRequest(endpoint, token);

		BoltNetwork.Accept(endpoint);
	}
}