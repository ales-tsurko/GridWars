public class ServerToken : Bolt.IProtocolToken {
	public string serverVersion = "0.1";
	public float gameId;
	public float gameTime;
	public bool isFull = false;

	public void Write(UdpKit.UdpPacket packet) {
		packet.WriteString(serverVersion);
		packet.WriteFloat(gameId);
		packet.WriteBool(isFull);
	}

	public void Read(UdpKit.UdpPacket packet) {
		serverVersion = packet.ReadString();
		gameId = packet.ReadFloat();
		isFull = packet.ReadBool();
	}
}