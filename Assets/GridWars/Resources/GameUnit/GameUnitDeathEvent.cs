using UnityEngine;
using System.Collections;

public class GameUnitDeathEvent : Bolt.IProtocolToken {
	public GameUnit gameUnit;
	public Vector3 position;
	public Quaternion rotation;

	public Transform transform {
		set {
			position = value.position;
			rotation = value.rotation;
		}
	}
		
	public virtual void Write(UdpKit.UdpPacket packet) {
		packet.WriteVector3(position);
		packet.WriteQuaternion(rotation);
	}

	public virtual void Read(UdpKit.UdpPacket packet) {
		position = packet.ReadVector3();
		rotation = packet.ReadQuaternion();
	}

	public virtual void Apply() {}
}
