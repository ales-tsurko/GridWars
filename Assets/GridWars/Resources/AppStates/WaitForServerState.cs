using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class WaitForServerState : BoltRendezvousState {
	//private 
	bool firstSessionListUpdate;
	int attempts = 0;
	Timer attemptTimer;

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		app.Log("StartClient", this);

		BoltLauncher.StartClient();
	}

	public override void ZeusConnected(UdpKit.UdpEndPoint endpoint) {
		base.ZeusConnected(endpoint);

		RequestSessionList();
	}

	void RequestSessionList() {
		app.Log("RequestSessionList", this);

		firstSessionListUpdate = true;

		Bolt.Zeus.RequestSessionList();
	}

	public override void SessionListUpdated(UdpKit.Map<System.Guid, UdpKit.UdpSession> sessionList) {
		base.SessionListUpdated(sessionList);

		if (firstSessionListUpdate) {
			firstSessionListUpdate = false;
			return;
		}

		attempts ++;

		if (attemptTimer != null) {
			attemptTimer.Cancel();
		}

		foreach (var session in sessionList) {
			if (session.Value.HostName == "GridWars") {
				var token = session.Value.GetProtocolToken() as ServerToken;
				if (token.gameId == gameId) {
					app.Log("Connecting To Game: " + token.gameId, this);
					BoltNetwork.Connect(session.Value);
					return;
				}
			}
		}

		attemptTimer = app.timerCenter.NewTimer();
		attemptTimer.action = RequestSessionList;
		attemptTimer.timeout = 0.25f;
		attemptTimer.Start();
	}

	public override void Connected(BoltConnection connection) {
		battlefield.PlayerNumbered(2).isLocal = true;

		base.Connected(connection);
	}

	public override void Cancel() {
		base.Cancel();

		if (attemptTimer != null) { //double click calls cancel twice
			attemptTimer.Cancel();
			attemptTimer = null;
		}
	}
}