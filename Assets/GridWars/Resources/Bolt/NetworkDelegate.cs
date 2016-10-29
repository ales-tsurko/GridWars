using System.Collections.Generic;

public interface NetworkDelegate {
	void BoltStartDone();
	void BoltStartFailed();
	void ZeusConnected(UdpKit.UdpEndPoint endpoint);
	void ZeusDisconnected();
	void SessionListUpdated(UdpKit.Map<System.Guid, UdpKit.UdpSession> sessionList);
	void ConnectRequest(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token);
	void ConnectRefused(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token);
	void Connected(BoltConnection connection);
	void BoltShutdownCompleted();
	void Disconnected(BoltConnection connection);
	void ReceivedRematchRequest();
	void ReceivedConcede();
	void ReceivedAcceptRematch();
}