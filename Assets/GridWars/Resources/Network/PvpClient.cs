using UnityEngine;
using System.Collections.Generic;
using AssemblyCSharp;

public class PvpClient : DefaultNetworkDelegate {
	public string gameId;

	int attempts = 0;
	Timer attemptTimer;
	bool firstSessionListUpdate; //Bolt has a bug where it calls SessionListUpdated twice.

	public override List<Player> localPlayers {
		get {
			var list = new List<Player>();
			list.Add(Battlefield.current.PlayerNumbered(2));
			return list;
		}
	}

	public override void Start() {
		base.Start();
		App.shared.Log("StartClient", this);
		BoltLauncher.StartClient();
	}

	public override void BoltStartDone() {
		base.BoltStartDone();
		Bolt.Zeus.Connect(UdpKit.UdpEndPoint.Parse(Network.shared.zeusEndpoint));
		App.shared.Log("Bolt.Zeus.Connect", this);
	}

	public override void ZeusConnected(UdpKit.UdpEndPoint endpoint) {
		base.ZeusConnected(endpoint);

		RequestSessionList();
	}

	void RequestSessionList() {
		App.shared.Log("RequestSessionList", this);
		firstSessionListUpdate = true;
		Bolt.Zeus.RequestSessionList();
	}

	public override void SessionListUpdated(UdpKit.Map<System.Guid, UdpKit.UdpSession> sessionList) {
		if (firstSessionListUpdate) {
			firstSessionListUpdate = false;
			return;
		}
		base.SessionListUpdated(sessionList);

		attempts ++;

		if (attemptTimer != null) {
			attemptTimer.Cancel();
		}

		foreach (var session in sessionList) {
			if (session.Value.HostName == "GridWars") {
				var token = session.Value.GetProtocolToken() as ServerToken;
				if (token.gameId == gameId) {
					App.shared.Log("Connecting To Game: " + token.gameId, this);
					BoltNetwork.Connect(session.Value);
					return;
				}
			}
		}

		attemptTimer = App.shared.timerCenter.NewTimer();
		attemptTimer.action = RequestSessionList;
		attemptTimer.timeout = 0.25f;
		attemptTimer.Start();

		//RequestSessionList();
	}

	public override void Connected(BoltConnection connection) {
		base.Connected(connection);
		this.connection = connection;

		Network.shared.StartGame();
	}

	public override void Disconnected(BoltConnection connection) { //TODO: handle reconnects
		base.Disconnected(connection);
		App.shared.Log("Opponent Disconnected.  Restarting.", this);

		Network.shared.LeaveGame(false);
	}

	public override void Cancel() {
		base.Cancel();

		attemptTimer.Cancel();
		attemptTimer = null;

		Network.shared.LeaveGame();
	}
}
