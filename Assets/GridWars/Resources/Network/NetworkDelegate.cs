public interface NetworkDelegate {
	BoltConnection connection { get; set; }
	bool boltStarted { get; set; }
	void Start();
	void BoltStartDone();
	void ZeusConnected(UdpKit.UdpEndPoint endpoint);
	void SessionListUpdated(UdpKit.Map<System.Guid, UdpKit.UdpSession> sessionList);
	void ConnectRequest(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token);
	void ConnectRefused(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token);
	void Connected(BoltConnection connection);
	void BoltShutdownCompleted();
	void Disconnected(BoltConnection connection);
}