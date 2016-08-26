using UnityEngine;
using System.Collections;

public class TowerState : GameUnitState {
	public string unitPrefabPath;

	public override void Write(UdpKit.UdpPacket packet) {
		base.Write(packet);
		packet.WriteString(unitPrefabPath);
	}

	public override void Read(UdpKit.UdpPacket packet) {
		base.Read(packet);
		unitPrefabPath = packet.ReadString();
	}

	public TowerState() : base() {
	}

	public TowerState(Tower t) : base(t) {
	}
}