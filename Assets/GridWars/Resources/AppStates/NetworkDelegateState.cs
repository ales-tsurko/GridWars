using UnityEngine;
using System.Collections;

public class NetworkDelegateState : AppState, NetworkDelegate {

	public virtual void BoltStartDone() {
		app.Log("BoltStartDone", this);
	}

	public virtual void BoltStartFailed() {
		app.Log("BoltStartFailed", this);
	}

	public virtual void ZeusConnected(UdpKit.UdpEndPoint endpoint) {
		app.Log("ZeusConnected", this);
	}

	public virtual void ZeusDisconnected() {
		app.Log("ZeusDisconnected", this);
	}

	public virtual void SessionListUpdated(UdpKit.Map<System.Guid, UdpKit.UdpSession> sessionList) {
		app.Log("SessionListUpdated", this);
	}

	public virtual void ConnectRequest(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token) {
		app.Log("ConnectRequest", this);
	}

	public virtual void ConnectRefused(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token) {
		app.Log("ConnectRefused", this);
	}

	public virtual void Connected(BoltConnection connection) {
		app.Log("Connected", this);
	}

	public virtual void BoltShutdownCompleted() {
		app.Log("BoltShutdownCompleted", this);
	}

	public virtual void Disconnected(BoltConnection connection) {
		app.Log("Disconnected", this);
	}
}
