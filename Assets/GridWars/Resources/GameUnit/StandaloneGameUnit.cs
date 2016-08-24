using UnityEngine;
using System.Collections;

public class StandaloneGameUnit : MonoBehaviour, GameUnitDelegate {

	//GameUnitDelegate implementation

	Player _player;
	public Player player {
		get {
			return _player;
		}

		set {
			_player = value;
		}
	}

	float _hitPoints;
	public float hitPoints {
		get {
			return _hitPoints;
		}

		set {
			_hitPoints = hitPoints;
		}
	}

	public T Instantiate<T>() where T: GameUnit { 
		return Instantiate(gameObject).GetComponent<T>();
	}

	//Mock NetworkObjectDelegate methods

	void Start() {
		if (BoltNetwork.isServer) {
			gameUnit.MasterStart();
		}
		gameUnit.SlaveStart();
	}

	void FixedUpdate() {
		if (BoltNetwork.isServer) {
			gameUnit.MasterFixedUpdate();
		}
		gameUnit.SlaveFixedUpdate();
	}

	//internal

	GameUnit gameUnit {
		get {
			return GetComponent<GameUnit>();
		}
	}
}
