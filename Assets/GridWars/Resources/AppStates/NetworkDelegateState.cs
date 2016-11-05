using UnityEngine;
using System.Collections;

public class NetworkDelegateState : AppState, NetworkDelegate {

	public virtual void BoltStartDone() {
	}

	public virtual void BoltStartFailed() {
	}

	public virtual void ZeusConnected(UdpKit.UdpEndPoint endpoint) {
	}

	public virtual void ZeusDisconnected() {
	}

	public virtual void SessionListUpdated(UdpKit.Map<System.Guid, UdpKit.UdpSession> sessionList) {
	}

	public virtual void ConnectRequest(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token) {
	}

	public virtual void ConnectRefused(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token) {
	}

	public virtual void Connected(BoltConnection connection) {
	}

	public virtual void BoltShutdownCompleted() {
	}

	public virtual void Disconnected(BoltConnection connection) {
	}
}
