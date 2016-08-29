using UnityEngine;
using System.Collections;

public class GameUnitState : Bolt.IProtocolToken {
	public GameUnit gameUnit;
	public bool useBoltState = false;

	//Only used during initialization.  Changes after initialization won't affect anything.
	public Vector3 position;
	public Quaternion rotation;

	public Transform transform {
		set {
			position = value.position;
			rotation = value.rotation;
		}
	}

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

	public GameUnit prefabGameUnit;

	public virtual GameUnit InstantiateGameUnit() {
		prefabGameUnit.gameUnitState = this;
		InitState();
		return prefabGameUnit.Instantiate();
	}

	public virtual GameUnit InstantiateGameUnit(System.Type gameUnitType) {
		prefabGameUnit = GameUnit.Load(gameUnitType);
		return InstantiateGameUnit();
	}

	public T InstantiateGameUnit<T>() where T: GameUnit {
		return (T) InstantiateGameUnit(typeof(T));
	}

	public virtual void InitState() {
		hitPoints = prefabGameUnit.maxHitPoints;
	}

	public virtual void Write(UdpKit.UdpPacket packet) {
		packet.WriteInt(playerNumber);
		packet.WriteVector3(position);
		packet.WriteQuaternion(rotation);
	}

	public virtual void Read(UdpKit.UdpPacket packet) {
		playerNumber = packet.ReadInt();
		position = packet.ReadVector3();
		rotation = packet.ReadQuaternion();
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