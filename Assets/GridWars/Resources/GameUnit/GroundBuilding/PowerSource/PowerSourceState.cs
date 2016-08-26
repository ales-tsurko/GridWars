using UnityEngine;
using System.Collections;

public class PowerSourceState : GameUnitState {
	float _power;
	public float power {
		get {
			if (useBoltState) {
				return boltState.power;
			}
			else {
				return _power;
			}
		}

		set {
			if (useBoltState) {
				boltState.power = value;
			}
			else {
				_power = value;
			}
		}
	}

	public PowerSourceState() : base() {
	}

	public PowerSourceState(PowerSource p) : base(p) {
		power = p.maxPower;
	}

	public override void Write(UdpKit.UdpPacket packet) {
		base.Write(packet);
		packet.WriteFloat(_power);
	}

	public override void Read(UdpKit.UdpPacket packet) {
		base.Read(packet);
		_power = packet.ReadFloat();
	}

	public override void ApplyToBoltState() {
		base.ApplyToBoltState();
		boltState.power = power;
	}

	IPowerSourceState boltState {
		get {
			return gameUnit.boltEntity.GetState<IPowerSourceState>();
		}
	}
}
