using UnityEngine;
using System.Collections;

public interface BoltAgentDelegate {
	void BoltAgentConnected();
	void BoltAgentConnectFailed();
	void BoltAgentDisconnected();
	void BoltAgentDidShutdown();
}

public class BoltAgent : NetworkDelegate {
	public BoltAgentDelegate boltAgentDelegate;

	public App app {
		get {
			return App.shared;
		}
	}

	public Account account {
		get {
			return app.account;
		}
	}

	public Game game {
		get {
			return account.game;
		}
	}

	public Network network {
		get {
			return app.network;
		}
	}

	public virtual void Start() {
		app.Log("Start", this);
		network.networkDelegate = this;
	}

	public virtual void Shutdown() {
		app.Log("Shutdown", this);
		network.ShutdownBolt();
	}

	// NetworkDelegate

	public virtual void BoltStartDone() {
		//app.Log("BoltStartDone", this);
		Bolt.Zeus.Connect(UdpKit.UdpEndPoint.Parse(network.zeusEndpoint));
		app.Log("Bolt.Zeus.Connect", this);
	}

	public virtual void BoltStartFailed() {
		//app.Log("BoltStartFailed", this);
		if (boltAgentDelegate != null) {
			boltAgentDelegate.BoltAgentConnectFailed();
		}
	}

	public virtual void ZeusConnected(UdpKit.UdpEndPoint endpoint) {
		//app.Log("ZeusConnected", this);
	}

	public virtual void ZeusDisconnected() {
		//app.Log("ZeusDisconnected", this);
		network.ShutdownBolt();
	}

	public virtual void SessionListUpdated(UdpKit.Map<System.Guid, UdpKit.UdpSession> sessionList) {
		//app.Log("SessionListUpdated", this);
	}

	public virtual void ConnectRequest(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token) {
		//app.Log("ConnectRequest", this);
	}

	public virtual void ConnectRefused(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token) {
		//app.Log("ConnectRefused", this);
	}

	public virtual void Connected(BoltConnection connection) {
		//app.Log("Connected", this);
		if (boltAgentDelegate != null) {
			boltAgentDelegate.BoltAgentConnected();
		}
	}

	public virtual void BoltShutdownCompleted() {
		//app.Log("BoltShutdownCompleted", this);
		if (boltAgentDelegate != null) {
			boltAgentDelegate.BoltAgentDidShutdown();
		}
	}

	public virtual void Disconnected(BoltConnection connection) {
		//app.Log("Disconnected", this);
		if (boltAgentDelegate != null) {
			boltAgentDelegate.BoltAgentDisconnected();
		}
	}
}
