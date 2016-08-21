using UnityEngine;
using System.Collections;

public class TowerProtocolToken : Bolt.IProtocolToken {
	public string unitPrefabPath;

	public void Write(UdpKit.UdpPacket packet) {
		packet.WriteString(unitPrefabPath);
	}

	public void Read(UdpKit.UdpPacket packet) {
		unitPrefabPath = packet.ReadString();
	}
}
