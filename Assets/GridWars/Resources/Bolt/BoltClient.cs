using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class BoltClient : BoltAgent {
	bool firstSessionListUpdate;
	Timer attemptTimer;
	bool didConnect;

	public override void Start () {
		base.Start();
		app.Log("BoltLauncher.StartClient()", this);
		didConnect = false;
		BoltLauncher.StartClient();
	}

	public override void ZeusConnected(UdpKit.UdpEndPoint endpoint) {
		base.ZeusConnected(endpoint);

		RequestSessionList();
	}

	void RequestSessionList() {
		app.Log("RequestSessionList", this);

		Bolt.Zeus.RequestSessionList();
	}

	public override void SessionListUpdated(UdpKit.Map<System.Guid, UdpKit.UdpSession> sessionList) {
		base.SessionListUpdated(sessionList);

		if (didConnect) { //sometimes bolt calls SessionListUpdated 2x for each RequestSessionList
			return;
		}

		CancelAttemptTimer();

		app.Log(sessionList.Count, this);

		foreach (var session in sessionList) {
			app.Log(session.Value.HostName);
			if (session.Value.HostName == "BareMetal") {
				var token = session.Value.GetProtocolToken() as ServerToken;
				app.Log(token.gameId, this);
				if (token.gameId == game.id) {
					app.Log("Connecting To Game: " + token.gameId, this);
					BoltNetwork.Connect(session.Value);
					didConnect = true;
					return;
				}
			}
		}

		attemptTimer = app.timerCenter.NewTimer();
		attemptTimer.action = RequestSessionList;
		attemptTimer.timeout = 1.0f;
		attemptTimer.Start();
	}

	public override void Shutdown() {
		base.Shutdown();
		CancelAttemptTimer();
	}

	void CancelAttemptTimer() {
		if (attemptTimer != null) {
			attemptTimer.Cancel();
			attemptTimer = null;
		}
	}
}
