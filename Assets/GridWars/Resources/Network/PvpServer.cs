using UnityEngine;
using System.Collections.Generic;
using AssemblyCSharp;

public class PvpServer : DefaultNetworkDelegate {
	public string gameId;

	ServerToken serverToken;

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
		serverToken.gameId = gameId;
		BoltNetwork.SetHostInfo("GridWars", serverToken);

		Bolt.Zeus.Connect(UdpKit.UdpEndPoint.Parse(Network.shared.zeusEndpoint));
		App.shared.Log("Bolt.Zeus.Connect.", this);
	}

	public override void ZeusConnected(UdpKit.UdpEndPoint endpoint) {
		base.ZeusConnected(endpoint);
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
			BoltNetwork.SetHostInfo("GridWars", serverToken);
			this.connection = connection;
			Network.shared.StartGame();
		}
	}

	public override void Disconnected(BoltConnection connection) {
		base.Disconnected(connection);
		if (connection == Network.shared.ConnectionForPlayer(Battlefield.current.PlayerNumbered(2))) {
			App.shared.Log("Opponent Disconnected.  Restarting.", this);
			Network.shared.LeaveGame();
		}
	}

	public override void Cancel() {
		base.Cancel();

		Network.shared.LeaveGame();
	}
}
