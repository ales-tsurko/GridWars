using UnityEngine;
using System.Collections.Generic;
using AssemblyCSharp;

public class PvpServer : DefaultNetworkDelegate {
	ServerToken serverToken;
	Timer requestSessionsTimer;
	bool startClient = false;

	public override List<Player> localPlayers {
		get {
			var list = new List<Player>();
			list.Add(Battlefield.current.PlayerNumbered(1));
			return list;
		}
	}

	public override void Start() {
		base.Start();
		//var endPoint = UdpKit.UdpEndPoint.
		//endPoint.Address = UdpKit.UdpIPv4Address.Any;
		//UdpKit.UdpEndPoint.Parse(Network.shared.listenEndpoint)
		App.shared.Log("BoltLauncher.StartServer", this);
		BoltLauncher.StartServer(UdpKit.UdpEndPoint.Parse("0.0.0.0:0"));
	}

	public override void BoltStartDone() {
		base.BoltStartDone();

		App.shared.Log("Setting Host Info.", this);
		serverToken = new ServerToken();
		serverToken.gameId = UnityEngine.Random.value;
		serverToken.gameTime = System.DateTime.UtcNow.Ticks;
		serverToken.isFull = false;
		BoltNetwork.SetHostInfo("GridWars", serverToken);

		Bolt.Zeus.Connect(UdpKit.UdpEndPoint.Parse(Network.shared.zeusEndpoint));
		App.shared.Log("Bolt.Zeus.Connect.", this);
	}

	public override void ZeusConnected(UdpKit.UdpEndPoint endpoint) {
		base.ZeusConnected(endpoint);
		RequestSessions();
	}

	void RequestSessions() {
		App.shared.Log("RequestSessions", this);
		CancelTimer();
		Bolt.Zeus.RequestSessionList();
	}

	public override void SessionListUpdated(UdpKit.Map<System.Guid, UdpKit.UdpSession> sessionList) {
		base.SessionListUpdated(sessionList);

		foreach (var session in sessionList) {
			if (session.Value.HostName == "GridWars") {
				var otherToken = session.Value.GetProtocolToken() as ServerToken;
				if (otherToken.gameId != serverToken.gameId && !otherToken.isFull && otherToken.gameTime < serverToken.gameTime) {
					App.shared.Log("Found Higher Priority Game.  Restarting as client.", this);
					startClient = true;
					Network.shared.RestartBolt();
					return;
				}
			}
		}

		if (requestSessionsTimer == null) {
			requestSessionsTimer = App.shared.timerCenter.NewTimer();
			requestSessionsTimer.timeout = 1f;
			requestSessionsTimer.action = RequestSessions;
			requestSessionsTimer.Start();
		}
	}

	public override void ConnectRequest(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token) {
		base.ConnectRequest(endpoint, token);
		if (Network.shared.isGameFull) {
			App.shared.Log("Game Full.  Refusing Connection.", this);
			BoltNetwork.Refuse(endpoint);
		}
		else {
			App.shared.Log("Game Empty.  Accepting Connection.", this);
			BoltNetwork.Accept(endpoint);
		}
	}

	public override void Connected(BoltConnection connection) {
		base.Connected(connection);
		if (Network.shared.isGameFull) {
			connection.Disconnect();
			App.shared.Log("Disconnecting unexpected connection during full game.", this);
		}
		else {
			App.shared.Log("Client Connected.", this);
			serverToken.isFull = true;
			BoltNetwork.SetHostInfo("GridWars", serverToken);
			this.connection = connection;
			Network.shared.StartGame();
			CancelTimer();
		}
	}

	public override void Disconnected(BoltConnection connection) {
		base.Disconnected(connection);
		if (connection == Network.shared.ConnectionForPlayer(Battlefield.current.PlayerNumbered(2))) {
			App.shared.Log("Opponent Disconnected.  Restarting.", this);
			Network.shared.LeaveGame();
		}
	}

	public override void BoltShutdownCompleted() {
		base.BoltShutdownCompleted();
		CancelTimer();
		if (startClient) {
			startClient = false;
			new PvpClient().Start();
		}
	}

	void CancelTimer() {
		if (requestSessionsTimer != null) {
			requestSessionsTimer.Cancel();
			requestSessionsTimer = null;
		}
	}

	public override void Cancel() {
		base.Cancel();

		Network.shared.LeaveGame();
		startClient = false;
	}
}
