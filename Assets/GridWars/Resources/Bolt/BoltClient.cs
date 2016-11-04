using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class BoltClient : BoltAgent {
	bool firstSessionListUpdate;
	int attempts = 0;
	Timer attemptTimer;

	public override void Start () {
		base.Start();
		app.Log("BoltLauncher.StartClient()", this);
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

		//Bolt sometimes calls SessionListUpdated with an empty sessionList and then calls it again on a subsequent Update.
		if (firstSessionListUpdate && sessionList.Count == 0) {
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
				if (token.gameId == game.id) {
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

	public override void Shutdown() {
		base.Shutdown();

		if (attemptTimer != null) { //double click calls cancel twice
			attemptTimer.Cancel();
			attemptTimer = null;
		}
	}
}
