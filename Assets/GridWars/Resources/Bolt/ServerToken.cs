public class ServerToken : Bolt.IProtocolToken {
	public string serverVersion = "0.1";
	public string gameId;

	public void Write(UdpKit.UdpPacket packet) {
		packet.WriteString(serverVersion);
		packet.WriteString(gameId);
	}

	public void Read(UdpKit.UdpPacket packet) {
		serverVersion = packet.ReadString();
		gameId = packet.ReadString();
	}
}