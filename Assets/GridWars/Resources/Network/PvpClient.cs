using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class PvpClient : DefaultNetworkDelegate {
	bool startServer = false;
	int processSessionsListCount = 0;
	Timer requestSessionListTimer;

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
		Bolt.Zeus.RequestSessionList();
	}

	public override void SessionListUpdated(UdpKit.Map<System.Guid, UdpKit.UdpSession> sessionList) {
		base.SessionListUpdated(sessionList);

		ProcessSessionsList();
	}

	void ProcessSessionsList() {
		processSessionsListCount ++;
		if (processSessionsListCount == 1) { //TODO HACK: Bolt has a bug where first SessionListUpdated is always empty.
			requestSessionListTimer = App.shared.timerCenter.NewTimer();
			requestSessionListTimer.timeout = 1f;
			requestSessionListTimer.action = ProcessSessionsList;
			requestSessionListTimer.Start();
		}
		else if (processSessionsListCount == 2) {
			requestSessionListTimer.Cancel();

			foreach (var session in BoltNetwork.SessionList) {
				if (session.Value.HostName == "GridWars") {
					var token = session.Value.GetProtocolToken() as ServerToken;
					if (!token.isFull) {
						App.shared.Log("Connecting To Game: " + token.gameId, this);
						BoltNetwork.Connect(session.Value);
						return;
					}
				}
			}

			App.shared.Log("No games found.  Restarting as server.", this);
			startServer = true; //Restart as server
			Network.shared.RestartBolt();
		}
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

	public override void BoltShutdownCompleted() {
		base.BoltShutdownCompleted();

		if (requestSessionListTimer != null) {
			requestSessionListTimer.Cancel();
			requestSessionListTimer = null;
		}

		if (startServer) {
			App.shared.Log("new PvpServer().Start()", this);
			startServer = false;
			new PvpServer().Start();
		}
	}
}
