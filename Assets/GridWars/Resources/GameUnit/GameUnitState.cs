using UnityEngine;
using System.Collections;

public class GameUnitState : Bolt.IProtocolToken {
	public GameUnit gameUnit;
	public bool useBoltState = false;

	int _playerNumber = 0;
	public int playerNumber {
		get {
			return _playerNumber;
		}

		set {
			_playerNumber = value;
		}
	}

	public Player player {
		get {
			return Battlefield.current.PlayerNumbered(playerNumber);
		}

		set {
			if (value == null) {
				playerNumber = -1;
			}
			else {
				playerNumber = value.playerNumber;
			}
		}
	}

	float _hitPoints = 0.0f;
	public float hitPoints {
		get {
			if (useBoltState) {
				return boltState.hitPoints;
			}
			else {
				return _hitPoints;
			}
		}

		set {
			if (useBoltState) {
				boltState.hitPoints = value;
			}
			else {
				_hitPoints = value;
			}

		}
	}

	public GameUnitState() {
	}

	public GameUnitState(GameUnit prefabGameUnit) {
		hitPoints = prefabGameUnit.maxHitPoints;
	}

	public virtual void Write(UdpKit.UdpPacket packet) {
		packet.WriteInt(playerNumber);
	}

	public virtual void Read(UdpKit.UdpPacket packet) {
		playerNumber = packet.ReadInt();
	}

	public virtual void ApplyToBoltState() {
		boltState.hitPoints = hitPoints;
	}

	IGameUnitState boltState {
		get {
			return gameUnit.boltEntity.GetState<IGameUnitState>();
		}
	}
}