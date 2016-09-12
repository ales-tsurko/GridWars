using UnityEngine;
using System.Collections.Generic;

public class DefaultNetworkDelegate : UnityEngine.Object, NetworkDelegate {
	public bool boltStarted { get; set; }
	public BoltConnection connection { get; set; }
	public virtual List<Player> localPlayers {
		get {
			return new List<Player>();
		}
	}

	public virtual void Start() {
		boltStarted = false;
		Network.shared.networkDelegate = this;
		App.shared.Log("Start", this);
	}

	public virtual void BoltStartDone() {
		App.shared.Log("BoltStartDone", this);
		boltStarted = true;
	}

	public virtual void ZeusConnected(UdpKit.UdpEndPoint endpoint) {
		App.shared.Log("ZeusConnected", this);
	}

	public virtual void SessionListUpdated(UdpKit.Map<System.Guid, UdpKit.UdpSession> sessionList) {
		App.shared.Log("SessionListUpdated", this);
	}

	public virtual void BoltShutdownCompleted() {
		App.shared.Log("BoltShutdownCompleted", this);
	}

	public virtual void Connected(BoltConnection connection) {
		App.shared.Log("Connected", this);
	}

	public virtual void ConnectRequest(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token) {
		App.shared.Log("ConnectRequest", this);
	}

	public virtual void ConnectRefused(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token) {
		App.shared.Log("ConnectRefused", this);
	}

	public virtual void Disconnected(BoltConnection connection) {
		App.shared.Log("Disconnected", this);
	}

	public virtual void Cancel() {
		App.shared.Log("Cancel", this);
	}
}
