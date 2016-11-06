using UnityEngine;
using System.Collections;

public class MatchmakerNetworkDelegateState : MatchmakerState, NetworkDelegate {
	public BoltConnection connection;

	public override void EnterFrom(AppState state) {
		base.EnterFrom(state);

		network.networkDelegate = this;
	}

	// NetworkDelegate

	public virtual void BoltStartDone() {
	}

	public virtual void BoltStartFailed() {
	}

	public virtual void ZeusConnected(UdpKit.UdpEndPoint endpoint) {
	}

	public virtual void ZeusConnectFailed() {
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
		this.connection = connection;
	}

	public virtual void BoltShutdownCompleted() {
	}

	public virtual void Disconnected(BoltConnection connection) {
	}
}
